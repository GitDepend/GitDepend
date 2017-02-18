using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Commands;
using GitDepend.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Commands
{
    [TestFixture]
    public class InitCommandTests : TestFixtureBase
    {
        [Test]
        public void Execute_ShouldReturnError_WhenGitDependFileCannotBeLoaded()
        {
            var factory = Container.Resolve<IGitDependFileFactory>();
            string dir;
            ReturnCode loadCode = ReturnCode.GitRepositoryNotFound;
            factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode))
                .Returns(null as GitDependFile);

            var options = new InitSubOptions();
            var instance = new InitCommand(options);
            var code = instance.Execute();
            Assert.AreEqual(ReturnCode.GitRepositoryNotFound, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldPromptForConfigFileValues()
        {
            var factory = Container.Resolve<IGitDependFileFactory>();
            var console = Container.Resolve<IConsole>();
            var fileSystem = Container.Resolve<IFileSystem>();

            string dir = Lib1Directory;
            ReturnCode loadCode = ReturnCode.Success;
            factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode))
                .Returns(Lib1Config);

            fileSystem.Arrange(f => f.File.WriteAllText(Arg.AnyString, Arg.AnyString))
                .MustBeCalled();

            int index = 0;
            string[] responses = {
                "buildall.bat",
                "Nuget\\Debug"
            };
            console.Arrange(c => c.ReadLine())
                .Returns(() => responses[index++]);
            
            var options = new InitSubOptions();
            var instance = new InitCommand(options);
            var code = instance.Execute();
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            fileSystem.Assert("WriteAllText should have been caleld");
            Assert.AreEqual(responses[0], Lib1Config.Build.Script, "Invalid Build Script");
            Assert.AreEqual(responses[1], Lib1Config.Packages.Directory, "Invalid Packages Directory");
        }

        [Test]
        public void Execute_ShouldUseDefaultValues_WhenNoInputIsGiven()
        {
            var factory = Container.Resolve<IGitDependFileFactory>();
            var console = Container.Resolve<IConsole>();
            var fileSystem = Container.Resolve<IFileSystem>();

            var config = new GitDependFile();
            string dir = Lib1Directory;
            ReturnCode loadCode = ReturnCode.Success;
            factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode))
                .Returns(config);

            fileSystem.Arrange(f => f.File.WriteAllText(Arg.AnyString, Arg.AnyString))
                .MustBeCalled();

            int index = 0;
            string[] responses = {
                "",
                ""
            };
            console.Arrange(c => c.ReadLine())
                .Returns(() => responses[index++]);

            var options = new InitSubOptions();
            var instance = new InitCommand(options);
            var code = instance.Execute();
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            fileSystem.Assert("WriteAllText should have been caleld");
            Assert.AreEqual("make.bat", config.Build.Script, "Invalid Build Script");
            Assert.AreEqual("artifacts/NuGet/Debug", config.Packages.Directory, "Invalid Packages Directory");
        }
    }
}
