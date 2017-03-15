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
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var lib2Dir = Path.Combine(path, Lib2Name);
            var lib1Dir = Path.Combine(path, Lib2Name);
            Clone(Lib2Url, lib2Dir);

            var info = GitDepend("clone", lib2Dir);

            Assert.AreEqual(ReturnCode.Success, info.ReturnCode);
            Assert.IsTrue(Directory.Exists(lib2Dir));
            Assert.IsTrue(Directory.Exists(lib1Dir));
        }
    }
}