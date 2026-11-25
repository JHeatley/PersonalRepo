using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// v177: downloads the Japanese printings' card images the game doesn't have yet. Resources/Lang/images_ja.txt lists, for each card,
    /// its Japanese printing's image (pokemon-card.com, the official Japanese card database, or pkmn.gg's scans). Each image is saved as
    /// SET_NUMBER in a "ja" folder inside the set's folder of CardImages (the folder you already have for that set, or one named after
    /// the set code), where the image loader picks it up for Japanese mode. One image a second on a background thread; stopping and
    /// starting again carries on where it left off.
    /// </summary>
    public sealed class JapaneseImages
    {
        private sealed class Item
        {
            public string Key, SetCode, Number, Url;
        }

        private readonly List<Item> items = new List<Item>();
        private Thread worker;
        private volatile bool cancel;
        private volatile bool running;
        private int done, saved, failed, skipped;
        private string lastError;
        private volatile bool finished;

        public bool Running { get { return running; } }
        public int Total { get { return items.Count; } }
        public int Done { get { return done; } }
        public int Saved { get { return saved; } }
        public int Failed { get { return failed; } }
        public string LastError { get { return lastError; } }

        /// <summary>True once after a run ends (so the caller reloads the images).</summary>
        public bool TakeFinished()
        {
            if (!finished) return false;
            finished = false;
            return true;
        }

        /// <summary>The cards whose Japanese image isn't there yet (from images_ja.txt, for cards in the pool).</summary>
        public static JapaneseImages Missing(CardDatabase db)
        {
            var job = new JapaneseImages();
            var list = Resources.Load<TextAsset>("Lang/images_ja");
            if (list == null || db == null) return job;
            foreach (var raw in list.text.Split('\n'))
            {
                var line = raw.TrimEnd('\r');
                if (line.Length == 0 || line[0] == '#') continue;
                int tab = line.IndexOf('\t');
                if (tab <= 0) continue;
                var def = db.Get(line.Substring(0, tab));
                if (def == null || CardVisuals.HasJapaneseImage(def)) continue;
                job.items.Add(new Item { Key = def.Key, SetCode = def.SetCode, Number = def.Number, Url = ExpandUrl(line.Substring(tab + 1)) });
            }
            return job;
        }

        /// <summary>"pcc:" is pokemon-card.com's image path; "gg:" pkmn.gg's; anything else is a whole address.</summary>
        public static string ExpandUrl(string url)
        {
            if (url.StartsWith("pcc:", StringComparison.Ordinal)) return "https://www.pokemon-card.com" + url.Substring(4);
            if (url.StartsWith("gg:", StringComparison.Ordinal)) return "https://assets.pkmn.gg/fit-in/600x836/filters:format(webp)/images/cards/" + url.Substring(3);
            return url;
        }

        public void Start()
        {
            if (running || items.Count == 0) return;
            cancel = false;
            running = true;
            string root = Application.isMobilePlatform ? CardVisuals.DeviceImageFolder : CardVisuals.ImageFolder;
            var work = items.ToList();
            worker = new Thread(() => Run(root, work)) { IsBackground = true, Name = "Japanese card images" };
            worker.Start();
        }

        public void Stop() { cancel = true; }

        private void Run(string root, List<Item> work)
        {
            try
            {
                Directory.CreateDirectory(root);
                var folders = SetFolders(root);
                using (var http = new HttpClient { Timeout = TimeSpan.FromSeconds(40) })
                {
                    http.DefaultRequestHeaders.UserAgent.ParseAdd("PokemonTCGSim/1.0");
                    DateTime last = DateTime.MinValue;
                    foreach (var item in work)
                    {
                        if (cancel) break;
                        string folder = Path.Combine(root, FolderFor(folders, item.SetCode), CardVisuals.JapaneseFolderName);
                        string stem = Path.Combine(folder, SafeName(item.SetCode) + "_" + SafeName(item.Number));
                        if (File.Exists(stem + ".jpg") || File.Exists(stem + ".png") || File.Exists(stem + ".webp"))
                        {
                            Interlocked.Increment(ref skipped);
                            Interlocked.Increment(ref done);
                            continue;
                        }
                        // One request a second: these are other people's servers.
                        var wait = TimeSpan.FromSeconds(1) - (DateTime.UtcNow - last);
                        if (wait > TimeSpan.Zero) Thread.Sleep(wait);
                        last = DateTime.UtcNow;
                        try
                        {
                            var bytes = http.GetByteArrayAsync(item.Url).Result;
                            string ext = Extension(bytes);
                            if (ext == null) throw new IOException("not an image");
                            Directory.CreateDirectory(folder);
                            File.WriteAllBytes(stem + ext, bytes);
                            Interlocked.Increment(ref saved);
                        }
                        catch (Exception e)
                        {
                            Interlocked.Increment(ref failed);
                            lastError = item.Key + ": " + (e.InnerException ?? e).Message;
                        }
                        Interlocked.Increment(ref done);
                    }
                }
            }
            catch (Exception e)
            {
                lastError = e.Message;
            }
            finally
            {
                running = false;
                finished = true;
            }
        }

        /// <summary>The set folders already in CardImages, by set code (the first one found for each set).</summary>
        private static Dictionary<string, string> SetFolders(string root)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                foreach (var dir in Directory.GetDirectories(root))
                {
                    string name = Path.GetFileName(dir);
                    string code = CardVisuals.SetCodeForFolder(name);
                    if (code != null && !map.ContainsKey(code)) map[code] = name;
                }
            }
            catch (Exception) { }
            return map;
        }

        private static string FolderFor(Dictionary<string, string> folders, string setCode)
        {
            string name;
            if (folders.TryGetValue(setCode, out name)) return name;
            folders[setCode] = SafeName(setCode);
            return folders[setCode];
        }

        private static string SafeName(string s)
        {
            foreach (var c in Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
            return s.Replace('?', '_');
        }

        /// <summary>The file type from the first bytes: JPEG, PNG or WebP (null for anything else, like an error page).</summary>
        public static string Extension(byte[] b)
        {
            if (b == null || b.Length < 12) return null;
            if (b[0] == 0xFF && b[1] == 0xD8) return ".jpg";
            if (b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47) return ".png";
            if (b[0] == 'R' && b[1] == 'I' && b[2] == 'F' && b[3] == 'F' && b[8] == 'W' && b[9] == 'E' && b[10] == 'B' && b[11] == 'P') return ".webp";
            return null;
        }
    }
}
