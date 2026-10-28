using System;
using System.IO;
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// "Open file" dialog (v77, for picking a video): the editor's file panel, Windows' own dialog in Windows builds, and
    /// nothing elsewhere (the path is typed or pasted instead).
    /// </summary>
    public static class FilePicker
    {
        public static bool CanBrowse
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                return true;
#else
                return false;
#endif
            }
        }

        /// <summary>Opens the dialog (blocks until it closes); null if cancelled. Call from Update, not OnGUI.</summary>
        public static string PickVideo(string title, string startFolder)
        {
#if UNITY_EDITOR
            string picked = UnityEditor.EditorUtility.OpenFilePanelWithFilters(title, Directory.Exists(startFolder ?? "") ? startFolder : "",
                new[] { "Videos", "mp4,m4v,mov,webm,avi,mkv,wmv", "All files", "*" });
            return string.IsNullOrEmpty(picked) ? null : picked.Replace('/', Path.DirectorySeparatorChar);
#elif UNITY_STANDALONE_WIN
            return WindowsOpenFile.Pick(title, startFolder, "Videos\0*.mp4;*.m4v;*.mov;*.webm;*.avi;*.mkv;*.wmv\0All files\0*.*\0\0");
#else
            return null;
#endif
        }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        /// <summary>The Windows "Open" dialog (comdlg32), for Windows builds.</summary>
        private static class WindowsOpenFile
        {
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            private struct OpenFileName
            {
                public int lStructSize;
                public IntPtr hwndOwner;
                public IntPtr hInstance;
                public string lpstrFilter;
                public string lpstrCustomFilter;
                public int nMaxCustFilter;
                public int nFilterIndex;
                public IntPtr lpstrFile;
                public int nMaxFile;
                public string lpstrFileTitle;
                public int nMaxFileTitle;
                public string lpstrInitialDir;
                public string lpstrTitle;
                public int Flags;
                public short nFileOffset;
                public short nFileExtension;
                public string lpstrDefExt;
                public IntPtr lCustData;
                public IntPtr lpfnHook;
                public string lpTemplateName;
                public IntPtr pvReserved;
                public int dwReserved;
                public int FlagsEx;
            }

            private const int FileMustExist = 0x1000, PathMustExist = 0x800, NoChangeDir = 0x8, Explorer = 0x80000;
            private const int MaxPath = 2048;

            [DllImport("comdlg32.dll", EntryPoint = "GetOpenFileNameW", CharSet = CharSet.Unicode, ExactSpelling = true)]
            private static extern bool GetOpenFileName(ref OpenFileName ofn);

            [DllImport("user32.dll", ExactSpelling = true)]
            private static extern IntPtr GetActiveWindow();

            public static string Pick(string title, string startFolder, string filter)
            {
                IntPtr buffer = Marshal.AllocHGlobal(MaxPath * 2);
                try
                {
                    // An empty, zeroed buffer for the chosen path.
                    for (int i = 0; i < MaxPath * 2; i++) Marshal.WriteByte(buffer, i, 0);
                    var ofn = new OpenFileName
                    {
                        lStructSize = Marshal.SizeOf(typeof(OpenFileName)),
                        hwndOwner = GetActiveWindow(),
                        lpstrFilter = filter,
                        nFilterIndex = 1,
                        lpstrFile = buffer,
                        nMaxFile = MaxPath,
                        lpstrInitialDir = Directory.Exists(startFolder ?? "") ? startFolder : null,
                        lpstrTitle = title,
                        Flags = FileMustExist | PathMustExist | NoChangeDir | Explorer
                    };
                    if (!GetOpenFileName(ref ofn)) return null;
                    string path = Marshal.PtrToStringUni(buffer);
                    return string.IsNullOrEmpty(path) ? null : path;
                }
                catch (Exception)
                {
                    return null;
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
        }
#endif
    }
}
