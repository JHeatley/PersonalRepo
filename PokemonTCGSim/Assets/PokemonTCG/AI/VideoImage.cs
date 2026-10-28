using System;
using System.IO;

namespace PokemonTCG.AI
{
    /// <summary>
    /// A picture as plain RGBA bytes, rows from the top (v77): a video frame or a part of one, or a card image. The video
    /// reader works on these so it doesn't depend on Unity (the Unity side copies frames and card images into them).
    /// </summary>
    public sealed class VideoImage
    {
        public readonly int Width, Height;
        /// <summary>4 bytes per pixel (red, green, blue, alpha), row by row from the top left.</summary>
        public readonly byte[] Rgba;

        public VideoImage(int width, int height)
        {
            Width = Math.Max(1, width);
            Height = Math.Max(1, height);
            Rgba = new byte[Width * Height * 4];
        }

        public VideoImage(int width, int height, byte[] rgba)
        {
            Width = Math.Max(1, width);
            Height = Math.Max(1, height);
            if (rgba == null || rgba.Length < Width * Height * 4) throw new ArgumentException("Not enough pixels for " + Width + "x" + Height + ".");
            Rgba = rgba;
        }

        public int R(int x, int y) { return Rgba[(y * Width + x) * 4]; }
        public int G(int x, int y) { return Rgba[(y * Width + x) * 4 + 1]; }
        public int B(int x, int y) { return Rgba[(y * Width + x) * 4 + 2]; }

        public void Set(int x, int y, int r, int g, int b)
        {
            int i = (y * Width + x) * 4;
            Rgba[i] = Clamp(r);
            Rgba[i + 1] = Clamp(g);
            Rgba[i + 2] = Clamp(b);
            Rgba[i + 3] = 255;
        }

        /// <summary>Fills a rectangle with one colour (for tests and simple drawings).</summary>
        public void Fill(int x, int y, int w, int h, int r, int g, int b)
        {
            for (int yy = Math.Max(0, y); yy < Math.Min(Height, y + h); yy++)
                for (int xx = Math.Max(0, x); xx < Math.Min(Width, x + w); xx++)
                    Set(xx, yy, r, g, b);
        }

        private static byte Clamp(int v)
        {
            return (byte)(v < 0 ? 0 : v > 255 ? 255 : v);
        }

        /// <summary>Brightness of a pixel, 0 to 255.</summary>
        public int Luma(int x, int y)
        {
            int i = (y * Width + x) * 4;
            return (Rgba[i] * 299 + Rgba[i + 1] * 587 + Rgba[i + 2] * 114) / 1000;
        }

        /// <summary>A part of the picture in pixels (clipped to the picture; at least 1x1).</summary>
        public VideoImage Crop(int x, int y, int w, int h)
        {
            int x0 = Math.Max(0, Math.Min(Width - 1, x));
            int y0 = Math.Max(0, Math.Min(Height - 1, y));
            int x1 = Math.Max(x0 + 1, Math.Min(Width, x + w));
            int y1 = Math.Max(y0 + 1, Math.Min(Height, y + h));
            var crop = new VideoImage(x1 - x0, y1 - y0);
            for (int yy = y0; yy < y1; yy++)
                Buffer.BlockCopy(Rgba, (yy * Width + x0) * 4, crop.Rgba, (yy - y0) * crop.Width * 4, crop.Width * 4);
            return crop;
        }

        /// <summary>A part of the picture given as fractions of its size (0 to 1, from the top left).</summary>
        public VideoImage CropNormalized(double x, double y, double w, double h)
        {
            return Crop((int)Math.Round(x * Width), (int)Math.Round(y * Height), (int)Math.Round(w * Width), (int)Math.Round(h * Height));
        }

        /// <summary>The picture scaled to a new size: each new pixel is the average of the pixels it covers (or the nearest one when enlarging).</summary>
        public VideoImage Resize(int width, int height)
        {
            width = Math.Max(1, width);
            height = Math.Max(1, height);
            var result = new VideoImage(width, height);
            for (int y = 0; y < height; y++)
            {
                int sy0 = y * Height / height;
                int sy1 = Math.Max(sy0 + 1, (y + 1) * Height / height);
                for (int x = 0; x < width; x++)
                {
                    int sx0 = x * Width / width;
                    int sx1 = Math.Max(sx0 + 1, (x + 1) * Width / width);
                    long r = 0, g = 0, b = 0;
                    int n = 0;
                    for (int sy = sy0; sy < sy1 && sy < Height; sy++)
                        for (int sx = sx0; sx < sx1 && sx < Width; sx++)
                        {
                            int i = (sy * Width + sx) * 4;
                            r += Rgba[i];
                            g += Rgba[i + 1];
                            b += Rgba[i + 2];
                            n++;
                        }
                    if (n == 0) n = 1;
                    result.Set(x, y, (int)(r / n), (int)(g / n), (int)(b / n));
                }
            }
            return result;
        }

