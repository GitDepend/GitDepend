using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using NUnit.Framework;

namespace GitDepend.IntegrationTests.Scenarios
{
    /// <summary>
    /// Integration tests that target the help page.
    /// </summary>
    public class HelpPageScenario : TestFixtureBase
    {
        private string _path;
        private string _lib2Dir;
        private string _lib1Dir;

        [TestFixtureSetUp]
        public void CloneLib2Repo()
        {
            _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            //Clone Repo
            _lib2Dir = Path.Combine(_path, Lib2Name);
            _lib1Dir = Path.Combine(_path, Lib1Name);
            Clone(Lib2Url, _lib2Dir);
        }

        [TearDown]
        public void CleanUpLib1Directory()
        {
            SafeDeleteDirectory(_lib1Dir);
        }

        [TestFixtureTearDown]
        public void CleanUpDirectory()
        {
            SafeDeleteDirectory(_path);
        }

        [Test]
        public void HelpVerb_Returns_Success()
        {
            var info = GitDepend("help");

            Assert.AreEqual(ReturnCode.Success, info.ReturnCode);
        }

        [Test]
        public void InvalidVerb_Returns_InvalidCommand()
        {
            var info = GitDepend("somebadcommand");

            Assert.AreEqual(ReturnCode.InvalidCommand, info.ReturnCode);
        }

        [Test]
        public void CloneVerb_ShouldCloneAllRepos()
        {
            var info = GitDepend("clone", _lib2Dir);

            var lib1Exists = Directory.Exists(_lib1Dir);
            var lib2Exists = Directory.Exists(_lib2Dir);


            Assert.AreEqual(ReturnCode.Success, info.ReturnCode, "Return Code was Wrong");
            Assert.IsTrue(lib1Exists, "Lib1 Doesn't Exists");
            Assert.IsTrue(lib2Exists, "Lib2 Doesn't Exists");
        }


        [Test]
        public void UpdateVerb_CloneReposChangeLib1_ShouldUpdateArtifacts()
        {
            //Clone Verb
            var cloneVerbInfo = GitDepend("clone", _lib2Dir);

            var lib1Exists = Directory.Exists(_lib1Dir);
            var lib2Exists = Directory.Exists(_lib2Dir);

            Assert.AreEqual(ReturnCode.Success, cloneVerbInfo.ReturnCode, "Return Code was Wrong");
            Assert.IsTrue(lib1Exists, "Lib1 Doesn't Exists");
            Assert.IsTrue(lib2Exists, "Lib2 Doesn't Exists");
            
            //Change Lib1
            var updated = UpdateFile(_lib1Dir, $"{Lib1Name}\\StringUtils.cs", "TestData\\NewStringUtils.txt");

            //Update Verb
            var updateVerbInfo = GitDepend("update", _lib2Dir);

            //Check For new Artifacts from Lib1
            var lib1ArtifactsExists = File.Exists(Path.Combine(_lib1Dir, "artifacts/NuGet/Debug/Lib1.0.1.0.nupkg"));

            //Clean up
            SafeDeleteDirectory(_lib1Dir);

            Assert.IsTrue(updated, "Unable to update file");
            Assert.AreEqual(ReturnCode.Success, updateVerbInfo.ReturnCode, $"Return Code was Wrong - Error: {updateVerbInfo.StandardError}");
            Assert.IsTrue(lib1ArtifactsExists, "Artifacts don't exists");
        }

    }
}