using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Busi
{
    [TestFixture]
    public class GitDependFileFactoryTests : TestFixtureBase
    {
        [Test]
        public void LoadFromDirectory_ShoulReturn_DirectoryDoesNotExist_WhenSpecifiedDirectoryDoesNotExist()
        {
            var instance = new GitDependFileFactory();

            string directory = @"C:\projects\DoesNotExit";
            string dir;
            ReturnCode code;
            var config = instance.LoadFromDirectory(directory, out dir, out code);

            Assert.IsNull(config, "Config file should be null");
            Assert.IsNull(dir, "The found directory should be null");
            Assert.AreEqual(ReturnCode.DirectoryDoesNotExist, code, "Invalid Return Code");
        }

        [Test]
        public void LoadFromDirectory_ShoulReturn_GitRepositoryNotFound_NoGitDirectoryExists()
        {
            var fileSystem = RegisterMockFileSystem();
            var instance = new GitDependFileFactory();

            string directory = @"C:\projects\GitDepend";
            fileSystem.Directory.CreateDirectory(directory);

            string dir;
            ReturnCode code;
            var config = instance.LoadFromDirectory(directory, out dir, out code);

            Assert.IsNull(config, "Config file should be null");
            Assert.IsNull(dir, "The found directory should be null");
            Assert.AreEqual(ReturnCode.GitRepositoryNotFound, code, "Invalid Return Code");
        }

        [Test]
        public void LoadFromDirectory_ShoulReturnEmptyFile_WhenNoGitDependFileExistsInGitRepo()
        {
            var fileSystem = RegisterMockFileSystem();
            var instance = new GitDependFileFactory();

            string directory = @"C:\projects\GitDepend";
            fileSystem.Directory.CreateDirectory(directory);
            fileSystem.Directory.CreateDirectory(fileSystem.Path.Combine(directory, ".git"));

            string dir;
            ReturnCode code;
            var config = instance.LoadFromDirectory(directory, out dir, out code);
            var expected = new GitDependFile();

            Assert.IsNotNull(config, "Config file should not be null");
            Assert.AreEqual(directory, dir, "Invalid directory");
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual(expected.ToString(), config.ToString(), "File does not match");
        }

        [Test]
        public void LoadFromDirectory_ShoulReturnExistingFile_WhenGitDependFileExistsInGitRepo()
        {
            var fileSystem = RegisterMockFileSystem();
            var instance = new GitDependFileFactory();

            string directory = @"C:\projects\GitDepend";
            fileSystem.Directory.CreateDirectory(directory);
            fileSystem.Directory.CreateDirectory(fileSystem.Path.Combine(directory, ".git"));
            var path = fileSystem.Path.Combine(directory, "GitDepend.json");
            fileSystem.File.WriteAllText(path, Lib2Config.ToString());

            string dir;
            ReturnCode code;
            var config = instance.LoadFromDirectory(directory, out dir, out code);

            Assert.IsNotNull(config, "Config file should not be null");
            Assert.AreEqual(directory, dir, "Invalid directory");
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual(Lib2Config.ToString(), config.ToString(), "File does not match");
        }

        [Test]
        public void LoadFromDirectory_ShoulReturn_UnknownError_WhenRandomErrorIsThrown()
        {
            var fileSysetm = Container.Resolve<IFileSystem>();
            var instance = new GitDependFileFactory();
            string directory = @"C:\projects\GitDepend";

            fileSysetm.Directory.Arrange(d => d.Exists(Arg.AnyString))
                .Returns(true);
            fileSysetm.Directory.Arrange(d => d.GetDirectories(Arg.AnyString, Arg.AnyString))
                .Returns(new[] { @"C:\projects\GitDepend\.git" });
            fileSysetm.File.Arrange(f => f.Exists(Arg.AnyString))
                .Returns(true);
            fileSysetm.File.Arrange(f => f.ReadAllText(Arg.AnyString))
                .Throws<Exception>();

            string dir;
            ReturnCode code;
            var config = instance.LoadFromDirectory(directory, out dir, out code);

            Assert.IsNull(config, "Config file should be null");
            Assert.IsNull(dir, "Invalid directory");
            Assert.AreEqual(ReturnCode.UnknownError, code, "Invalid Return Code");
        }
    }
}
