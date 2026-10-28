using System;
using System.Collections.Generic;
using System.Text;

namespace PokemonTCG.AI
{
    /// <summary>
    /// Text reading (OCR) for the video reader (v77). Windows has an OCR engine built in (Windows.Media.Ocr, Japanese and
    /// English among its languages); the app runs it through a small PowerShell script over a list of picture files and reads
    /// the results back. This class holds the parts that don't depend on Windows: preparing a part of the frame so the OCR
    /// reads it well, the script itself, and reading its output.
    /// </summary>
    public static class VideoOcr
    {
        /// <summary>
        /// Gets a box of the frame ready for OCR: grey, dark text on a light background (light text on a dark background is
        /// inverted), contrast stretched, scaled up so the text is about 40 to 60 pixels high, with a white margin.
        /// </summary>
        public static VideoImage Prepare(VideoImage crop)
        {
            if (crop == null) return new VideoImage(8, 8);
            // Scale so the box is at least 64 pixels high (Windows' OCR misses small text), at most 4 times.
            int scale = 1;
            while (crop.Height * scale < 64 && scale < 4) scale++;
            var img = scale > 1 ? Enlarge(crop, scale) : crop;
            int w = img.Width, h = img.Height;
            var luma = new int[w * h];
            var histogram = new int[256];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int l = img.Luma(x, y);
                    luma[y * w + x] = l;
                    histogram[l]++;
                }
            // The background is the most common brightness; text on a dark background is turned dark-on-light.
            int median = Percentile(histogram, w * h, 0.5);
            bool invert = median < 110;
            int lo = Percentile(histogram, w * h, 0.03), hi = Percentile(histogram, w * h, 0.97);
            if (hi - lo < 30)
            {
                lo = 0;
                hi = 255;
            }
            const int margin = 16;
            var result = new VideoImage(w + margin * 2, h + margin * 2);
            result.Fill(0, 0, result.Width, result.Height, 255, 255, 255);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int l = luma[y * w + x];
                    int v = (l - lo) * 255 / Math.Max(1, hi - lo);
                    if (invert) v = 255 - v;
                    result.Set(x + margin, y + margin, v, v, v);
                }
            return result;
        }

        private static VideoImage Enlarge(VideoImage src, int scale)
        {
            // Bilinear, so the letters stay smooth.
            int w = src.Width * scale, h = src.Height * scale;
            var dst = new VideoImage(w, h);
            for (int y = 0; y < h; y++)
            {
                double sy = (y + 0.5) / scale - 0.5;
                int y0 = Math.Max(0, Math.Min(src.Height - 1, (int)Math.Floor(sy)));
                int y1 = Math.Min(src.Height - 1, y0 + 1);
                double fy = Math.Max(0, Math.Min(1, sy - y0));
                for (int x = 0; x < w; x++)
                {
                    double sx = (x + 0.5) / scale - 0.5;
                    int x0 = Math.Max(0, Math.Min(src.Width - 1, (int)Math.Floor(sx)));
                    int x1 = Math.Min(src.Width - 1, x0 + 1);
                    double fx = Math.Max(0, Math.Min(1, sx - x0));
                    dst.Set(x, y,
                        Lerp2(src.R(x0, y0), src.R(x1, y0), src.R(x0, y1), src.R(x1, y1), fx, fy),
                        Lerp2(src.G(x0, y0), src.G(x1, y0), src.G(x0, y1), src.G(x1, y1), fx, fy),
                        Lerp2(src.B(x0, y0), src.B(x1, y0), src.B(x0, y1), src.B(x1, y1), fx, fy));
                }
            }
            return dst;
        }

        private static int Lerp2(int a, int b, int c, int d, double fx, double fy)
        {
            double top = a + (b - a) * fx;
            double bottom = c + (d - c) * fx;
            return (int)Math.Round(top + (bottom - top) * fy);
        }

        private static int Percentile(int[] histogram, int total, double fraction)
        {
            long target = (long)(total * fraction);
            long seen = 0;
            for (int i = 0; i < histogram.Length; i++)
            {
                seen += histogram[i];
                if (seen > target) return i;
            }
            return 255;
        }

        // ------------------------------------------------------------------ The Windows OCR script

        /// <summary>
        /// The PowerShell script (Windows PowerShell 5.1: powershell.exe, which can use Windows' own OCR). Arguments: -List, a
        /// UTF-8 text file with one picture path per line; -Out, where to write the results; -Lang, the OCR language ("ja",
        /// "en"; any installed variant such as "en-US" is used). It writes one line per picture, "path TAB text" (the lines of text joined with " / "), as UTF-8. If the
        /// language isn't installed it writes "#error TAB language TAB the installed languages" and stops.
        /// </summary>
        public const string Script = @"param([string]$List, [string]$Out, [string]$Lang)
