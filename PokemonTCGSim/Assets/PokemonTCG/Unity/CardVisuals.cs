using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PokemonTCG.Core;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// Card textures and materials. Real card images are read from StreamingAssets/CardImages (sub-folders too).
    /// A file matches a card when its name starts with the set code and number, in any of these styles:
    /// TWM_130.png, TWM-130.jpg, TWM 130 Dragapult ex.png, TWM_130_R_EN_LG.png, MEE_013.png.
    /// A file named with just the card number (4.png, 004 Charizard.png, 4_hires.jpg) takes its set from the folder it's in,
    /// named with the set code or name: BS/4.png, Base Set/4.png, "Base Set (BS)/4.png" (v47).
    /// Images saved from pkmncards.com keep their names as saved (v69), in each of the site's styles: sv6_en_001_std.jpg,
    /// en_US-SWSH9-001-exeggcute.jpg, en_US-Promo_SWSH-SWSH289_morpeko_v_union.jpg, charizard-base-set-bs-4.jpg, with or
    /// without the size a thumbnail gets (-312x436).
    /// Big images are scaled down to 512 pixels high and compressed, so a full pool doesn't use much memory.
    /// Cards without an image get a plain coloured placeholder with the name drawn by the UI.
    /// Works with both URP and the Built-in render pipeline.
    /// </summary>
    public static class CardVisuals
    {
        private const int MaxImageHeight = 512;

        private static Shader shader;
        private static readonly Dictionary<string, Texture2D> faces = new Dictionary<string, Texture2D>();
        private static readonly HashSet<string> hasImage = new HashSet<string>();
        private static readonly Dictionary<Texture, Material> materials = new Dictionary<Texture, Material>();
        private static readonly Dictionary<Color32, Texture2D> solids = new Dictionary<Color32, Texture2D>();
        private static Texture2D back;
        private static Texture2D disc;

        /// <summary>Card key ("TWM-130") -> image file. Built on first use; ReloadImages() rescans the folder.</summary>
        private static Dictionary<string, string> index;
        /// <summary>Files in the image folder that can't be used, with the reason.</summary>
        public static readonly List<string> SkippedFiles = new List<string>();
        /// <summary>Files that matched a card name pattern but couldn't be decoded (often WebP renamed to .png).</summary>
        public static readonly List<string> FailedFiles = new List<string>();
        /// <summary>
        /// v153.1: AVIF or WebP pictures with a .jpg or .png name (PokéBeach's images, renamed) being converted to PNG: the
        /// original file -> the card keys showing a placeholder until it's done (null when converting it failed).
        /// </summary>
        private static readonly Dictionary<string, List<string>> relabeling = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        /// <summary>v153.1: conversions finished on a background thread, picked up by PumpLoads.</summary>
        private static readonly System.Collections.Concurrent.ConcurrentQueue<RelabeledImage> relabeled = new System.Collections.Concurrent.ConcurrentQueue<RelabeledImage>();

        private sealed class RelabeledImage
        {
            public string Source;
            public string Png;
            public string Problem;
        }

        /// <summary>Known set codes and set names (normalized: lower-case letters and digits) -> set code, from the card pool.</summary>
        private static Dictionary<string, string> setsByCode, setsByName;
        /// <summary>Set names with the "&amp;" dropped rather than read as "and" ("diamondpearl"), the way a web slug writes them (v69).</summary>
        private static Dictionary<string, string> setsByBareName;
        /// <summary>
        /// v168.1: the pool's cards numbered with a letter or a sign rather than digits (Unseen Forces' Unown UF A-Z, ! and ?):
        /// upper-case key -> the pool's key, so "V.png" in a UF folder and pkmncards.com's "unown-unseen-forces-uf-v" find them.
        /// </summary>
        private static Dictionary<string, string> letterKeys;

        public static string ImageFolder
        {
            get { return Path.Combine(Application.streamingAssetsPath, "CardImages"); }
        }

        /// <summary>
        /// v118: the list of image files the build put in the app (an Android APK can't list its StreamingAssets, so the Build
        /// Android APK command and every build write this file; paths are relative to CardImages, one per line).
        /// </summary>
        public const string ImageListFileName = "_image_list.txt";

        /// <summary>
        /// v118: on a phone or tablet, card images can also go in this folder on the device (the app's own files folder:
        /// Android/data/(the app's package name)/files/CardImages). They're used along with the ones built into the app.
        /// </summary>
        public static string DeviceImageFolder
        {
            get { return Path.Combine(Application.persistentDataPath, "CardImages"); }
        }

        /// <summary>v118: StreamingAssets are inside the app's package (Android), so they're read through UnityWebRequest, not files.</summary>
        private static bool ImagesInsidePackage
        {
            get { return Application.platform == RuntimePlatform.Android; }
        }

        /// <summary>Reads a file in StreamingAssets on Android (a jar: URL), waiting for it. Null when it can't be read.</summary>
        private static byte[] ReadPackagedFile(string url)
        {
            using (var request = UnityWebRequest.Get(url))
            {
                var op = request.SendWebRequest();
                var until = DateTime.UtcNow.AddSeconds(10);
                while (!op.isDone && DateTime.UtcNow < until) System.Threading.Thread.Sleep(1);
                if (!op.isDone || request.result != UnityWebRequest.Result.Success) return null;
                return request.downloadHandler.data;
            }
        }

        /// <summary>The image files built into an Android app (from the image list), relative to CardImages.</summary>
        private static List<string> PackagedImageList()
        {
            var list = new List<string>();
            var bytes = ReadPackagedFile(ImageFolder + "/" + ImageListFileName);
            if (bytes == null) return list;
            foreach (var line in Encoding.UTF8.GetString(bytes).Split('\n'))
            {
                string rel = line.Trim(); // the build writes '/' separators
                if (rel.Length > 0) list.Add(rel);
            }
            return list;
        }

        /// <summary>The folders of a relative image path, innermost first (like FoldersOf).</summary>
        private static List<string> FoldersOfRelative(string rel)
        {
            var parts = rel.Split('/');
            var list = new List<string>();
            for (int k = parts.Length - 2; k >= 0; k--)
                if (parts[k].Length > 0) list.Add(parts[k]);
            return list;
        }

        public static Material MaterialFor(Texture texture)
        {
            Material m;
            if (materials.TryGetValue(texture, out m) && m != null) return m;
            if (shader == null)
            {
                // The game's own shader first (Resources/Shaders/CardUnlit.shader, always in a build: v54.1). Unity's built-in
                // shaders found by name are only in a build if something else uses them; "Sprites/Default" always is, but it
                // doesn't write depth, so with it the playmat hid every card on the far side of the table.
                shader = Resources.Load<Shader>("Shaders/CardUnlit");
                if (shader == null || !shader.isSupported) shader = Shader.Find("PokemonTCG/Card Unlit");
                if (shader == null || !shader.isSupported)
                {
                    bool urp = GraphicsSettings.currentRenderPipeline != null;
                    shader = Shader.Find(urp ? "Universal Render Pipeline/Unlit" : "Unlit/Texture");
                    if (shader == null) shader = Shader.Find("Unlit/Texture");
                    if (shader == null) shader = Shader.Find("Sprites/Default");
                }
                if (shader == null) Debug.LogError("PokemonTCG: no shader found to draw the cards with.");
                else if (shader.renderQueue >= 2500)
                    Debug.LogWarning("PokemonTCG: drawing cards with " + shader.name + ", which doesn't use depth; cards may be hidden by the playmat.");
                else Debug.Log("PokemonTCG: drawing cards with " + shader.name + ".");
            }
            m = new Material(shader);
            m.mainTexture = texture;
            materials[texture] = m;
            return m;
        }

        public static bool HasImage(CardDef def)
        {
            if (def == null) return false;
            Face(def);
            return hasImage.Contains(def.Key);
        }

        /// <summary>True when an image for this card can be shown (its own, or its main printing's), without loading it.</summary>
        public static bool HasImageFile(CardDef def)
        {
            return def != null && (Index().ContainsKey(def.Key) || (def.IsAlternatePrint && Index().ContainsKey(def.BaseKey)));
        }

        /// <summary>True when this exact printing has its own image file.</summary>
        public static bool HasOwnImageFile(CardDef def)
        {
            return def != null && Index().ContainsKey(def.Key);
        }

        /// <summary>The card's face: its image if there is one, otherwise a coloured placeholder. Loads on first use.</summary>
        public static Texture2D Face(CardDef def)
        {
            if (def == null || def.IsFaceDown) return Back(); // v126: Lt. Surge's Secret Plan's face-down card
            Texture2D tex;
            if (faces.TryGetValue(def.Key, out tex) && tex != null) return tex;
            tex = LoadImage(def);
            if (tex != null) hasImage.Add(def.Key);
            else tex = Placeholder(def);
            faces[def.Key] = tex;
            return tex;
        }

        /// <summary>
        /// v134.6: card images being read and decoded in the background (by card key). A picture used to be read, decoded,
        /// scaled and compressed on the thread that draws the screen, which stopped the game for seconds when a new game
        /// showed both opening hands while the AI tables were busy. Now Unity decodes the file on its own threads, the
        /// graphics card scales it, and the finished picture is compressed a couple per frame (PumpLoads, every frame).
        /// </summary>
        private sealed class PendingImage
        {
            public CardDef Def;
            public UnityWebRequest Request;
        }

        private static readonly Dictionary<string, PendingImage> loading = new Dictionary<string, PendingImage>(StringComparer.Ordinal);
        /// <summary>Images read and decoded at the same time.</summary>
        private const int MaxLoadsInFlight = 6;
        /// <summary>Decoded images scaled and compressed per frame.</summary>
        private const int FinishesPerFrame = 2;
        private static int pumpFrame = -1;
        /// <summary>Images read the old way (all at once on this thread) when one can't be read in the background: 1 per frame.</summary>
        private static int directFrame = -1;
        private static int directLeft;

        /// <summary>
        /// The card's face for the board if it's ready; null while its image is still loading (the caller shows the card back
        /// meanwhile and asks again next frame). A card without an image file gets its coloured placeholder at once.
        /// </summary>
        public static Texture2D FaceWithinBudget(CardDef def)
        {
            if (def == null || def.IsFaceDown) return Back();
            Texture2D tex;
            if (faces.TryGetValue(def.Key, out tex) && tex != null) return tex;
            if (!HasImageFile(def)) return Face(def); // no file: the coloured placeholder, which is quick
            return LoadInBackground(def);
        }

        /// <summary>
        /// True when the board shows this card's image, or will once it has loaded; false for a coloured placeholder (the
        /// game screen writes the card's name on those). Never loads anything.
        /// </summary>
        public static bool HasBoardImage(CardDef def)
        {
            if (def == null) return false;
            Texture2D tex;
            if (faces.TryGetValue(def.Key, out tex) && tex != null) return hasImage.Contains(def.Key);
            return HasImageFile(def);
        }

        /// <summary>
        /// For lists that show many cards: returns the card's image if it's loaded, starting to load it only while
        /// loadBudget lasts (each card still loading uses 1). Returns null when there's no image (yet), so the caller draws its own placeholder.
        /// </summary>
        public static Texture2D TryImage(CardDef def, ref int loadBudget)
        {
            if (def == null) return null;
            if (def.IsFaceDown) return Back(); // v126: Lt. Surge's Secret Plan's face-down card
            Texture2D tex;
            if (faces.TryGetValue(def.Key, out tex) && tex != null) return hasImage.Contains(def.Key) ? tex : null;
            if (!HasImageFile(def) || loading.ContainsKey(def.Key)) return null; // still loading: costs no budget
            if (loadBudget <= 0) return null;
            loadBudget--;
            tex = LoadInBackground(def);
            return tex != null && hasImage.Contains(def.Key) ? tex : null;
        }

        /// <summary>Starts loading a card's image file (or keeps waiting for it). Null until it's ready.</summary>
        private static Texture2D LoadInBackground(CardDef def)
        {
            Texture2D tex;
            if (faces.TryGetValue(def.Key, out tex) && tex != null) return tex;
            if (loading.ContainsKey(def.Key)) return null;
            if (loading.Count >= MaxLoadsInFlight) return null;
            string uri = BackgroundUri(def);
            if (uri == null) return DirectLoadAllowed() ? Face(def) : null;
            UnityWebRequest request = null;
            try
            {
                request = UnityWebRequestTexture.GetTexture(uri, true);
                request.SendWebRequest();
            }
            catch (Exception)
            {
                if (request != null) request.Dispose();
                return DirectLoadAllowed() ? Face(def) : null;
            }
            loading[def.Key] = new PendingImage { Def = def, Request = request };
            return null;
        }

        /// <summary>The address Unity reads the card's image from in the background, or null when it has to be read directly.</summary>
        private static string BackgroundUri(CardDef def)
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null) return null;
            string path;
            if (!Index().TryGetValue(def.Key, out path) && !(def.IsAlternatePrint && Index().TryGetValue(def.BaseKey, out path))) return null;
            if (path.StartsWith("jar:", StringComparison.OrdinalIgnoreCase)) return path; // v118: an image inside the Android app
            // Characters a file address can mistake for something else: read those files directly.
            if (path.IndexOf('#') >= 0 || path.IndexOf('%') >= 0 || path.IndexOf('?') >= 0) return null;
            try { return new Uri(Path.GetFullPath(path)).AbsoluteUri; }
            catch (Exception) { return null; }
        }

        private static bool DirectLoadAllowed()
        {
            if (Time.frameCount != directFrame)
            {
                directFrame = Time.frameCount;
                directLeft = 1;
            }
            if (directLeft <= 0) return false;
            directLeft--;
            return true;
        }

        /// <summary>
        /// v134.6: finishes the card images that have been decoded (a couple per frame): scaled on the graphics card,
        /// compressed, and kept for every screen. Call once per frame from Update (not OnGUI: it draws into a render texture).
        /// </summary>
        public static void PumpLoads()
        {
            if (!relabeled.IsEmpty) FinishRelabeled();
            if (loading.Count == 0 || Time.frameCount == pumpFrame) return;
            pumpFrame = Time.frameCount;
            List<PendingImage> done = null;
            foreach (var p in loading.Values)
            {
                if (!p.Request.isDone) continue;
                if (done == null) done = new List<PendingImage>();
                done.Add(p);
                if (done.Count >= FinishesPerFrame) break;
            }
            if (done == null) return;
            foreach (var p in done)
            {
                loading.Remove(p.Def.Key);
                FinishLoad(p);
            }
        }

        private static void FinishLoad(PendingImage p)
        {
            var def = p.Def;
            Texture2D full = null;
            try
            {
                if (p.Request.result == UnityWebRequest.Result.Success) full = DownloadHandlerTexture.GetContent(p.Request);
            }
            catch (Exception)
            {
                full = null;
            }
            finally
            {
                p.Request.Dispose();
            }
            Texture2D tex = null;
            if (full != null)
            {
                try { tex = ScaleOnGraphicsCard(full, def.Key); }
                catch (Exception e)
                {
                    Debug.LogWarning("[PokemonTCG] Couldn't scale card image " + def.Key + " on the graphics card (" + e.Message + "); reading it directly.");
                    tex = null;
                }
                UnityEngine.Object.Destroy(full);
            }
            Texture2D existing;
            if (faces.TryGetValue(def.Key, out existing) && existing != null)
            {
                // Loaded some other way meanwhile (a screen that reads images directly).
                if (tex != null) UnityEngine.Object.Destroy(tex);
                return;
            }
            if (tex == null)
            {
                // Unity couldn't read it in the background: the old way, which also reports a file it can't decode.
                Face(def);
                return;
            }
            hasImage.Add(def.Key);
            faces[def.Key] = tex;
        }

        /// <summary>
        /// The decoded image at most MaxImageHeight pixels high, as a compressed texture with mipmaps (like Shrink), scaled on
        /// the graphics card: halved until it's within twice the size (an even box average), then drawn at the final size.
        /// </summary>
        private static Texture2D ScaleOnGraphicsCard(Texture2D full, string name)
        {
            int sw = full.width, sh = full.height;
            bool shrink = sh > MaxImageHeight;
            int dh = shrink ? MaxImageHeight : sh;
            int dw = shrink ? Mathf.Max(4, Mathf.RoundToInt(sw * (float)dh / sh)) : sw;
            if (shrink) dw = (dw + 3) / 4 * 4; // block compression wants multiples of 4
            full.filterMode = FilterMode.Bilinear;
            full.wrapMode = TextureWrapMode.Clamp;

            var previous = RenderTexture.active;
            RenderTexture step = null, target = null;
            Texture2D dst = null;
            try
            {
                Texture source = full;
                int cw = sw, ch = sh;
                while (cw >= dw * 2 && ch >= dh * 2)
                {
                    cw /= 2;
                    ch /= 2;
                    var next = RenderTexture.GetTemporary(cw, ch, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
                    next.filterMode = FilterMode.Bilinear;
                    next.wrapMode = TextureWrapMode.Clamp;
                    Graphics.Blit(source, next);
                    if (step != null) RenderTexture.ReleaseTemporary(step);
                    step = next;
                    source = next;
                }
                target = RenderTexture.GetTemporary(dw, dh, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
                Graphics.Blit(source, target);
                dst = new Texture2D(dw, dh, TextureFormat.RGBA32, true);
                RenderTexture.active = target;
                dst.ReadPixels(new Rect(0, 0, dw, dh), 0, 0, false);
            }
            catch (Exception)
            {
                if (dst != null) UnityEngine.Object.Destroy(dst);
                throw;
            }
            finally
            {
                RenderTexture.active = previous;
                if (step != null) RenderTexture.ReleaseTemporary(step);
                if (target != null) RenderTexture.ReleaseTemporary(target);
            }
            dst.Apply(true, false);
            if (shrink) dst.Compress(true);
            dst.Apply(false, true);
            dst.wrapMode = TextureWrapMode.Clamp;
            dst.anisoLevel = 4;
            dst.name = name;
            return dst;
        }

        /// <summary>Forgets every loaded image and rescans the folder (use after adding or renaming image files).</summary>
        public static void ReloadImages()
        {
            // Only card faces go; materials for the card back and the table slots stay in use.
            var faceTextures = new HashSet<Texture>(faces.Values.Where(t => t != null).Cast<Texture>());
            foreach (var t in faceTextures)
            {
                Material m;
                if (materials.TryGetValue(t, out m))
                {
                    if (m != null) UnityEngine.Object.Destroy(m);
                    materials.Remove(t);
                }
                UnityEngine.Object.Destroy(t);
            }
            faces.Clear();
            hasImage.Clear();
            FailedFiles.Clear();
            relabeling.Clear(); // v153.1: try converting again (a codec may have been installed since)
            foreach (var p in loading.Values) p.Request.Dispose();
            loading.Clear();
            indexWarming = null;
            index = null;
        }

        /// <summary>Every card key that has an image file, with the file's path.</summary>
        public static Dictionary<string, string> ImageFiles()
        {
            return new Dictionary<string, string>(Index());
        }

        /// <summary>
        /// Learns the set codes and set names of the card pool, so image folders named after a set ("BS", "Base Set",
        /// "Base Set (BS)") give their set to files named with just the card number. Called when the cards are loaded.
        /// </summary>
        public static void UseSetsFrom(CardDatabase db)
        {
            var byCode = new Dictionary<string, string>();
            var byName = new Dictionary<string, string>();
            var byBareName = new Dictionary<string, string>();
            var lettered = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (db != null)
            {
                foreach (var d in db.All)
                {
                    if (!string.IsNullOrEmpty(d.Key) && !string.IsNullOrEmpty(d.Number) && !d.Number.Any(char.IsDigit) && !lettered.ContainsKey(d.Key))
                        lettered[d.Key] = d.Key;
                    if (string.IsNullOrEmpty(d.SetCode)) continue;
                    string code = d.SetCode.Trim().ToUpperInvariant();
                    string nc = NormalizeSetName(code);
                    if (nc.Length > 0 && !byCode.ContainsKey(nc)) byCode[nc] = code;
                    string nn = NormalizeSetName(d.PrintSetName);
                    if (nn.Length > 0 && !byName.ContainsKey(nn)) byName[nn] = code;
                    string nb = NormalizeSetName((d.PrintSetName ?? "").Replace("&", " "));
                    if (nb.Length > 0 && !byBareName.ContainsKey(nb)) byBareName[nb] = code;
                }
            }
            // Other names a set's folder is often given (Limitless calls the Wizards Black Star Promos "WotC Promos").
            foreach (var alias in ExtraSetNames)
                if (byCode.ContainsKey(NormalizeSetName(alias.Value)) && !byName.ContainsKey(alias.Key)) byName[alias.Key] = alias.Value;
            setsByCode = byCode;
            setsByName = byName;
            setsByBareName = byBareName;
            letterKeys = lettered;
            indexWarming = null; // one being built read the old sets
            index = null;
        }

        /// <summary>Extra folder names (already normalized) -> set code.</summary>
        private static readonly Dictionary<string, string> ExtraSetNames = new Dictionary<string, string>
        {
            { "wotcpromos", "WP" },
            { "wotcblackstarpromos", "WP" },
            { "wizardspromos", "WP" },
            { "wizardsblackstarpromos", "WP" },
            { "celebrationsclassiccollection", "CEL" },
            // v135: Limitless calls the HGSS promos "HeartGold & SoulSilver Promos".
            { "heartgoldandsoulsilverpromos", "HSP" },
            { "heartgoldsoulsilverpromos", "HSP" },
            { "hgsspromos", "HSP" },
            // v136: Limitless calls the DP promos "Diamond & Pearl Promos".
            { "diamondandpearlpromos", "DPP" },
            { "diamondpearlpromos", "DPP" },
            { "dppromos", "DPP" },
            // v137: Limitless calls the Nintendo Black Star Promos "Nintendo Promos".
            { "nintendopromos", "NP" },
            { "nintendoblackstarpromos", "NP" },
            // v152.3: your folder for the stamped prints (Trick or Trade 2026's Halloween stamp).
            { "leaguestamps", "TOT" },
            // v154: the Japanese sets' names without "(Japan, not yet in English)".
            { "stormemeralda", "M6" },
            { "exstartersetzoruaandzoroarkex", "MEZ" },
            { "zoruaandzoroarkexstarterset", "MEZ" },
            // v155: Japan's High Class Pack MEGA x MEGA Parade (M7A).
            { "megaxmegaparade", "M7A" },
            { "megamegaparade", "M7A" },
            // v156.1: the "What's Your Favorite? 30" stamped prints.
            { "whatsyourfavorite", "WYF" },
            { "whatsyourfavorite30", "WYF" },
            // v156.2: the World Championships 2026 stamped prints (v156.7: Play! Pokémon stamps use their set's code + PPS, e.g. MEGPPS).
            { "worldchampionships2026", "WC26" },
            { "worlds2026", "WC26" },
            { "worldchampionships2024", "WC24" },
            { "worlds2024", "WC24" },
            // v156.5: the Pokémon Day stamps.
            { "pokemonday2025", "PKD25" },
            { "pokemonday2026", "PKD26" }
        };

        /// <summary>Lower-case letters and digits only, accents dropped, "&amp;" read as "and": "Pokémon GO" -> "pokemongo".</summary>
        private static string NormalizeSetName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            var sb = new StringBuilder();
            foreach (char ch in name.Replace("&", " and ").Normalize(NormalizationForm.FormD))
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark) continue;
                if (char.IsLetterOrDigit(ch)) sb.Append(char.ToLowerInvariant(ch));
            }
            return sb.ToString();
        }

        private static bool KnowsSets { get { return setsByCode != null && setsByCode.Count > 0; } }

        /// <summary>A set code as written in a file or folder name: a code of the card pool (any token with a letter before the pool is known).</summary>
        private static string KnownSetCode(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            if (!KnowsSets) return token.Any(char.IsLetter) && token.All(char.IsLetterOrDigit) ? token.ToUpperInvariant() : null;
            string code;
            return setsByCode.TryGetValue(NormalizeSetName(token), out code) ? code : null;
        }

        /// <summary>
        /// The set an image folder stands for, tried in this order: its set code ("BS", "bs"), a code in brackets ("Base Set
        /// (BS)"), the set's name ("Base Set", "base-set", "Pokemon GO"), the name after a leading code or word ("BS - Base
        /// Set", "XY Evolutions" is Evolutions), then a leading code on its own ("BS Holos"). Null if none.
        /// </summary>
        public static string SetCodeForFolder(string folderName)
        {
            if (string.IsNullOrEmpty(folderName)) return null;
            string name = folderName.Trim();
            string code = KnownSetCode(name);
            if (code != null && (KnowsSets || name.Length <= 5)) return code;
            if (!KnowsSets) return null;
            var m = Regex.Match(name, @"[\(\[]\s*([A-Za-z0-9]+)\s*[\)\]]");
            if (m.Success && (code = KnownSetCode(m.Groups[1].Value)) != null) return code;
            if (setsByName.TryGetValue(NormalizeSetName(name), out code)) return code;
            var words = name.Split(new[] { ' ', '-', '_', '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length >= 2)
            {
                string rest = string.Join(" ", words.Skip(1).ToArray());
                if (setsByName.TryGetValue(NormalizeSetName(Regex.Replace(rest, @"[\(\[].*?[\)\]]", "")), out code)) return code;
                if ((code = KnownSetCode(words[0])) != null) return code;
            }
            return null;
        }

        /// <summary>A card number as written in a file name: digits, maybe with a short letter prefix or a letter after ("4", "004", "H1", "TG01", "4a").</summary>
        private static bool IsCardNumber(string token)
        {
            return token != null && Regex.IsMatch(token, @"^[A-Za-z]{0,3}[0-9]+[A-Za-z]?$");
        }

        /// <summary>
        /// v168.1: a card "number" written as a word, the way a file name has to spell a sign: "question", "question mark",
        /// "questionmark" are ?, "exclamation", "exclamation mark" are ! (Windows doesn't allow ? in a file name). Else the token.
        /// </summary>
        private static string SpelledNumber(string token)
        {
            string t = Regex.Replace(token ?? "", @"[\s_\-.]+", "").ToLowerInvariant();
            if (t == "question" || t == "questionmark" || t == "qmark") return "?";
            if (t == "exclamation" || t == "exclamationmark" || t == "exclamationpoint") return "!";
            return (token ?? "").Trim();
        }

        /// <summary>v168.1: the pool's key for a card of this set numbered with a letter or sign ("V", "!", "question mark"), or null.</summary>
        private static string LetterNumberKey(string set, string token)
        {
            if (letterKeys == null || string.IsNullOrEmpty(set) || string.IsNullOrEmpty(token)) return null;
            string number = SpelledNumber(token);
            if (number.Length == 0 || number.Any(char.IsDigit)) return null;
            string key;
            return letterKeys.TryGetValue(set.Trim().ToUpperInvariant() + "-" + number.ToUpperInvariant(), out key) ? key : null;
        }

        /// <summary>
        /// The card key for an image file. A name that starts with a set code gives its own set ("BS_4", "TWM 130 Dragapult
        /// ex"); otherwise the card number in the name ("4", "004 Charizard", "4_hires") goes with the set of the folder it's
        /// in, checking the folders from the innermost one out. Null if neither works.
        /// </summary>
        public static string KeyFor(string fileNameWithoutExtension, IList<string> foldersInnermostFirst)
        {
            // A name pkmncards.com gave the file says the set itself, whatever folder it's in (v69).
            var pk = KeyFromPkmncardsName(fileNameWithoutExtension);
            if (pk != null) return pk;
            var parts = (fileNameWithoutExtension ?? "").Split(new[] { '_', '-', ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return null;
            bool startsWithCode = parts.Length >= 2 && !parts[0].All(char.IsDigit) && KnownSetCode(parts[0]) != null;
            if (startsWithCode)
            {
                var own = KeyFromFileName(fileNameWithoutExtension);
                if (own != null) return own;
            }
            string folderSet = null;
            if (foldersInnermostFirst != null)
                foreach (var folder in foldersInnermostFirst)
                    if ((folderSet = SetCodeForFolder(folder)) != null) break;
            if (folderSet != null)
            {
                var number = parts.FirstOrDefault(IsCardNumber);
                if (number != null) return CardDatabase.MakeKey(folderSet, WithoutPromoPrefix(folderSet, number));
                // v168.1: a card numbered with a letter or sign: "V.png", "Unown V.png", "!.png", "question mark.png".
                string lettered = LetterNumberKey(folderSet, fileNameWithoutExtension);
                if (lettered != null) return lettered;
                for (int i = parts.Length - 1; i >= 0 && lettered == null; i--)
                {
                    lettered = LetterNumberKey(folderSet, parts[i]);
                    if (lettered == null && i + 1 < parts.Length) lettered = LetterNumberKey(folderSet, parts[i] + parts[i + 1]);
                }
                if (lettered != null) return lettered;
            }
            // A name that starts with a code the pool doesn't have (an image for a card not imported yet): keep the old
            // reading, so it's listed as not matching any card rather than skipped.
            return parts[0].All(char.IsDigit) ? null : KeyFromFileName(fileNameWithoutExtension);
        }

        /// <summary>The folders between the image folder and the file, innermost first.</summary>
        private static List<string> FoldersOf(string path, string root)
        {
            var list = new List<string>();
            string rootFull;
            try { rootFull = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); }
            catch (Exception) { return list; }
            var dir = Path.GetDirectoryName(path);
            for (int guard = 0; guard < 32 && !string.IsNullOrEmpty(dir); guard++)
            {
                string full;
                try { full = Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); }
                catch (Exception) { break; }
                if (full.Length <= rootFull.Length || string.Equals(full, rootFull, StringComparison.OrdinalIgnoreCase)) break;
                list.Add(Path.GetFileName(full));
                dir = Path.GetDirectoryName(full);
            }
            return list;
        }

        /// <summary>"TWM_130", "twm-130", "TWM 130 Dragapult ex", "TWM_130_R_EN_LG", "MEE_013" all give "TWM-130" style keys.</summary>
        /// <summary>v153: language codes a file name may have between the set code and the number (PokéBeach: "MEP_EN_111").</summary>
        private static readonly HashSet<string> LanguageTags = new HashSet<string>
        {
            "EN", "JP", "JA", "FR", "DE", "IT", "ES", "PT", "KO", "KR", "ZH", "CN", "TW", "NL", "PL", "RU", "TH", "ID"
        };

        public static string KeyFromFileName(string fileNameWithoutExtension)
        {
            var parts = (fileNameWithoutExtension ?? "").Split(new[] { '_', '-', ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return null;
            // v153: PokéBeach names its images set, language, number: "MEP_EN_111", "DLR_EN_084".
            if (parts.Length >= 3 && LanguageTags.Contains(parts[1].ToUpperInvariant()) && parts[2].Any(char.IsDigit))
                return CardDatabase.MakeKey(parts[0], parts[2]);
            // v168.1: a sign spelled out after the set code ("UF_question_mark", "UF exclamation").
            if (!parts[1].Any(char.IsDigit))
            {
                string spelled = SpelledNumber(parts.Length >= 3 ? parts[1] + parts[2] : parts[1]);
                if (spelled.Length == 1 && !char.IsLetter(spelled[0])) return CardDatabase.MakeKey(parts[0], spelled);
                spelled = SpelledNumber(parts[1]);
                if (spelled.Length == 1 && !char.IsLetter(spelled[0])) return CardDatabase.MakeKey(parts[0], spelled);
            }
            // Card numbers have digits, except a few Energy printings numbered by letter (SUM_P, BRS_R...).
            if (!parts[1].Any(char.IsDigit) && parts[1].Length > 2) return null;
            return CardDatabase.MakeKey(parts[0], parts[1]);
        }

        // ------------------------------------------------------------------ pkmncards.com names (v69)

        /// <summary>Where a set code in a pkmncards.com file name points: the pool's set, and how that file writes its card numbers.</summary>
        private sealed class PkSet
        {
            public readonly string Set;
            /// <summary>Put in front of the number: the 30th Celebration's Classic Collection is "me5-5c_en_027" for CC27.</summary>
            public readonly string NumberPrefix;
            /// <summary>Taken off the number: "SWSH289" in Promo_SWSH is SP 289, "SV001" in Promo_SV is SVP 1.</summary>
            public readonly string StripPrefix;
            /// <summary>
            /// The set an SV-numbered card listed under this set belongs to: the site files the Shiny Vault cards under the set
            /// their art first came from ("en_US-SM1-SV44-oranguru" is Hidden Fates SV44).
            /// </summary>
            public readonly string ShinyVault;

            public PkSet(string set, string numberPrefix, string stripPrefix, string shinyVault)
            {
                Set = set;
                NumberPrefix = numberPrefix;
                StripPrefix = stripPrefix;
                ShinyVault = shinyVault;
            }

            public PkSet(string set) : this(set, null, null, null) { }
        }

        private static PkSet Sv(string set, string shinyVault) { return new PkSet(set, null, null, shinyVault); }

        /// <summary>The site's newer names, TPCi's own: "sv6_en_001_std", "sv1_en_001-1", "swshbsp_en_299_std", "me5-5c_en_027_std".</summary>
        private static readonly Dictionary<string, PkSet> PkTpciSets = new Dictionary<string, PkSet>(StringComparer.OrdinalIgnoreCase)
        {
            { "sv1", new PkSet("SVI") }, { "sv2", new PkSet("PAL") }, { "sv3", new PkSet("OBF") }, { "sv3-5", new PkSet("MEW") },
            { "sv4", new PkSet("PAR") }, { "sv4-5", new PkSet("PAF") }, { "sv5", new PkSet("TEF") }, { "sv6", new PkSet("TWM") },
            { "sv6-5", new PkSet("SFA") }, { "sv7", new PkSet("SCR") }, { "sv8", new PkSet("SSP") }, { "sv8-5", new PkSet("PRE") },
            { "sv9", new PkSet("JTG") }, { "sv10", new PkSet("DRI") }, { "zsv10-5", new PkSet("BLK") }, { "rsv10-5", new PkSet("WHT") },
            { "me1", new PkSet("MEG") }, { "me2", new PkSet("PFL") }, { "me2-5", new PkSet("ASC") }, { "me3", new PkSet("POR") },
            { "me4", new PkSet("CRI") }, { "me5", new PkSet("PBL") }, { "me5-5", new PkSet("30C") }, { "me5-5c", new PkSet("30C", "CC", null, null) },
            { "mebsp", new PkSet("MEP") }, { "mee", new PkSet("MEE") }, { "sve", new PkSet("SVE") }, { "svbsp", new PkSet("SVP") },
            { "swshbsp", new PkSet("SP") }
        };

        /// <summary>The site's "en_US-SET-NUMBER-name" names (Black &amp; White to Sword &amp; Shield, and the early promos of each era).</summary>
        private static readonly Dictionary<string, PkSet> PkEnUsSets = new Dictionary<string, PkSet>(StringComparer.OrdinalIgnoreCase)
        {
            // Black & White
            { "BW1", new PkSet("BLW") }, { "BW2", new PkSet("EPO") }, { "BW3", new PkSet("NVI") }, { "BW4", new PkSet("NXD") },
            { "BW5", new PkSet("DEX") }, { "BW6", new PkSet("DRX") }, { "BW7", new PkSet("BCR") }, { "BW8", new PkSet("PLS") },
            { "BW9", new PkSet("PLF") }, { "BW10", new PkSet("PLB") }, { "BW11", new PkSet("LTR") }, { "DV", new PkSet("DRV") },
            // XY
            { "XY0", new PkSet("KSS") }, { "XY1", new PkSet("XY") }, { "XY2", new PkSet("FLF") }, { "XY3", new PkSet("FFI") },
            { "XY4", new PkSet("PHF") }, { "XY5", new PkSet("PRC") }, { "XY6", new PkSet("ROS") }, { "XY7", new PkSet("AOR") },
            { "XY8", new PkSet("BKT") }, { "XY9", new PkSet("BKP") }, { "XY10", new PkSet("FCO") }, { "XY11", new PkSet("STS") },
            { "XY12", new PkSet("EVO") }, { "TwentiethAnn", new PkSet("GEN") },
            // Sun & Moon: the Hidden Fates Shiny Vault cards are filed under the set their art came from
            { "SM1", Sv("SUM", "HIF") }, { "SM2", Sv("GRI", "HIF") }, { "SM3", Sv("BUS", "HIF") }, { "SL", Sv("SLG", "HIF") },
            { "SM4", Sv("CIN", "HIF") }, { "SM5", Sv("UPR", "HIF") }, { "SM6", Sv("FLI", "HIF") }, { "SM7", Sv("CES", "HIF") },
            { "SM75", Sv("DRM", "HIF") }, { "SM8", Sv("LOT", "HIF") }, { "SM9", Sv("TEU", "HIF") }, { "SM10", Sv("UNB", "HIF") },
            { "SM11", Sv("UNM", "HIF") }, { "HF", new PkSet("HIF") }, { "SM12", Sv("CEC", "HIF") },
            { "GUM", new PkSet("DET") }, // Detective Pikachu (v79)
            // Sword & Shield: the same for the Shining Fates Shiny Vault
            { "SWSH1", Sv("SSH", "SHF") }, { "SWSH2", Sv("RCL", "SHF") }, { "SWSH3", Sv("DAA", "SHF") }, { "CP", Sv("CPA", "SHF") },
            { "SWSH4", Sv("VIV", "SHF") }, { "SF", new PkSet("SHF") }, { "SWSH5", new PkSet("BST") }, { "SWSH6", new PkSet("CRE") },
            { "SWSH7", new PkSet("EVS") }, { "SWSH8", new PkSet("FST") }, { "Ann25th", new PkSet("CEL") }, { "SWSH9", new PkSet("BRS") },
            { "SWSH10", new PkSet("ASR") }, { "PGO", new PkSet("PGO") }, { "SWSH11", new PkSet("LOR") }, { "SWSH12", new PkSet("SIT") },
            { "CZ", new PkSet("CRZ") },
            // Promos: the site writes the set's prefix into the number
            { "Promo_SWSH", new PkSet("SP", null, "SWSH", null) }, { "Promo_SV", new PkSet("SVP", null, "SV", null) },
            // v93: the Sun & Moon promos ("en_US-Promo_SM-SM108-ashs_pikachu" is SMP 108), and the XY and Black & White ones.
            { "Promo_SM", new PkSet("SMP", null, "SM", null) }, { "Promo_XY", new PkSet("XYP", null, "XY", null) },
            { "Promo_BW", new PkSet("BWP", null, "BW", null) },
            // v135: the HeartGold & SoulSilver promos ("en_US-Promo_HGSS-HGSS01-ho_oh" is HSP 1).
            { "Promo_HGSS", new PkSet("HSP", null, "HGSS", null) },
            // v136: the Diamond & Pearl promos ("en_US-Promo_DP-DP01-turtwig" is DPP 1).
            { "Promo_DP", new PkSet("DPP", null, "DP", null) }
        };

        /// <summary>
        /// The prefix a promo set writes in front of its numbers, which the pool leaves off: "SM108" in the SM promos' folder is
        /// SMP 108 (v93).
        /// </summary>
        private static readonly Dictionary<string, string> PromoNumberPrefixes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "SMP", "SM" }, { "SP", "SWSH" }, { "SVP", "SV" }, { "XYP", "XY" }, { "BWP", "BW" }, { "HSP", "HGSS" }, { "DPP", "DP" }
        };

        private static string WithoutPromoPrefix(string set, string number)
        {
            string prefix;
            if (set != null && number != null && PromoNumberPrefixes.TryGetValue(set, out prefix) && number.Length > prefix.Length
                && number.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && char.IsDigit(number[prefix.Length]))
                return number.Substring(prefix.Length);
            return number;
        }

        /// <summary>
        /// Celebrations' Classic Collection: the site numbers those files by the card's original number (four of them are "015"),
        /// so the name after it tells them apart.
        /// </summary>
        private static readonly Dictionary<string, int> PkCelebrationsClassic = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "blastoise", 1 }, { "charizard", 2 }, { "venusaur", 3 }, { "imposter_professor_oak", 4 }, { "dark_gyarados", 5 },
            { "here_comes_team_rocket", 6 }, { "rockets_zapdos", 7 }, { "_s_pikachu", 8 }, { "cleffa", 9 }, { "shining_magikarp", 10 },
            { "team_magmas_groudon", 11 }, { "rockets_admin", 12 }, { "mew_ex", 13 }, { "gardevoir_ex", 14 }, { "umbreon_star", 15 },
            { "claydol", 16 }, { "luxray_gl", 17 }, { "garchomp_c", 18 }, { "donphan", 19 }, { "reshiram", 20 }, { "zekrom", 21 },
            { "mewtwo_ex", 22 }, { "xerneas_ex", 23 }, { "m_rayquaza_ex", 24 }, { "tapu_lele_gx", 25 }
        };

        /// <summary>The Sun &amp; Moon basic Energy ("SM_Energy" 001-009, and 010-018 for Team Up's) in the order the pool letters them.</summary>
        private static readonly string[] PkEnergyLetters = { "G", "R", "W", "L", "P", "F", "D", "M", "Y" };

        /// <summary>The pool's basic Fairy Energy, the only one there is: every Fairy Energy image is used for it (v69).</summary>
        private const string FairyEnergyKey = "SUM-Y";

        /// <summary>The pool's main basic Energy cards (Mega Evolution Energy 9-16) in the site's Energy order, then Fairy.</summary>
        private static readonly string[] PkMainEnergyKeys = { "MEE-9", "MEE-10", "MEE-11", "MEE-12", "MEE-13", "MEE-14", "MEE-15", "MEE-16", FairyEnergyKey };

        /// <summary>
        /// For a basic Energy image of a printing the pool doesn't have (pkmncards.com's "en_US-XY_Energy-001-grass_energy" and
        /// "en_US-SWSH_Energy-..."): the pool's card of that type, used only if nothing else gives that card an image (v69).
        /// Null for any other name.
        /// </summary>
        public static string FallbackKeyFromPkmncardsName(string fileNameWithoutExtension)
        {
            if (string.IsNullOrEmpty(fileNameWithoutExtension)) return null;
            var m = PkEnUsName.Match(PkSizeSuffix.Replace(fileNameWithoutExtension.Trim(), ""));
            if (!m.Success) return null;
            string code = m.Groups["code"].Value;
            if (!string.Equals(code, "XY_Energy", StringComparison.OrdinalIgnoreCase) && !string.Equals(code, "SWSH_Energy", StringComparison.OrdinalIgnoreCase))
                return null;
            int n;
            if (!int.TryParse(m.Groups["num"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out n) || n < 1 || n > PkMainEnergyKeys.Length) return null;
            return PkMainEnergyKeys[n - 1];
        }

        /// <summary>The size WordPress adds to a thumbnail's name (-312x436), and -scaled on a big original.</summary>
        private static readonly Regex PkSizeSuffix = new Regex(@"(-\d+x\d+|-scaled)+$", RegexOptions.IgnoreCase);
        private static readonly Regex PkTpciName = new Regex(@"^(?<code>[a-z]+\d*(?:-\d+)?[a-z]?)_en_(?<num>\d{1,3})(?:_std)?(?:-\d+)*$", RegexOptions.IgnoreCase);
        // English only: the site also has a few other-language scans ("pt_BR-SM9-152a-...") that shouldn't take a card's image.
        private static readonly Regex PkEnUsName = new Regex(@"^en_us-(?<code>[a-z0-9_]+?)-(?<num>[a-z]{0,4}\d{1,3}[a-z]?)(?:[-_](?<rest>.*))?$", RegexOptions.IgnoreCase);

        /// <summary>
        /// The card key for an image saved from pkmncards.com under the name the site gave it, or null when the name isn't one of
        /// the site's. The site has used three styles: TPCi's own "sv6_en_001_std" (Scarlet &amp; Violet on, and the later promos
        /// and Energy), "en_US-SWSH9-001-exeggcute" (Black &amp; White to Sword &amp; Shield, and the earlier promos) and
        /// "charizard-base-set-bs-4" (everything older, the set's name written out). A thumbnail's size (-312x436) and
        /// WordPress's "-1" for a second upload are ignored.
        /// </summary>
        public static string KeyFromPkmncardsName(string fileNameWithoutExtension)
        {
            if (string.IsNullOrEmpty(fileNameWithoutExtension)) return null;
            string name = PkSizeSuffix.Replace(fileNameWithoutExtension.Trim(), "");
            var m = PkTpciName.Match(name);
            if (m.Success)
            {
                PkSet ps;
                if (!PkTpciSets.TryGetValue(m.Groups["code"].Value, out ps)) return null;
                return CardDatabase.MakeKey(ps.Set, (ps.NumberPrefix ?? "") + m.Groups["num"].Value);
            }
            m = PkEnUsName.Match(name);
            if (m.Success) return KeyFromPkmncardsEnUs(m.Groups["code"].Value, m.Groups["num"].Value, m.Groups["rest"].Value);
            return KeyFromPkmncardsSlug(name);
        }

        private static string KeyFromPkmncardsEnUs(string code, string number, string rest)
        {
            // The name after the number, without WordPress's "-1" and the site's own "-ya" / "-yaa" tags.
            string slug = Regex.Replace(rest ?? "", @"(-\d+|-ya+)+$", "", RegexOptions.IgnoreCase);
            int n;
            if (string.Equals(code, "SM_Energy", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(number, NumberStyles.None, CultureInfo.InvariantCulture, out n)) return null;
                // Fairy Energy: the pool has only Sun & Moon's (SUM Y), so Team Up's goes to it too.
                if (n == PkEnergyLetters.Length || n == 2 * PkEnergyLetters.Length) return FairyEnergyKey;
                if (n >= 1 && n <= PkEnergyLetters.Length) return CardDatabase.MakeKey("SUM", PkEnergyLetters[n - 1]);
                if (n > PkEnergyLetters.Length && n <= 2 * PkEnergyLetters.Length) return CardDatabase.MakeKey("TEU", PkEnergyLetters[n - 1 - PkEnergyLetters.Length]);
                return null;
            }
            if (string.Equals(code, "Ann25thR", StringComparison.OrdinalIgnoreCase))
                return PkCelebrationsClassic.TryGetValue(slug, out n) ? CardDatabase.MakeKey("CEL", "CC" + n) : null;
            PkSet ps;
            if (!PkEnUsSets.TryGetValue(code, out ps)) return null;
            if (ps.StripPrefix != null && number.Length > ps.StripPrefix.Length && number.StartsWith(ps.StripPrefix, StringComparison.OrdinalIgnoreCase))
                number = number.Substring(ps.StripPrefix.Length);
            string set = ps.Set;
            if (ps.ShinyVault != null && number.Length > 2 && number.StartsWith("SV", StringComparison.OrdinalIgnoreCase) && char.IsDigit(number[2]))
                set = ps.ShinyVault;
            return CardDatabase.MakeKey(set, (ps.NumberPrefix ?? "") + number);
        }

        /// <summary>
        /// The site's older names, with the set's name written out: "charizard-base-set-bs-4", "dialga-diamond-pearl-dp-1",
        /// "pikachu-wizards-black-star-promos-1", "azumarill-team-rocket-returns-rr-1". The set's name decides; the short code
        /// after it (when there is one) isn't always the pool's ("rr" there is Team Rocket Returns, not Rising Rivals).
        /// </summary>
        private static string KeyFromPkmncardsSlug(string name)
        {
            if (!KnowsSets || name.IndexOf(' ') >= 0 || name.IndexOf('_') >= 0) return null;
            // v168.1: how the site writes the Unown signs: "unown-unseen-forces-uf-question-mark" is ?, and the ! card's file is
            // "unown-unseen-forces-uf-" (the sign dropped; its thumbnail "...-uf--312x429").
            if (name.EndsWith("-question-mark", StringComparison.OrdinalIgnoreCase)) name = name.Substring(0, name.Length - "question-mark".Length) + "?";
            else if (name.EndsWith("-exclamation-mark", StringComparison.OrdinalIgnoreCase)) name = name.Substring(0, name.Length - "exclamation-mark".Length) + "!";
            else if (name.EndsWith("-", StringComparison.Ordinal)) name = name.TrimEnd('-') + "-!";
            var tokens = name.Split('-');
            string key = SlugKey(tokens);
            // WordPress's "-1" for a second upload of the same name.
            if (key == null && tokens.Length > 3 && tokens[tokens.Length - 1].All(char.IsDigit))
                key = SlugKey(tokens.Take(tokens.Length - 1).ToArray());
            return key;
        }

        private static string SlugKey(string[] tokens)
        {
            int n = tokens.Length;
            if (n < 3) return null;
            string number = tokens[n - 1];
            bool digits = IsCardNumber(number) && number.Any(char.IsDigit);
            // v168.1: or a letter or sign, if the set has a card numbered that way (Unown UF X, !, ?).
            if (!digits && (number.Length == 0 || number.Length > 2 || letterKeys == null || letterKeys.Count == 0)) return null;
            // The set's name ends right before the number, or before a code token ("bs" in "base-set-bs-4"), and starts
            // somewhere after the card's name: try every split.
            for (int end = n - 1; end >= n - 2; end--)
                for (int start = 1; start < end; start++)
                {
                    string code = SetCodeForSlug(string.Join(" ", tokens, start, end - start));
                    if (code == null) continue;
                    if (digits) return CardDatabase.MakeKey(code, number);
                    string lettered = LetterNumberKey(code, number);
                    if (lettered != null) return lettered;
                }
            return null;
        }

        /// <summary>A set's name as a web slug writes it ("diamond pearl", "pokemon go", "base set 2") -> set code, or null.</summary>
        private static string SetCodeForSlug(string text)
        {
            string norm = NormalizeSetName(text);
            if (norm.Length == 0) return null;
            string code;
            if (setsByName.TryGetValue(norm, out code)) return code;
            return setsByBareName != null && setsByBareName.TryGetValue(norm, out code) ? code : null;
        }

        /// <summary>The image index and the files it skipped, built together (v134.6: possibly on a background thread).</summary>
        private sealed class ImageIndexBuild
        {
            public readonly Dictionary<string, string> Index = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            public readonly List<string> Skipped = new List<string>();
            /// <summary>Images used only for a card nothing else gives an image (another printing's basic Energy, v69).</summary>
            public readonly List<KeyValuePair<string, string>> Fallbacks = new List<KeyValuePair<string, string>>();
            /// <summary>v153: where AVIF and WebP images are converted to PNG (null where they can't be: phones and tablets).</summary>
            public string ConvertCache;
            /// <summary>v153: AVIF and WebP images to convert: card key -> the original file.</summary>
            public readonly List<KeyValuePair<string, string>> ToConvert = new List<KeyValuePair<string, string>>();
            /// <summary>v153.1: the names of the PNGs already in the convert folder.</summary>
            public readonly HashSet<string> Converted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// v153.1: an image Unity couldn't read that is really AVIF or WebP under a .jpg or .png name: converted to PNG on a
        /// background thread (like a .avif file), and the card shows its placeholder until it's done. False when it isn't one,
        /// can't be converted here (phones and tablets) or converting it already failed.
        /// </summary>
        private static bool ConvertRelabeled(CardDef def, string path, byte[] bytes)
        {
            if (path.StartsWith("jar:", StringComparison.OrdinalIgnoreCase) || ImageConverter.RealFormat(bytes) == null) return false;
            string cache = ConvertCacheFolder();
            if (cache == null) return false;
            List<string> waiting;
            if (relabeling.TryGetValue(path, out waiting))
            {
                if (waiting == null) return false;
                if (!waiting.Contains(def.Key)) waiting.Add(def.Key);
                return true;
            }
            string png = ImageConverter.CachedPngPath(cache, path);
            relabeling[path] = new List<string> { def.Key };
            Debug.Log("[PokemonTCG] " + Path.GetFileName(path) + " is an AVIF or WebP picture with a " + Path.GetExtension(path)
                      + " name: converting it to PNG.");
            try
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    var problems = new List<string>();
                    var ready = ImageConverter.ConvertAll(new[] { new KeyValuePair<string, string>(path, png) }, problems);
                    relabeled.Enqueue(new RelabeledImage
                    {
                        Source = path,
                        Png = ready.Count > 0 ? png : null,
                        Problem = problems.Count > 0 ? problems[0] : null
                    });
                });
            }
            catch (Exception)
            {
                relabeling[path] = null;
                return false;
            }
            return true;
        }

        /// <summary>v153.1: uses the PNGs made from relabeled AVIF/WebP pictures; the cards waiting for them load them next time they're drawn.</summary>
        private static void FinishRelabeled()
        {
            RelabeledImage done;
            while (relabeled.TryDequeue(out done))
            {
                List<string> waiting;
                relabeling.TryGetValue(done.Source, out waiting);
                if (done.Png == null)
                {
                    relabeling[done.Source] = null;
                    FailedFiles.Add(Path.GetFileName(done.Source));
                    Debug.LogWarning("[PokemonTCG] " + (done.Problem ?? Path.GetFileName(done.Source) + " couldn't be converted to PNG."));
                    continue;
                }
                relabeling.Remove(done.Source);
                if (index != null)
                {
                    string source = done.Source;
                    var keys = index.Where(kv => string.Equals(kv.Value, source, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Key).ToList();
                    foreach (var key in keys) index[key] = done.Png;
                }
                if (waiting != null)
                    foreach (var key in waiting) faces.Remove(key);
            }
        }

        /// <summary>
        /// v153.1: the PNG made earlier from this .jpg/.png file (an AVIF or WebP picture with that name), if it's still up to
        /// date, so the next start uses it at once. Null for every other file.
        /// </summary>
        private static string ConvertedCopy(ImageIndexBuild b, string path)
        {
            if (b.ConvertCache == null || b.Converted.Count == 0 || path.StartsWith("jar:", StringComparison.OrdinalIgnoreCase)) return null;
            string png = ImageConverter.CachedPngPath(b.ConvertCache, path);
            return b.Converted.Contains(Path.GetFileName(png)) && ImageConverter.Fresh(path, png) ? png : null;
        }

        /// <summary>v153: the folder the PNG copies of AVIF and WebP card images go in (null on phones and tablets, which can't convert).</summary>
        private static string ConvertCacheFolder()
        {
            return Application.isMobilePlatform ? null : Path.Combine(Application.persistentDataPath, "ConvertedCardImages");
        }

        /// <summary>v134.6: the index being built in the background since the cards loaded (null when none is).</summary>
        private static System.Threading.Tasks.Task<ImageIndexBuild> indexWarming;

        /// <summary>
        /// v134.6: starts building the image index on a background thread, so the first screen that shows a card image doesn't
        /// stop while every file in the image folder is listed and read by name. Called once the card pool's sets are known.
        /// </summary>
        public static void WarmIndexInBackground()
        {
            if (index != null || indexWarming != null || ImagesInsidePackage) return;
            string folder = ImageFolder;
            string device = Application.isMobilePlatform ? DeviceImageFolder : null;
            string cache = ConvertCacheFolder();
            try { indexWarming = System.Threading.Tasks.Task.Run(() => BuildIndex(folder, null, device, cache)); }
            catch (Exception) { indexWarming = null; }
        }

        private static Dictionary<string, string> Index()
        {
            if (index != null) return index;
            ImageIndexBuild built = null;
            var warming = indexWarming;
            indexWarming = null;
            if (warming != null)
            {
                try { built = warming.Result; } // waits if it isn't done yet: never slower than building it here
                catch (Exception) { built = null; }
            }
            if (built == null)
            {
                // v118: on Android the built-in images are listed in the image list (StreamingAssets can't be browsed there).
                built = BuildIndex(ImageFolder, ImagesInsidePackage ? PackagedImageList() : null,
                                   Application.isMobilePlatform ? DeviceImageFolder : null, ConvertCacheFolder());
            }
            SkippedFiles.Clear();
            SkippedFiles.AddRange(built.Skipped);
            index = built.Index;
            return index;
        }

        /// <summary>
        /// Lists the image files by card key: the image folder (or the Android app's image list), then a phone or tablet's own
        /// image folder (v118). Touches no Unity API and no shared state, so it can run on any thread.
        /// </summary>
        private static ImageIndexBuild BuildIndex(string folder, List<string> packaged, string deviceFolder, string convertCache)
        {
            var b = new ImageIndexBuild();
            b.ConvertCache = convertCache;
            if (convertCache != null)
            {
                try
                {
                    if (Directory.Exists(convertCache))
                        foreach (var made in Directory.GetFiles(convertCache, "*.png")) b.Converted.Add(Path.GetFileName(made));
                }
                catch (Exception) { }
            }
            if (packaged != null)
            {
                // The image list's paths are relative to the image folder.
                foreach (var rel in packaged)
                    AddImage(b, folder + "/" + rel, rel.Substring(rel.LastIndexOf('/') + 1), FoldersOfRelative(rel));
            }
            else AddImageFolder(b, folder);
            if (deviceFolder != null) AddImageFolder(b, deviceFolder);
            // v153: AVIF and WebP images (PokéBeach's are AVIF) become PNG copies in the cache folder, made once.
            if (b.ConvertCache != null && b.ToConvert.Count > 0)
            {
                var pairs = new List<KeyValuePair<string, string>>();
                var keyOf = new Dictionary<string, string>();
                foreach (var kv in b.ToConvert)
                {
                    if (b.Index.ContainsKey(kv.Key) || keyOf.ContainsKey(kv.Value)) continue;
                    keyOf[kv.Value] = kv.Key;
                    pairs.Add(new KeyValuePair<string, string>(kv.Value, ImageConverter.CachedPngPath(b.ConvertCache, kv.Value)));
                }
                foreach (var done in ImageConverter.ConvertAll(pairs, b.Skipped))
                {
                    string key = keyOf[done.Key];
                    if (!b.Index.ContainsKey(key)) b.Index[key] = done.Value;
                }
            }
            foreach (var kv in b.Fallbacks)
                if (!b.Index.ContainsKey(kv.Key)) b.Index[kv.Key] = kv.Value;
            return b;
        }

        private static void AddImageFolder(ImageIndexBuild b, string dir)
        {
            if (!Directory.Exists(dir)) return;
            string[] files;
            try { files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories); }
            catch (IOException) { return; }
            catch (UnauthorizedAccessException) { return; }
            foreach (var path in files) AddImage(b, path, Path.GetFileName(path), FoldersOf(path, dir));
        }

        /// <summary>Adds one image file to the index (path: a file path, or a jar: URL on Android).</summary>
        private static void AddImage(ImageIndexBuild b, string path, string fileName, List<string> folders)
        {
            string ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (ext == ".meta" || fileName.StartsWith(".") || fileName == ImageListFileName) return;
            bool convert = ImageConverter.IsConvertible(ext);
            if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && !convert)
            {
                b.Skipped.Add(fileName + " (only PNG, JPG, AVIF and WebP images can be loaded)");
                return;
            }
            if (convert)
            {
                // A PNG or JPG of the same name next to it (the Android build makes one) is used instead.
                if (!path.StartsWith("jar:", StringComparison.OrdinalIgnoreCase) && HasPngOrJpgTwin(path)) return;
                if (b.ConvertCache == null)
                {
                    b.Skipped.Add(fileName + " (AVIF and WebP images are converted to PNG on a computer: open the game there once, or re-save it as PNG)");
                    return;
                }
            }
            string bare = Path.GetFileNameWithoutExtension(fileName);
            string fallback = convert ? null : FallbackKeyFromPkmncardsName(bare);
            if (fallback != null)
            {
                b.Fallbacks.Add(new KeyValuePair<string, string>(fallback, path));
                return;
            }
            string key = KeyFor(bare, folders);
            if (key == null)
            {
                b.Skipped.Add(fileName + " (the name should start with the set code and number, e.g. TWM_130, be"
                              + " just the number in a folder named after the set, e.g. Base Set/4.png, or be the name pkmncards.com gave it)");
                return;
            }
            if (convert)
            {
                b.ToConvert.Add(new KeyValuePair<string, string>(key, path));
                return;
            }
            if (!b.Index.ContainsKey(key)) b.Index[key] = ConvertedCopy(b, path) ?? path;
        }

        /// <summary>v153: a PNG or JPG with the same name sits next to this AVIF/WebP file.</summary>
        private static bool HasPngOrJpgTwin(string path)
        {
            try
            {
                string stem = Path.Combine(Path.GetDirectoryName(path) ?? "", Path.GetFileNameWithoutExtension(path));
                return File.Exists(stem + ".png") || File.Exists(stem + ".jpg") || File.Exists(stem + ".jpeg");
            }
            catch (Exception) { return false; }
        }

        private static Texture2D LoadImage(CardDef def)
        {
            string path;
            // An alternate printing without its own image shows the main printing's art.
            if (!Index().TryGetValue(def.Key, out path) && !(def.IsAlternatePrint && Index().TryGetValue(def.BaseKey, out path))) return null;
            byte[] bytes;
            if (path.StartsWith("jar:", StringComparison.OrdinalIgnoreCase))
            {
                // v118: an image inside the Android app (StreamingAssets is a jar: URL there).
                bytes = ReadPackagedFile(path);
                if (bytes == null) return null;
            }
            else
            {
                try { bytes = File.ReadAllBytes(path); }
                catch (IOException) { return null; }
                catch (UnauthorizedAccessException) { return null; }
            }
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, true);
            if (!tex.LoadImage(bytes))
            {
                UnityEngine.Object.Destroy(tex);
                if (ConvertRelabeled(def, path, bytes)) return null;
                FailedFiles.Add(Path.GetFileName(path));
                Debug.LogWarning("[PokemonTCG] Couldn't read card image " + path + ". It may be a WebP or other format saved with a .png/.jpg name; re-save it as PNG or JPG.");
                return null;
            }
            tex = Shrink(tex);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.anisoLevel = 4;
            tex.name = def.Key;
            return tex;
        }

        /// <summary>Scales a big image down to MaxImageHeight (area average), then compresses it and frees the CPU copy.</summary>
        private static Texture2D Shrink(Texture2D src)
        {
            if (src.height <= MaxImageHeight)
            {
                src.Apply(false, true);
                return src;
            }
            int sw = src.width, sh = src.height;
            int dh = MaxImageHeight;
            int dw = Mathf.Max(4, Mathf.RoundToInt(sw * (float)dh / sh));
            dw = (dw + 3) / 4 * 4; // block compression wants multiples of 4
            var sp = src.GetPixels32();
            var dp = new Color32[dw * dh];
            for (int y = 0; y < dh; y++)
            {
                int y0 = y * sh / dh;
                int y1 = Mathf.Max(y0 + 1, (y + 1) * sh / dh);
                for (int x = 0; x < dw; x++)
                {
                    int x0 = Mathf.Min(sw - 1, x * sw / dw);
                    int x1 = Mathf.Min(sw, Mathf.Max(x0 + 1, (x + 1) * sw / dw));
                    int r = 0, g = 0, b = 0, a = 0, n = 0;
                    for (int yy = y0; yy < y1; yy++)
                    {
                        int row = yy * sw;
                        for (int xx = x0; xx < x1; xx++)
                        {
                            var c = sp[row + xx];
                            r += c.r;
                            g += c.g;
                            b += c.b;
                            a += c.a;
                            n++;
                        }
                    }
                    dp[y * dw + x] = new Color32((byte)(r / n), (byte)(g / n), (byte)(b / n), (byte)(a / n));
                }
            }
            UnityEngine.Object.Destroy(src);
            var dst = new Texture2D(dw, dh, TextureFormat.RGBA32, true);
            dst.SetPixels32(dp);
            dst.Apply(true, false);
            dst.Compress(true);
            dst.Apply(false, true);
            return dst;
        }

        public static Color TypeColor(CardDef def)
        {
            if (def == null) return new Color(0.2f, 0.2f, 0.25f);
            if (def.IsTrainer)
            {
                switch (def.TrainerKind)
                {
                    case TrainerKind.Supporter: return new Color(0.85f, 0.45f, 0.2f);
                    case TrainerKind.Stadium: return new Color(0.3f, 0.6f, 0.35f);
                    case TrainerKind.Tool: return new Color(0.5f, 0.4f, 0.7f);
                    default: return new Color(0.25f, 0.45f, 0.8f);
                }
            }
            var type = def.IsEnergy ? def.EnergyProvides : def.PokemonType;
            switch (type)
            {
                case EnergyType.Grass: return new Color(0.3f, 0.65f, 0.3f);
                case EnergyType.Fire: return new Color(0.85f, 0.3f, 0.2f);
                case EnergyType.Water: return new Color(0.25f, 0.5f, 0.85f);
                case EnergyType.Lightning: return new Color(0.9f, 0.8f, 0.2f);
                case EnergyType.Psychic: return new Color(0.6f, 0.35f, 0.75f);
                case EnergyType.Fighting: return new Color(0.7f, 0.45f, 0.25f);
                case EnergyType.Darkness: return new Color(0.2f, 0.25f, 0.3f);
                case EnergyType.Metal: return new Color(0.6f, 0.62f, 0.65f);
                case EnergyType.Dragon: return new Color(0.7f, 0.6f, 0.2f);
                case EnergyType.Fairy: return new Color(0.9f, 0.5f, 0.7f);
                default: return new Color(0.75f, 0.73f, 0.68f);
            }
        }

        private static Texture2D Placeholder(CardDef def)
        {
            const int w = 126, h = 176, border = 7;
            var outer = (Color32)TypeColor(def);
            var inner = (Color32)Color.Lerp(TypeColor(def), Color.white, 0.55f);
            var px = new Color32[w * h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    px[y * w + x] = (x < border || y < border || x >= w - border || y >= h - border) ? outer : inner;
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.SetPixels32(px);
            tex.Apply();
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.name = "Placeholder " + def.Key;
            return tex;
        }

        public static Texture2D Back()
        {
            if (back != null) return back;
            const int w = 63, h = 88;
            var outer = new Color32(28, 45, 92, 255);
            var inner = new Color32(52, 82, 150, 255);
            var px = new Color32[w * h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    bool edge = x < 4 || y < 4 || x >= w - 4 || y >= h - 4;
                    bool stripe = ((x + y) / 6) % 2 == 0;
                    px[y * w + x] = edge ? outer : (stripe ? inner : outer);
                }
            back = new Texture2D(w, h, TextureFormat.RGBA32, false);
            back.SetPixels32(px);
            back.Apply();
            back.wrapMode = TextureWrapMode.Clamp;
            back.name = "Card back";
            return back;
        }

        /// <summary>A white filled circle with a soft edge (tint it with GUI.color), for Energy symbols.</summary>
        public static Texture2D Disc()
        {
            if (disc != null) return disc;
            const int n = 64;
            var px = new Color32[n * n];
            float c = (n - 1) / 2f;
            float radius = n / 2f - 1f;
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float d = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c));
                    float a = Mathf.Clamp01(radius - d + 0.5f);
                    px[y * n + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(a * 255f));
                }
            disc = new Texture2D(n, n, TextureFormat.RGBA32, false);
            disc.SetPixels32(px);
            disc.Apply();
            disc.wrapMode = TextureWrapMode.Clamp;
            disc.filterMode = FilterMode.Bilinear;
            disc.name = "Energy disc";
            return disc;
        }

        /// <summary>The letter used for an Energy type in card text ([G], [R], [W]...).</summary>
        public static string TypeLetter(EnergyType type)
        {
            switch (type)
            {
                case EnergyType.Grass: return "G";
                case EnergyType.Fire: return "R";
                case EnergyType.Water: return "W";
                case EnergyType.Lightning: return "L";
                case EnergyType.Psychic: return "P";
                case EnergyType.Fighting: return "F";
                case EnergyType.Darkness: return "D";
                case EnergyType.Metal: return "M";
                case EnergyType.Dragon: return "N";
                case EnergyType.Fairy: return "Y";
                default: return "C";
            }
        }

        public static Texture2D Solid(Color color)
        {
            var key = (Color32)color;
            Texture2D tex;
            if (solids.TryGetValue(key, out tex) && tex != null) return tex;
            tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            solids[key] = tex;
            return tex;
        }
    }
}
