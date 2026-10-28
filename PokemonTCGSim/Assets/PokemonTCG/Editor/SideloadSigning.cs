using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PokemonTCG.EditorTools
{
    /// <summary>
    /// v118.3: every Android build is signed with the same key and gets a higher version number, so Android installs a new APK as
    /// an update of the one on the phone (saved decks and the AI's brain stay) instead of refusing it ("App not installed" /
    /// "package conflicts with an existing package") until you uninstall.
    ///
    /// The key lives in the project folder, next to Assets: AndroidSigning/PokemonTCG-sideload.keystore. It's made the first
    /// time you build (with the JDK that comes with Unity's Android module). If the computer already has the Android debug key
    /// (.android/debug.keystore in your user folder) that earlier builds were signed with, that key is copied there instead
    /// (as AndroidSigning/adopted-debug.keystore), so the APK on your phone takes the next build as an update too.
    /// Keep the AndroidSigning folder (back it up): a build signed with a different key can't update the installed app.
    /// </summary>
    public static class SideloadSigning
    {
        public const string Folder = "AndroidSigning";
        public const string OwnKeyFile = "PokemonTCG-sideload.keystore";
        public const string AdoptedKeyFile = "adopted-debug.keystore";
        /// <summary>The project's own key. It only signs a side-loaded fan build, so the password isn't a secret.</summary>
        public const string OwnAlias = "pokemontcg", OwnPassword = "pokemontcg-sideload";
        /// <summary>The Android debug key's standard alias and password.</summary>
        public const string DebugAlias = "androiddebugkey", DebugPassword = "android";

        /// <summary>The AndroidSigning folder (absolute).</summary>
        public static string FolderPath
        {
            get { return Path.Combine(Directory.GetParent(Application.dataPath).FullName, Folder); }
        }

        /// <summary>
        /// Points the Android player settings at the project's key (making or adopting it the first time). Returns a line for the
        /// Console; when no key can be had (no JDK found), the build is signed with Unity's debug key as before.
        /// </summary>
        public static string Apply()
        {
            string alias, password;
            string path = FindOrCreateKey(out alias, out password);
            if (path == null)
            {
                PlayerSettings.Android.useCustomKeystore = false;
                return "signed with Unity's debug key (no Java keytool was found to make the project's own key; the APK may not install as an update)";
            }
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = path;
            PlayerSettings.Android.keystorePass = password;
            PlayerSettings.Android.keyaliasName = alias;
            PlayerSettings.Android.keyaliasPass = password;
            return "signed with " + path;
        }

        /// <summary>
        /// Gives the build a version code higher than any before it (minutes since 2026, or one more than the last one), so
        /// Android treats it as newer than the installed APK. Returns the new code.
        /// </summary>
        public static int BumpVersionCode()
        {
            int byClock = (int)Math.Min(2000000000.0, Math.Max(1.0, (DateTime.UtcNow - new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMinutes));
            int code = Math.Max(byClock, PlayerSettings.Android.bundleVersionCode + 1);
            PlayerSettings.Android.bundleVersionCode = code;
            return code;
        }

        private static string FindOrCreateKey(out string alias, out string password)
        {
            string dir = FolderPath;
            string own = Path.Combine(dir, OwnKeyFile);
            string adopted = Path.Combine(dir, AdoptedKeyFile);
            alias = OwnAlias;
            password = OwnPassword;
            if (File.Exists(own)) return own;
            if (File.Exists(adopted))
            {
                alias = DebugAlias;
                password = DebugPassword;
                return adopted;
            }

            string keytool = FindKeytool();
            if (keytool == null) return null;
            Directory.CreateDirectory(dir);

            // Earlier builds were signed with the Android debug key: keep using it, so the installed app updates.
            foreach (var debugKey in DebugKeyPlaces())
            {
                if (!File.Exists(debugKey)) continue;
                string output;
                if (Run(keytool, "-list -keystore " + Quote(debugKey) + " -storepass " + DebugPassword + " -alias " + DebugAlias, out output) != 0) continue;
                try
                {
                    File.Copy(debugKey, adopted, false);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("[PokemonTCG] Couldn't copy the Android debug key " + debugKey + ": " + e.Message);
                    continue;
                }
                Debug.Log("[PokemonTCG] Android signing: copied the debug key earlier builds used (" + debugKey + ") to " + adopted
                          + ". Every build is signed with it from now on; keep the " + Folder + " folder.");
                alias = DebugAlias;
                password = DebugPassword;
                return adopted;
            }

            // No earlier key: make the project's own (good for 100 years).
            string made;
            int exit = Run(keytool, "-genkeypair -keystore " + Quote(own) + " -storepass " + OwnPassword + " -keypass " + OwnPassword
                                    + " -alias " + OwnAlias + " -keyalg RSA -keysize 2048 -validity 36500"
                                    + " -dname " + Quote("CN=Pokemon TCG Engine (side-load), O=Fan project"), out made);
            if (exit != 0 || !File.Exists(own))
            {
                Debug.LogWarning("[PokemonTCG] Couldn't make the Android signing key with " + keytool + " (exit " + exit + "): " + made);
                return null;
            }
            Debug.Log("[PokemonTCG] Android signing: made the project's key, " + own + ". Every build is signed with it from now on; keep the "
                      + Folder + " folder (back it up).");
            return own;
        }

        /// <summary>Where the Android debug key usually is.</summary>
        private static string[] DebugKeyPlaces()
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string userHome = Environment.GetEnvironmentVariable("ANDROID_USER_HOME");
            string sdkHome = Environment.GetEnvironmentVariable("ANDROID_SDK_HOME");
            return new[]
            {
                string.IsNullOrEmpty(userHome) ? "" : Path.Combine(userHome, "debug.keystore"),
                string.IsNullOrEmpty(sdkHome) ? "" : Path.Combine(sdkHome, ".android", "debug.keystore"),
                string.IsNullOrEmpty(home) ? "" : Path.Combine(home, ".android", "debug.keystore")
            };
        }

        /// <summary>Java's keytool: from the JDK in Unity's Android module, the JDK set in Preferences, or JAVA_HOME.</summary>
        private static string FindKeytool()
        {
            string exe = Application.platform == RuntimePlatform.WindowsEditor ? "keytool.exe" : "keytool";
            string engine = "";
            try
            {
                engine = BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.Android, BuildOptions.None);
            }
            catch (Exception)
            {
                engine = "";
            }
            var roots = new[]
            {
                string.IsNullOrEmpty(engine) ? "" : Path.Combine(engine, "OpenJDK"),
                EditorPrefs.GetString("JdkPath", ""),
                Environment.GetEnvironmentVariable("JAVA_HOME") ?? ""
            };
            foreach (var root in roots)
            {
                if (string.IsNullOrEmpty(root)) continue;
                string path = Path.Combine(root, "bin", exe);
                if (File.Exists(path)) return path;
            }
            return null;
        }

        private static string Quote(string s)
        {
            return "\"" + s + "\"";
        }

        private static int Run(string exe, string args, out string output)
        {
            output = "";
            try
            {
                var info = new ProcessStartInfo(exe, args)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                using (var p = Process.Start(info))
                {
                    var err = p.StandardError.ReadToEndAsync();
                    string text = p.StandardOutput.ReadToEnd();
                    if (!p.WaitForExit(60000))
                    {
                        try
                        {
                            p.Kill();
                        }
                        catch (Exception)
                        {
                        }
                        output = text;
                        return -1;
                    }
                    output = (text + " " + err.Result).Trim();
                    return p.ExitCode;
                }
            }
            catch (Exception e)
            {
                output = e.Message;
                return -1;
            }
        }

        [MenuItem("Pokemon TCG/Show Android Signing Key")]
        public static void ShowKey()
        {
            string dir = FolderPath;
            if (Directory.Exists(dir) && Directory.GetFiles(dir, "*.keystore").Length > 0)
                EditorUtility.RevealInFinder(Directory.GetFiles(dir, "*.keystore")[0]);
            else
                EditorUtility.DisplayDialog("Android signing key", "There's no signing key yet. The first Android build makes one in " + dir
                                            + " (or copies the debug key earlier builds used).", "OK");
        }
    }
}
