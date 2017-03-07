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
    public class AddCommandTests : TestFixtureBase
    {
        private IFileSystem _fileSystem;
        private const string GitDependConfigFile = "GitDepend.json";

        [SetUp]
        public void AddCommandTestsSetup()
        {
            _fileSystem = RegisterMockFileSystem();
            EnsureDirectory(_fileSystem, Lib1Directory);
            EnsureDirectory(_fileSystem, Lib2Directory);
            EnsureFiles(_fileSystem, Lib2Directory, new List<string>()
            {
                GitDependConfigFile
            });
            var tuple = new Tuple<string, GitDependFile>(GitDependConfigFile, Lib2Config);
            WriteFile(_fileSystem, Lib2Directory, tuple);
        }


        [Test]
        public void AddCommand_Succeeds()
        {
            var options = new AddSubOptions()
            {
                Directory = Lib2Directory,
                Branch = "master",
                DependencyDirectory = "..\\lib3",
                Url = "https://test-url.com/repos/lib3"
            };

            var command = new AddCommand(options);

            var code = command.Execute();

            Assert.AreEqual(ReturnCode.Success, code);
        }

        [Test]
        public void AddCommand_ExistingDirectory_ShouldNotAdd()
        {
            var factory = Container.Resolve<IGitDependFileFactory>();
            string dir = null;
            ReturnCode loadCode;
            factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode)).Returns(Lib2Config);

            var options = new AddSubOptions()
            {
                Directory = Lib2Directory,
                Branch = "develop",
                DependencyDirectory = "..\\Lib1",
            };

            var command = new AddCommand(options);

            var code = command.Execute();

            Assert.AreEqual(ReturnCode.DependencyAlreadyExists, code);
        }

        [Test]
        public void AddCommand_ExistingDirectory_ShouldBeCaseInsensitive()
        {
            var factory = Container.Resolve<IGitDependFileFactory>();
            string dir = null;
            ReturnCode loadCode;
            factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode)).Returns(Lib2Config);

            var options = new AddSubOptions()
            {
                Directory = Lib2Directory,
                Branch = "develop",
                DependencyDirectory = "..\\lib1",
            };

            var command = new AddCommand(options);
            var code = command.Execute();

            Assert.AreEqual(ReturnCode.DependencyAlreadyExists, code);
        }


    }
}