$ErrorActionPreference = 'Stop'
$results = New-Object System.Collections.Generic.List[string]
try {
    Add-Type -AssemblyName System.Runtime.WindowsRuntime
    $null = [Windows.Storage.StorageFile, Windows.Storage, ContentType = WindowsRuntime]
    $null = [Windows.Storage.Streams.IRandomAccessStream, Windows.Storage.Streams, ContentType = WindowsRuntime]
    $null = [Windows.Media.Ocr.OcrEngine, Windows.Foundation, ContentType = WindowsRuntime]
    $null = [Windows.Graphics.Imaging.BitmapDecoder, Windows.Graphics, ContentType = WindowsRuntime]
    $null = [Windows.Graphics.Imaging.SoftwareBitmap, Windows.Graphics, ContentType = WindowsRuntime]
    $null = [Windows.Globalization.Language, Windows.Globalization, ContentType = WindowsRuntime]
    $asTaskGeneric = ([System.WindowsRuntimeSystemExtensions].GetMethods() | Where-Object { $_.Name -eq 'AsTask' -and $_.GetParameters().Count -eq 1 -and $_.GetParameters()[0].ParameterType.Name -eq 'IAsyncOperation`1' })[0]
    function Await($op, [Type]$type) {
        $task = $asTaskGeneric.MakeGenericMethod($type).Invoke($null, @($op))
        $task.Wait(-1) | Out-Null
        $task.Result
    }
    $language = $null
    foreach ($l in [Windows.Media.Ocr.OcrEngine]::AvailableRecognizerLanguages) {
        if ($l.LanguageTag -eq $Lang -or $l.LanguageTag.StartsWith($Lang + '-')) { $language = $l; break }
    }
    if ($language -eq $null) {
        $installed = ([Windows.Media.Ocr.OcrEngine]::AvailableRecognizerLanguages | ForEach-Object { $_.LanguageTag }) -join ', '
        $results.Add(""#error`tlanguage`t$installed"")
    } else {
        $engine = [Windows.Media.Ocr.OcrEngine]::TryCreateFromLanguage($language)
        foreach ($path in [System.IO.File]::ReadAllLines($List, [System.Text.Encoding]::UTF8)) {
            if ($path.Trim().Length -eq 0) { continue }
            $text = ''
            try {
                $file = Await ([Windows.Storage.StorageFile]::GetFileFromPathAsync($path)) ([Windows.Storage.StorageFile])
                $stream = Await ($file.OpenAsync([Windows.Storage.FileAccessMode]::Read)) ([Windows.Storage.Streams.IRandomAccessStream])
                try {
                    $decoder = Await ([Windows.Graphics.Imaging.BitmapDecoder]::CreateAsync($stream)) ([Windows.Graphics.Imaging.BitmapDecoder])
                    $bitmap = Await ($decoder.GetSoftwareBitmapAsync()) ([Windows.Graphics.Imaging.SoftwareBitmap])
                    $ocr = Await ($engine.RecognizeAsync($bitmap)) ([Windows.Media.Ocr.OcrResult])
                    $text = ($ocr.Lines | ForEach-Object { $_.Text }) -join ' / '
                } finally { try { $stream.Dispose() } catch { } }
            } catch { $text = '#failed ' + $_.Exception.Message }
            $text = $text -replace ""[`t`r`n]"", ' '
            $results.Add(""$path`t$text"")
        }
    }
} catch {
    $results.Add(""#error`tscript`t"" + ($_.Exception.Message -replace ""[`t`r`n]"", ' '))
}
[System.IO.File]::WriteAllLines($Out, $results, (New-Object System.Text.UTF8Encoding $false))
";

        /// <summary>
        /// The script's output: picture path to text (null for a picture it couldn't open). error is set when the whole run
        /// failed (language missing, no WinRT).
        /// </summary>
        public static Dictionary<string, string> ParseResults(string output, out string error)
        {
            error = null;
            var results = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var raw in (output ?? "").Replace("\r", "").Split('\n'))
            {
                if (raw.Length == 0) continue;
                int tab = raw.IndexOf('\t');
                if (raw.StartsWith("#error", StringComparison.Ordinal))
                {
                    var parts = raw.Split('\t');
                    string what = parts.Length > 1 ? parts[1] : "";
                    string detail = parts.Length > 2 ? parts[2] : "";
                    error = what == "language"
                        ? "Windows doesn't have the OCR language installed (installed: " + (detail.Length > 0 ? detail : "none") + ")."
                        : "The OCR script failed: " + detail;
                    continue;
                }
                if (tab < 0) continue;
                string path = raw.Substring(0, tab);
                string text = raw.Substring(tab + 1);
                // A picture the OCR couldn't open has no reading (null), which isn't the same as reading nothing ("").
                results[path] = text.StartsWith("#failed", StringComparison.Ordinal) ? null : text.Trim();
            }
            return results;
        }

        /// <summary>The list file the script reads: one path per line.</summary>
        public static string ListFile(IEnumerable<string> paths)
        {
            var sb = new StringBuilder();
            foreach (var p in paths) sb.Append(p).Append("\r\n");
            return sb.ToString();
        }

        /// <summary>What to tell someone whose Windows doesn't have the Japanese OCR.</summary>
        public const string JapaneseHelp =
            "To read Japanese, add Japanese in Windows Settings > Time & language > Language & region (the OCR comes with it), or run "
            + "in an administrator PowerShell: Add-WindowsCapability -Online -Name \"Language.OCR~~~ja-JP~0.0.1.0\"";
    }
}
