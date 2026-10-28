using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// v153: card images saved as AVIF (PokéBeach's format) or WebP. Unity only reads PNG and JPG, so on a computer these are
    /// converted to PNG once, with a converter the computer already has, and the PNG is used from then on (it's made again only when
    /// the original file changes). Tried in order: Windows's own image codecs (through PowerShell; AVIF needs Microsoft's free "AV1
    /// Video Extension", WebP the "WebP Image Extensions", both usually installed with Windows 11), macOS's sips, then ffmpeg,
    /// ImageMagick (magick), avifdec and dwebp if they're installed. Phones and tablets can't run these: convert the images on a
    /// computer first (the Android build does that for the images inside the project).
    /// Touches no Unity API, so it can run on a background thread.
    /// </summary>
    public static class ImageConverter
    {
        /// <summary>Image formats that are converted to PNG before they're loaded.</summary>
        public static bool IsConvertible(string extension)
        {
            string ext = (extension ?? "").ToLowerInvariant();
            return ext == ".avif" || ext == ".webp";
        }

        /// <summary>
        /// v153.1: what a file really is, from its first 12 bytes: ".avif", ".webp", or null for anything else (PNG, JPG...).
        /// An AVIF or WebP picture saved or renamed with a .jpg or .png name (PokéBeach's images are AVIF) is found this way.
        /// </summary>
        public static string RealFormat(byte[] head)
        {
            if (head == null || head.Length < 12) return null;
            if (head[0] == (byte)'R' && head[1] == (byte)'I' && head[2] == (byte)'F' && head[3] == (byte)'F'
                && head[8] == (byte)'W' && head[9] == (byte)'E' && head[10] == (byte)'B' && head[11] == (byte)'P')
                return ".webp";
            if (head[4] == (byte)'f' && head[5] == (byte)'t' && head[6] == (byte)'y' && head[7] == (byte)'p')
            {
                string brand = Encoding.ASCII.GetString(head, 8, 4);
                if (brand == "avif" || brand == "avis" || brand == "mif1" || brand == "msf1") return ".avif";
            }
            return null;
        }

        /// <summary>v153.1: the file's real format (RealFormat), read from the file; its extension when that can't be told.</summary>
        private static string FormatOf(string path)
        {
            try
            {
                var head = new byte[12];
                int read = 0;
                using (var stream = File.OpenRead(path))
                {
                    while (read < head.Length)
                    {
                        int n = stream.Read(head, read, head.Length - read);
                        if (n <= 0) break;
                        read += n;
                    }
                }
                string real = read == head.Length ? RealFormat(head) : null;
                if (real != null) return real;
            }
            catch (Exception) { }
            return (Path.GetExtension(path) ?? "").ToLowerInvariant();
        }

        /// <summary>Where the PNG made from this file goes in the cache folder (a stable name, so it's found again next time).</summary>
        public static string CachedPngPath(string cacheFolder, string source)
        {
            string bare = Path.GetFileNameWithoutExtension(source);
            var sb = new StringBuilder();
            foreach (char ch in bare) sb.Append(char.IsLetterOrDigit(ch) || ch == '-' || ch == '_' ? ch : '_');
            return Path.Combine(cacheFolder, sb + "_" + StableHash(source.Replace('\\', '/')).ToString("x8") + ".png");
        }

        /// <summary>
        /// Makes a PNG for each (source, target) pair whose target is missing or older than the source. Returns the pairs whose PNG
        /// is ready; the others are added to problems with the reason.
        /// </summary>
        public static List<KeyValuePair<string, string>> ConvertAll(IList<KeyValuePair<string, string>> pairs, List<string> problems)
        {
            var ready = new List<KeyValuePair<string, string>>();
            var pending = new List<KeyValuePair<string, string>>();
            foreach (var pair in pairs)
            {
                if (Fresh(pair.Key, pair.Value)) ready.Add(pair);
                else pending.Add(pair);
            }
            if (pending.Count == 0) return ready;

            foreach (var pair in pending)
            {
                try
                {
                    string dir = Path.GetDirectoryName(pair.Value);
                    if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                }
                catch (Exception) { }
            }

            // Windows: one PowerShell run converts the whole list with the system's codecs.
            if (Environment.OSVersion.Platform == PlatformID.Win32NT) ConvertWithWindowsCodecs(pending);
            pending = StillPending(pending, ready);

            // Then one file at a time with whatever command-line converter is installed.
            var tools = new List<Func<string, string, string[]>>();
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
                tools.Add((inPath, outPath) => new[] { "sips", "-s format png \"" + inPath + "\" --out \"" + outPath + "\"" });
            tools.Add((inPath, outPath) => new[] { "ffmpeg", "-y -loglevel error -i \"" + inPath + "\" -frames:v 1 \"" + outPath + "\"" });
            tools.Add((inPath, outPath) => new[] { "magick", "\"" + inPath + "\" \"" + outPath + "\"" });
            // ImageMagick 6's "convert" (never on Windows, where convert.exe is the system's disk tool).
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                tools.Add((inPath, outPath) => new[] { "convert", "\"" + inPath + "\" \"" + outPath + "\"" });
            tools.Add((inPath, outPath) => FormatOf(inPath) == ".avif" ? new[] { "avifdec", "\"" + inPath + "\" \"" + outPath + "\"" } : null);
            tools.Add((inPath, outPath) => FormatOf(inPath) == ".webp" ? new[] { "dwebp", "\"" + inPath + "\" -o \"" + outPath + "\"" } : null);
            var missing = new HashSet<string>();
            foreach (var tool in tools)
            {
                if (pending.Count == 0) break;
                foreach (var pair in pending)
                {
                    var command = tool(pair.Key, pair.Value);
                    if (command == null || missing.Contains(command[0])) continue;
                    if (!Run(command[0], command[1], null, 60000)) missing.Add(command[0]);
                }
                pending = StillPending(pending, ready);
            }

            foreach (var pair in pending)
                problems.Add(Path.GetFileName(pair.Key) + " (couldn't convert it to PNG: on Windows install Microsoft's free \"AV1 Video"
                             + " Extension\" for AVIF or \"WebP Image Extensions\" for WebP from the Microsoft Store, or install ffmpeg;"
                             + " or re-save it as PNG)");
            return ready;
        }

        /// <summary>The PNG exists and isn't older than the original.</summary>
        public static bool Fresh(string source, string target)
        {
            try
            {
                return File.Exists(target) && new FileInfo(target).Length > 0
                       && File.GetLastWriteTimeUtc(target) >= File.GetLastWriteTimeUtc(source);
            }
            catch (Exception) { return false; }
        }

        private static List<KeyValuePair<string, string>> StillPending(List<KeyValuePair<string, string>> pending, List<KeyValuePair<string, string>> ready)
        {
            var left = new List<KeyValuePair<string, string>>();
            foreach (var pair in pending)
            {
                if (Fresh(pair.Key, pair.Value)) ready.Add(pair);
                else left.Add(pair);
            }
            return left;
        }

        /// <summary>
        /// Windows: decodes with WPF's BitmapDecoder, which uses the Windows Imaging Component codecs installed on the computer
        /// (AVIF through the AV1 Video Extension, WebP through the WebP Image Extensions), and saves PNG. The list of files goes in a
        /// text file named by an environment variable, so no path needs quoting on the command line.
        /// </summary>
        private static void ConvertWithWindowsCodecs(List<KeyValuePair<string, string>> pending)
        {
            string listFile = null;
            try
            {
                listFile = Path.Combine(Path.GetTempPath(), "ptcg_convert_" + Guid.NewGuid().ToString("N") + ".txt");
                var sb = new StringBuilder();
                foreach (var pair in pending) sb.Append(pair.Key).Append('\t').Append(pair.Value).Append('\n');
                File.WriteAllText(listFile, sb.ToString(), new UTF8Encoding(false));
                const string script =
                    "$ErrorActionPreference = 'Continue'\n" +
                    "Add-Type -AssemblyName PresentationCore\n" +
                    "foreach ($line in [IO.File]::ReadAllLines($env:PTCG_CONVERT_LIST)) {\n" +
                    "  $p = $line.Split(\"`t\")\n" +
                    "  if ($p.Length -lt 2) { continue }\n" +
                    "  try {\n" +
                    "    $s = [IO.File]::OpenRead($p[0])\n" +
                    "    try {\n" +
                    "      $d = [System.Windows.Media.Imaging.BitmapDecoder]::Create($s, [System.Windows.Media.Imaging.BitmapCreateOptions]::None, [System.Windows.Media.Imaging.BitmapCacheOption]::OnLoad)\n" +
                    "    } finally { $s.Close() }\n" +
                    "    $e = New-Object System.Windows.Media.Imaging.PngBitmapEncoder\n" +
                    "    $e.Frames.Add([System.Windows.Media.Imaging.BitmapFrame]::Create($d.Frames[0]))\n" +
                    "    $o = [IO.File]::Create($p[1])\n" +
                    "    try { $e.Save($o) } finally { $o.Close() }\n" +
                    "  } catch { if (Test-Path $p[1]) { Remove-Item $p[1] -ErrorAction SilentlyContinue } }\n" +
                    "}\n";
                string encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
                var env = new Dictionary<string, string> { { "PTCG_CONVERT_LIST", listFile } };
                Run("powershell", "-NoProfile -NonInteractive -ExecutionPolicy Bypass -STA -EncodedCommand " + encoded, env, 30000 + 2000 * pending.Count);
            }
            catch (Exception) { }
            finally
            {
                try { if (listFile != null && File.Exists(listFile)) File.Delete(listFile); }
                catch (Exception) { }
            }
        }

        /// <summary>Runs a program and waits for it. False when it isn't installed (or couldn't start).</summary>
        private static bool Run(string program, string arguments, Dictionary<string, string> environment, int timeoutMs)
        {
            try
            {
                var psi = new ProcessStartInfo(program, arguments)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                if (environment != null)
                    foreach (var kv in environment) psi.EnvironmentVariables[kv.Key] = kv.Value;
                using (var process = Process.Start(psi))
                {
                    if (process == null) return false;
                    // Read the output so a chatty program can't fill its pipe and stall.
                    process.OutputDataReceived += (s, e) => { };
                    process.ErrorDataReceived += (s, e) => { };
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    if (!process.WaitForExit(timeoutMs))
                    {
                        try { process.Kill(); }
                        catch (Exception) { }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>FNV-1a: the same number for the same path on every run (string.GetHashCode isn't).</summary>
        private static uint StableHash(string text)
        {
            uint h = 2166136261;
            foreach (char ch in text)
            {
                h ^= ch;
                h *= 16777619;
            }
            return h;
        }
    }
}
