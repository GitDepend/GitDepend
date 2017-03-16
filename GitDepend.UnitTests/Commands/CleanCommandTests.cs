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
using GitDepend.Visitors;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Commands
{
    [TestFixture]
    public class CleanCommandTests : TestFixtureBase
    {
        private CleanSubOptions _goodCleanSubOptions;
        private CleanSubOptions _badCleanSubOptions;
        private IGitDependFileFactory _gitDependFactory;
        private IFileSystem _fileSystem;

        [SetUp]
        public void Setup()
        {
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
            _badCleanSubOptions = new CleanSubOptions()
            {
                Directory = "dir",
                DryRun = false,
                Force = false,
                RemoveUntrackedFiles = false,
                RemoveUntrackedDirectories = false
            };

            _goodCleanSubOptions = new CleanSubOptions()
            {
                Directory = "dir",
                DryRun = true,
                Force = true,
                RemoveUntrackedFiles = true,
                RemoveUntrackedDirectories = true
            };
            string dir;
            ReturnCode code;
            _gitDependFactory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _gitDependFactory.Arrange(x => x.LoadFromDirectory("dir", out dir, out code)).Returns(() =>
            {
                dir = "dir";
                code = ReturnCode.Success;
                return new GitDependFile()
                {
                    Name = ""
                };
            });
        }

        [Test]
        public void CleanShouldSucceedWithDefaultOptions()
        {
            var options = new CleanSubOptions();
            var instance = new CleanCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.Success, code);
        }

        [Test]
        public void CleanCommand_ShouldExecute()
        {
            //setup IGit
            var algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
            algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString)).DoInstead((IVisitor visitor, string directory) =>
            {
                visitor.ReturnCode = ReturnCode.Success;
            }).MustBeCalled();

            var instance = new CleanCommand(_goodCleanSubOptions);

            var code = instance.Execute();

            algorithm.Assert("TraverseDependencies should have been called.");
            Assert.AreEqual(ReturnCode.Success, code);
        }

        [Test]
        public void CleanCommand_Fails_GitReturnCode_FailedToRun()
        {
            var git = DependencyInjection.Resolve<IGit>();
            git.Arrange(x => x.Clean(false, false, false, false)).Returns(ReturnCode.FailedToRunGitCommand).MustBeCalled();
            var algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
            algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString)).DoInstead(
                (IVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = visitor.VisitProject(directory, Lib1Config);
                }).MustBeCalled();

            var instance = new CleanCommand(_badCleanSubOptions);

            var code = instance.Execute();

            git.Assert("Clean should have been called");
            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code);
            algorithm.Assert("TraverseDependencies should have been called.");
        }


        [Test]
        public void CleanCommand_WithProject_Succeeds()
        {
            var git = DependencyInjection.Resolve<IGit>();
            git.Arrange(x => x.Clean(Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool))
                .Returns(ReturnCode.Success)
                .MustBeCalled();

            var algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
            algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString)).DoInstead(
                (IVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = visitor.VisitProject(directory, Lib1Config);
                }).MustBeCalled();
            string dir;
            ReturnCode returnCode;
            _gitDependFactory.Arrange(x => x.LoadFromDirectory(Arg.AnyString, out dir, out returnCode)).Returns(new GitDependFile()
            {
                Name = "name"
            });
            CleanSubOptions newOptions = _goodCleanSubOptions;
            newOptions.Dependencies = new List<string>()
            {
                "name"
            };

            var instance = new CleanCommand(newOptions);

            var code = instance.Execute();

            algorithm.Assert("TraverseDependencies should have been called");
            git.Assert("Clean should have been called");
            Assert.AreEqual(ReturnCode.Success, code);
        }
    }
}
