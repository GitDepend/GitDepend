using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter.Xml;
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
        private IGitDependFileFactory _factory;
        private const string GitDependConfigFile = "GitDepend.json";

        [SetUp]
        public void AddCommandTestsSetup()
        {
            _fileSystem = RegisterMockFileSystem();
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            EnsureDirectory(_fileSystem, Lib1Directory);
            EnsureDirectory(_fileSystem, Lib2Directory);
            EnsureFiles(_fileSystem, Lib2Directory, new List<string>()
            {
                GitDependConfigFile
            });
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
            string dir;
            ReturnCode returnCode;
            var dependFile = _factory.LoadFromDirectory(options.Directory, out dir, out returnCode);
            
            Assert.AreEqual(ReturnCode.Success, code);
            Assert.IsNotNull(dependFile.Dependencies);
            Assert.AreEqual(1, dependFile.Dependencies.Count);

            var newDependency = dependFile.Dependencies.FirstOrDefault(x => x.Directory == options.DependencyDirectory);

            Assert.IsNotNull(newDependency);
            Assert.AreEqual(newDependency.Directory, options.DependencyDirectory);
            Assert.AreEqual(newDependency.Branch, options.Branch);
            Assert.AreEqual(newDependency.Url, options.Url);

        }

        [Test]
        public void AddCommand_ExistingDirectory_ShouldNotAdd()
        {
            
            string dir = null;
            ReturnCode loadCode;
            _factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode)).Returns(Lib2Config).MustBeCalled();

            var options = new AddSubOptions()
            {
                Directory = Lib2Directory,
                Branch = "develop",
                DependencyDirectory = "..\\Lib1",
            };

            var command = new AddCommand(options);

            var code = command.Execute();

            Assert.AreEqual(ReturnCode.DependencyAlreadyExists, code);
            _factory.Assert("LoadFromDirectory should have been called");
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
