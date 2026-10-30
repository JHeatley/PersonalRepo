using System;
using System.IO;
using System.Text;

namespace PokemonTCG.AI
{
    /// <summary>
    /// Saving a file so a crash (the whole PC, or a power cut) can't lose it or leave it half written: the new contents go to
    /// path.tmp and are forced onto the disk, then replace the old file in one step. At any moment there is the old file or the
    /// new one. Recover, before reading, puts back a new file that a crash left only as path.tmp.
    /// </summary>
    public static class SafeFile
    {
        public static void Write(string path, Action<Stream> write)
        {
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            string temp = path + ".tmp";
            using (var stream = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 16))
            {
                write(stream);
                stream.Flush(true); // on the disk, not only in Windows' write cache
            }
            if (!File.Exists(path))
            {
                File.Move(temp, path);
                return;
            }
            try
            {
                File.Replace(temp, path, null);
            }
            catch (Exception e) when (e is IOException || e is PlatformNotSupportedException || e is UnauthorizedAccessException)
            {
                // A drive that can't replace in one step (some network or FAT drives): the old way.
                File.Delete(path);
                File.Move(temp, path);
            }
        }

        public static void WriteBytes(string path, byte[] bytes)
        {
            Write(path, s => s.Write(bytes, 0, bytes.Length));
        }

        public static void WriteText(string path, string text)
        {
            WriteBytes(path, new UTF8Encoding(false).GetBytes(text));
        }

        /// <summary>A crash between taking the old file away and putting the new one in left only path.tmp: it becomes path.</summary>
        public static void Recover(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            string temp = path + ".tmp";
            try
            {
                if (!File.Exists(path) && File.Exists(temp) && new FileInfo(temp).Length > 0) File.Move(temp, path);
            }
            catch (IOException)
            {
            }
        }
    }
}
