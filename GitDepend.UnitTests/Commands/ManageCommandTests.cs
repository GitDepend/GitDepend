using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Commands;
using GitDepend.Visitors;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Commands
{
    [TestFixture]
    public class ManageCommandTests : TestFixtureBase
    {
        private IDependencyVisitorAlgorithm _algorithm;
        private IGitDependFileFactory _factory;
        private IFileSystem _fileSystem;

        [SetUp]
        public void Setup()
        {
            _algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
        }

        private ManageSubOptions SucceedSetup()
        {
            string dir;
            ReturnCode returnCode;
            _fileSystem.Arrange(x => x.Path.Combine(Arg.AnyString, Arg.AnyString)).Returns("C:\\projects\\Lib1").MustBeCalled();
            _fileSystem.Arrange(x => x.Path.GetFullPath("C:\\projects\\Lib1")).Returns("C:\\projects\\Lib1").MustBeCalled();
            _algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString, false)).DoInstead(
                (ManageDependenciesVisitor visitor, string directory) =>
                {
                    visitor.NameMatchingDirectory = Lib1Directory;
                    visitor.ReturnCode = ReturnCode.Success;
                }).MustBeCalled();
            _factory.Arrange(x => x.LoadFromDirectory(Arg.AnyString, out dir, out returnCode)).Returns(Lib2Config).MustBeCalled();

            var options = new ManageSubOptions();
            options.Name = Lib1Config.Name;
            options.Directory = Lib2Directory;
            return options;
        }

        [Test]
        public void ManageCommand_ShouldSucceed_OnlyBranch()
        {
            SucceedSetup();
            var options = new ManageSubOptions();
            options.SetBranch = "newBranchName";

            var manageCommand = new ManageCommand(options);

            var code = manageCommand.Execute();

            Assert.AreEqual(ReturnCode.Success, code);
            Assert.AreEqual(Lib1Dependency.Branch, "newBranchName");
        }

        [Test]
        public void ManageCommand_ShouldSucceed_OnlyUrl()
        {
            var options = SucceedSetup();
            options.SetUrl = "newUrl";

            var manageCommand = new ManageCommand(options);
            var code = manageCommand.Execute();

            AssertSetup();
            Assert.AreEqual(ReturnCode.Success, code);
            Assert.AreEqual(Lib1Dependency.Url, "newUrl");
        }

        

        [Test]
        public void ManageCommand_ShouldSucceed_OnlyDirectory()
        {
            var options = SucceedSetup();
            options.SetDirectory = "newDirectory";

            var manageCommand = new ManageCommand(options);
            var code = manageCommand.Execute();

            AssertSetup();
            Assert.AreEqual(ReturnCode.Success, code);
            Assert.AreEqual(Lib1Dependency.Directory, "newDirectory");

        }

        [Test]
        public void ManageCommand_ShouldSucceed_AllArguments()
        {
            var options = SucceedSetup();
            options.SetDirectory = "newDirectory";
            options.SetUrl = "newurl";
            options.SetBranch = "newBranch";

            var manageCommand = new ManageCommand(options);
            var code = manageCommand.Execute();

            AssertSetup();
            Assert.AreEqual(ReturnCode.Success, code);
            Assert.AreEqual(Lib1Dependency.Directory, "newDirectory");
            Assert.AreEqual(Lib1Dependency.Url, "newurl");
            Assert.AreEqual(Lib1Dependency.Branch, "newBranch");
        }

        [Test]
        public void ManageCommand_Fails_WhenOnlyNameGiven()
        {
            var options = SucceedSetup();
            
            var manageCommand = new ManageCommand(options);
            var code = manageCommand.Execute();

            AssertSetup();
            Assert.AreEqual(ReturnCode.NameDidNotMatchRequestedDependency, code);

        }

        private void AssertSetup()
        {
            _fileSystem.Assert("Combine and GetFullPath must be called");
            _factory.Assert("LoadFromDirectory must be called");
            _algorithm.Assert("TraverseDependencies must be called");
        }
    }
}