        /// <summary>Average brightness, 0 to 255.</summary>
        public double MeanLuma()
        {
            long sum = 0;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    sum += Luma(x, y);
            return sum / (double)(Width * Height);
        }

        /// <summary>A small grey thumbnail (16 x 8) to tell quickly whether a part of the screen changed.</summary>
        public byte[] Signature()
        {
            var small = Resize(16, 8);
            var sig = new byte[16 * 8];
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 16; x++)
                    sig[y * 16 + x] = (byte)small.Luma(x, y);
            return sig;
        }

        /// <summary>The average difference between two signatures, 0 (the same) to 255.</summary>
        public static double SignatureDifference(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return 255;
            long sum = 0;
            for (int i = 0; i < a.Length; i++) sum += Math.Abs(a[i] - b[i]);
            return sum / (double)a.Length;
        }

        /// <summary>
        /// Counts the filled icons in a row of icon places (Prize card icons: the ones still there are colourful, the taken
        /// ones grey or gone). The row is split into equal places; a place counts when enough of its middle is a strong colour.
        /// </summary>
        public int CountFilledIcons(int slots)
        {
            slots = Math.Max(1, slots);
            var shares = new double[slots];
            double max = 0;
            for (int s = 0; s < slots; s++)
            {
                int x0 = s * Width / slots, x1 = (s + 1) * Width / slots;
                int cx0 = x0 + (x1 - x0) / 5, cx1 = x1 - (x1 - x0) / 5;
                int cy0 = Height / 5, cy1 = Height - Height / 5;
                int n = 0, colourful = 0;
                for (int y = cy0; y < cy1; y++)
                    for (int x = cx0; x < cx1; x++)
                    {
                        int r = R(x, y), g = G(x, y), b = B(x, y);
                        int hi = Math.Max(r, Math.Max(g, b)), lo = Math.Min(r, Math.Min(g, b));
                        n++;
                        if (hi > 64 && (hi - lo) > 0.35 * hi) colourful++;
                    }
                shares[s] = n > 0 ? colourful / (double)n : 0;
                if (shares[s] > max) max = shares[s];
            }
            if (max < 0.15) return 0;
            double threshold = Math.Max(0.15, max * 0.5);
            int count = 0;
            foreach (var share in shares) if (share >= threshold) count++;
            return count;
        }

        /// <summary>The picture as a 24-bit BMP file (Windows' OCR reads these; no image library needed to write one).</summary>
        public byte[] ToBmp()
        {
            int rowSize = (Width * 3 + 3) / 4 * 4;
            int dataSize = rowSize * Height;
            using (var ms = new MemoryStream(54 + dataSize))
            using (var w = new BinaryWriter(ms))
            {
                w.Write((byte)'B');
                w.Write((byte)'M');
                w.Write(54 + dataSize);
                w.Write(0);
                w.Write(54);
                w.Write(40);
                w.Write(Width);
                w.Write(Height);
                w.Write((short)1);
                w.Write((short)24);
                w.Write(0);
                w.Write(dataSize);
                w.Write(2835);
                w.Write(2835);
                w.Write(0);
                w.Write(0);
                var row = new byte[rowSize];
                // BMP rows go from the bottom up, each pixel blue, green, red.
                for (int y = Height - 1; y >= 0; y--)
                {
                    Array.Clear(row, 0, row.Length);
                    for (int x = 0; x < Width; x++)
                    {
                        int i = (y * Width + x) * 4;
                        row[x * 3] = Rgba[i + 2];
                        row[x * 3 + 1] = Rgba[i + 1];
                        row[x * 3 + 2] = Rgba[i];
                    }
                    w.Write(row);
                }
                w.Flush();
                return ms.ToArray();
            }
        }
    }
}
