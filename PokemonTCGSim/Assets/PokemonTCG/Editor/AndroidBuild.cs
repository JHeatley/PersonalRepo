using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PokemonTCG.Presentation;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PokemonTCG.EditorTools
{
    /// <summary>
    /// v118: one click from the menu bar (Pokemon TCG > Build Android APK) builds an APK to side-load on an Android phone or
    /// tablet: it switches the project to Android, sets the player settings a side-loaded build needs (IL2CPP, ARM64, landscape,
    /// a package name, the project's signing key and a new version code (v118.3, see SideloadSigning), writes the card image list,
    /// and builds Builds/Android/PokemonTCG.apk next to the Assets folder.
    ///
    /// Command line (no Editor window):
    ///   Unity -batchmode -quit -projectPath (project) -executeMethod PokemonTCG.EditorTools.AndroidBuild.BuildFromCommandLine
    /// </summary>
    public static class AndroidBuild
    {
        public const string OutputFolder = "Builds/Android";
        public const string ApkName = "PokemonTCG.apk";
        /// <summary>The app's package name when the project still has Unity's placeholder one.</summary>
        public const string DefaultPackageName = "com.fanproject.pokemontcgengine";
        private const string BoardScenePath = "Assets/PokemonTCG/Scenes/Board.unity";

        [MenuItem("Pokemon TCG/Build Android APK (side-load)")]
        public static void BuildFromMenu()
        {
            Build(true);
        }

        /// <summary>For -executeMethod: exits with code 1 when the build fails.</summary>
        public static void BuildFromCommandLine()
        {
            if (!Build(false)) EditorApplication.Exit(1);
        }

        private static bool Build(bool interactive)
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
                return Fail("Android Build Support isn't installed for this Unity version. In Unity Hub: Installs > this version > Add modules > "
                            + "Android Build Support (tick OpenJDK and Android SDK & NDK Tools too), then try again.", interactive);
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android
                && !EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
                return Fail("Unity couldn't switch the project to Android (File > Build Profiles shows why).", interactive);

            ConfigurePlayerSettings();
            // v118.3: the same key and a higher version every build, so the APK installs over the last one as an update.
            string signing = SideloadSigning.Apply();
            int versionCode = SideloadSigning.BumpVersionCode();
            Debug.Log("[PokemonTCG] Android build: version code " + versionCode + ", " + signing + ".");
            CardImageList.Write();
            var scenes = ScenesToBuild();
            if (scenes.Length == 0) return Fail("There's no scene to build.", interactive);

            Directory.CreateDirectory(OutputFolder);
            string path = Path.Combine(OutputFolder, ApkName);
            EditorUserBuildSettings.buildAppBundle = false; // an .apk to side-load, not a Play Store bundle
            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = path,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.None
            };
            // v118.1: the Android C++ compiler (clang, in the NDK) can crash on IL2CPP's generated code (an access violation,
            // 0xC0000005, on Windows). Each step below changes what the compiler is given; between tries the half-built C++
            // files are deleted. The first try uses smaller IL2CPP code, which avoids most of these crashes by itself.
            BuildReport report = null;
            for (int attempt = 0; attempt < CompilerFallbacks.Length; attempt++)
            {
                CompilerFallbacks[attempt].Apply();
                Debug.Log("[PokemonTCG] Android build, try " + (attempt + 1) + ": " + CompilerFallbacks[attempt].Description + ".");
                report = BuildPipeline.BuildPlayer(options);
                if (report.summary.result == BuildResult.Succeeded) break;
                if (report.summary.result == BuildResult.Cancelled || !CompilerCrashed(report) || attempt == CompilerFallbacks.Length - 1) break;
                Debug.LogWarning("[PokemonTCG] The Android C++ compiler crashed (a compiler problem, not the game's code). Deleting the "
                                 + "half-built files and trying again with " + CompilerFallbacks[attempt + 1].Description + ".");
                DeleteNativeBuildCache();
            }
            if (report == null || report.summary.result != BuildResult.Succeeded)
            {
                string why = report != null && CompilerCrashed(report)
                    ? " The Android C++ compiler (clang) crashed on every try. Try deleting the project's Library/Bee folder, letting your "
                      + "antivirus skip the project's Library folder and the Unity install folder, and reinstalling Android Build Support in "
                      + "Unity Hub; if clang fails on the same file every time, send that Console text along."
                    : "";
                return Fail("The Android build didn't finish (" + (report != null ? report.summary.result + ", " + report.summary.totalErrors + " error(s)" : "no report")
                            + "); the Console has the details." + why, interactive);
            }

            string full = Path.GetFullPath(path);
            double mb = report.summary.totalSize / (1024.0 * 1024.0);
            Debug.Log("[PokemonTCG] Android APK built: " + full + " (" + mb.ToString("0.0") + " MB). Copy it to the phone and open it to install "
                      + "(allow installing from that app when Android asks), or run: adb install -r \"" + full + "\"");
            if (interactive)
            {
                EditorUtility.RevealInFinder(full);
                EditorUtility.DisplayDialog("Android APK built", "Built " + full + " (" + mb.ToString("0.0") + " MB, version code " + versionCode + ").\n\nCopy it to your "
                                            + "Android device and open it to install, or use adb install -r. If an earlier build is installed, Android asks to "
                                            + "update it (your saved decks and the AI's brain stay).", "OK");
            }
            return true;
        }

        /// <summary>One way of setting up IL2CPP's C++ code (v118.1).</summary>
        private sealed class CompilerSetup
        {
            public string Description;
            public Il2CppCodeGeneration CodeGeneration;
            public Il2CppCompilerConfiguration Configuration;

            public void Apply()
            {
                PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.Android, CodeGeneration);
                PlayerSettings.SetIl2CppCompilerConfiguration(NamedBuildTarget.Android, Configuration);
            }
        }

        /// <summary>
        /// The tries, in order: smaller generated code (IL2CPP shares more generic code: less C++ to compile, a faster build, a
        /// little slower at run time), then the same without the C++ optimizer (it's the optimizer that crashes), then the
        /// standard settings. The settings of the build that worked stay set.
        /// </summary>
        private static readonly CompilerSetup[] CompilerFallbacks =
        {
            new CompilerSetup { Description = "smaller IL2CPP code (Faster (smaller) builds), Release C++", CodeGeneration = Il2CppCodeGeneration.OptimizeSize, Configuration = Il2CppCompilerConfiguration.Release },
            new CompilerSetup { Description = "smaller IL2CPP code, Debug C++ (no optimizer)", CodeGeneration = Il2CppCodeGeneration.OptimizeSize, Configuration = Il2CppCompilerConfiguration.Debug },
            new CompilerSetup { Description = "standard IL2CPP code (Faster runtime), Release C++", CodeGeneration = Il2CppCodeGeneration.OptimizeSpeed, Configuration = Il2CppCompilerConfiguration.Release }
        };

        /// <summary>The build failed because clang (the C++ compiler) crashed, not because of an error in the code.</summary>
        private static bool CompilerCrashed(BuildReport report)
        {
            foreach (var step in report.steps)
                foreach (var message in step.messages)
                {
                    string text = message.content ?? "";
                    if (text.Contains("clang frontend command failed") || text.Contains("PLEASE submit a bug report") || text.Contains("0xC0000005")
                        || (text.Contains("clang++") && text.Contains("Stack dump"))) return true;
                }
            return false;
        }

        /// <summary>Deletes the half-built Android C++ files, so the next try compiles everything fresh.</summary>
        private static void DeleteNativeBuildCache()
        {
            foreach (var dir in new[] { "Library/Bee/artifacts/Android", "Library/Il2cppBuildCache" })
            {
                try
                {
                    if (Directory.Exists(dir)) Directory.Delete(dir, true);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("[PokemonTCG] Couldn't delete " + dir + ": " + e.Message);
                }
            }
        }

        private static bool Fail(string message, bool interactive)
        {
            Debug.LogError("[PokemonTCG] " + message);
            if (interactive) EditorUtility.DisplayDialog("Android build", message, "OK");
            return false;
        }

        /// <summary>The player settings a side-loaded Android build needs. Settings you changed yourself (package name, minimum Android) are kept.</summary>
        public static void ConfigurePlayerSettings()
        {
            var target = NamedBuildTarget.Android;
            // 64-bit ARM is what current phones run; IL2CPP is the only way Unity builds it.
            PlayerSettings.SetScriptingBackend(target, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            // The game uses no reflection, but light stripping keeps the build safe for the card scripts' registry.
            PlayerSettings.SetManagedStrippingLevel(target, ManagedStrippingLevel.Minimal);
            if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel24)
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            string id = PlayerSettings.GetApplicationIdentifier(target);
            if (string.IsNullOrEmpty(id) || id.StartsWith("com.DefaultCompany", StringComparison.OrdinalIgnoreCase)
                || id.StartsWith("com.Company", StringComparison.OrdinalIgnoreCase) || id == "com.unity.template")
                PlayerSettings.SetApplicationIdentifier(target, DefaultPackageName);
            // The board is laid out for a wide screen: landscape only, either way round.
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            // Signing: SideloadSigning (v118.3) sets the project's own key every build.
            // Nothing in the game uses the internet.
            PlayerSettings.Android.forceInternetPermission = false;
        }

        /// <summary>The enabled scenes in the build list; the board scene (made if missing) when the list is empty.</summary>
        private static string[] ScenesToBuild()
        {
            var listed = EditorBuildSettings.scenes.Where(s => s.enabled && File.Exists(s.path)).Select(s => s.path).ToArray();
            if (listed.Length > 0) return listed;
            if (!File.Exists(BoardScenePath))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                new GameObject("Pokemon TCG Game").AddComponent<GameBootstrap>();
                if (!AssetDatabase.IsValidFolder("Assets/PokemonTCG/Scenes")) AssetDatabase.CreateFolder("Assets/PokemonTCG", "Scenes");
                EditorSceneManager.SaveScene(scene, BoardScenePath);
            }
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(BoardScenePath, true) };
            return new[] { BoardScenePath };
        }
    }

    /// <summary>
    /// v118: writes StreamingAssets/CardImages/_image_list.txt (every image's path relative to CardImages) before every build.
    /// An Android app can't list the files inside its own package, so the game reads this list there instead.
    /// </summary>
    public sealed class CardImageList : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildReport report)
        {
            Write();
        }

        public static void Write()
        {
            string dir = Path.Combine(Application.dataPath, "StreamingAssets", "CardImages");
            if (!Directory.Exists(dir)) return;
            string listPath = Path.Combine(dir, CardVisuals.ImageListFileName);
            ConvertAvifAndWebp(dir);
            var sb = new StringBuilder();
            int count = 0;
            foreach (var file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories).OrderBy(f => f, StringComparer.Ordinal))
            {
                string name = Path.GetFileName(file);
                string ext = Path.GetExtension(file).ToLowerInvariant();
                if (name.StartsWith(".") || name == CardVisuals.ImageListFileName || (ext != ".png" && ext != ".jpg" && ext != ".jpeg")) continue;
                string rel = file.Substring(dir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Replace('\\', '/');
                sb.Append(rel).Append('\n');
                count++;
            }
            File.WriteAllText(listPath, sb.ToString(), new UTF8Encoding(false));
            Debug.Log("[PokemonTCG] Card image list: " + count + " image(s) in " + listPath + ".");
        }

        /// <summary>
        /// v153: a phone or tablet can't convert AVIF or WebP images (PokéBeach's are AVIF), so before a build each one gets a PNG
        /// copy next to it (made once, with the computer's converters; see ImageConverter). The game uses the PNG.
        /// </summary>
        private static void ConvertAvifAndWebp(string dir)
        {
            var pairs = new List<KeyValuePair<string, string>>();
            foreach (var file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
            {
                if (!ImageConverter.IsConvertible(Path.GetExtension(file))) continue;
                string stem = Path.Combine(Path.GetDirectoryName(file) ?? "", Path.GetFileNameWithoutExtension(file));
                if (File.Exists(stem + ".jpg") || File.Exists(stem + ".jpeg")) continue;
                pairs.Add(new KeyValuePair<string, string>(file, stem + ".png"));
            }
            if (pairs.Count == 0) return;
            var problems = new List<string>();
            var done = ImageConverter.ConvertAll(pairs, problems);
            Debug.Log("[PokemonTCG] AVIF/WebP card images: " + done.Count + " of " + pairs.Count + " have a PNG copy for the build.");
            foreach (var problem in problems) Debug.LogWarning("[PokemonTCG] " + problem);
        }
    }
}
