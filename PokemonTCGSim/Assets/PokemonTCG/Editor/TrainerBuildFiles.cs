using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace PokemonTCG.EditorTools
{
    /// <summary>
    /// After a Windows build, copies what training outside the game needs into the build's folder, so a build you give someone
    /// can train in the background and on their graphics card too:
    ///   Tools/sim-train/sim-train.exe   the background trainer ("Train in the background")
    ///   TrainerData/                    its card data, decklists and starting network (a build packs Resources into Unity's
    ///                                   own files, which the trainer can't read; it looks here instead)
    ///   GpuLearner/                     the GPU learner and start_gpu_learner.bat, which sets up its Python packages the first
    ///                                   time and starts it (needs Python and an NVIDIA graphics card; without it, training
    ///                                   learns on the CPU)
    /// Each player's AI and decks stay in their own save folder (Application.persistentDataPath), not here.
    /// </summary>
    public class TrainerBuildFiles : IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPostprocessBuild(BuildReport report)
        {
            var target = report.summary.platform;
            if (target != BuildTarget.StandaloneWindows && target != BuildTarget.StandaloneWindows64) return;
            string build = Path.GetDirectoryName(report.summary.outputPath);
            string project = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string resources = Path.Combine(Application.dataPath, "PokemonTCG", "Resources");
            try
            {
                int n = 0;
                n += CopyFile(Path.Combine(project, "Tools", "sim-train", "sim-train.exe"), Path.Combine(build, "Tools", "sim-train", "sim-train.exe"));
                string data = Path.Combine(build, "TrainerData");
                n += CopyFolder(Path.Combine(resources, "CardData"), Path.Combine(data, "CardData"), "*.json");
                n += CopyFolder(Path.Combine(resources, "CardPrintings"), Path.Combine(data, "CardPrintings"), "*.json");
                n += CopyFolder(Path.Combine(resources, "Decklists"), Path.Combine(data, "Decklists"), "*.txt");
                n += CopyFile(Path.Combine(resources, "AI", "policy_start.bytes"), Path.Combine(data, "AI", "policy_start.bytes"));
                foreach (var name in new[] { "learner.py", "README.md", "start_gpu_learner.bat" })
                    n += CopyFile(Path.Combine(project, "GpuLearner", name), Path.Combine(build, "GpuLearner", name));
                Debug.Log("[PokemonTCG] Copied " + n + " files for the background trainer and the GPU learner next to the build (" + build + ").");
            }
            catch (Exception e)
            {
                Debug.LogError("[PokemonTCG] Couldn't copy the background trainer's files next to the build: " + e.Message);
            }
        }

        private static int CopyFile(string from, string to)
        {
            if (!File.Exists(from))
            {
                Debug.LogWarning("[PokemonTCG] Not in the project, so not in the build: " + from);
                return 0;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(to));
            File.Copy(from, to, true);
            return 1;
        }

        private static int CopyFolder(string from, string to, string pattern)
        {
            if (!Directory.Exists(from))
            {
                Debug.LogWarning("[PokemonTCG] Not in the project, so not in the build: " + from);
                return 0;
            }
            int n = 0;
            foreach (var f in Directory.GetFiles(from, pattern)) n += CopyFile(f, Path.Combine(to, Path.GetFileName(f)));
            return n;
        }
    }
}
