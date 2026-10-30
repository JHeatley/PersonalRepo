using System.IO;
using NUnit.Framework;
using PokemonTCG.AI;

namespace PokemonTCG.Tests
{
    /// <summary>Saves survive a crash: the old file or the new one is always there, and a save left only as .tmp is picked up.</summary>
    public class SafeFileTests
    {
        private string dir;

        [SetUp]
        public void MakeFolder()
        {
            dir = Path.Combine(Path.GetTempPath(), "safefile-" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
        }

        [TearDown]
        public void RemoveFolder()
        {
            if (Directory.Exists(dir)) Directory.Delete(dir, true);
        }

        [Test]
        public void Writing_Replaces_The_File_And_Leaves_No_Temp_File()
        {
            string path = Path.Combine(dir, "brain.txt");
            SafeFile.WriteText(path, "first");
            SafeFile.WriteText(path, "second");
            Assert.AreEqual("second", File.ReadAllText(path));
            Assert.IsFalse(File.Exists(path + ".tmp"));
        }

        [Test]
        public void A_Save_Left_Only_As_Temp_By_A_Crash_Is_Recovered()
        {
            string path = Path.Combine(dir, "brain.txt");
            File.WriteAllText(path + ".tmp", "saved just before the crash");
            SafeFile.Recover(path);
            Assert.AreEqual("saved just before the crash", File.ReadAllText(path));

            // With the file there, a leftover temp file (a save the crash cut short) is ignored.
            File.WriteAllText(path + ".tmp", "half");
            SafeFile.Recover(path);
            Assert.AreEqual("saved just before the crash", File.ReadAllText(path));
        }

        [Test]
        public void A_Brain_Whose_Files_A_Crash_Left_As_Temp_Files_Loads_Them()
        {
            string path = Path.Combine(dir, "brain.txt");
            var brain = new LearningBrain { FilePath = path };
            brain.GamesTrained = 1234;
            brain.Save();
            string neural = LearningBrain.NeuralPath(path);
            Assert.IsTrue(File.Exists(path) && File.Exists(neural));
            File.Move(path, path + ".tmp");
            File.Move(neural, neural + ".tmp");

            var back = LearningBrain.Load(path);
            Assert.AreEqual(1234, back.GamesTrained);
            Assert.IsNull(back.PolicyLoadError);
            Assert.IsTrue(File.Exists(path) && File.Exists(neural));
        }
    }
}
