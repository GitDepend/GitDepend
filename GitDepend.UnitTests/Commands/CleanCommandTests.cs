using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Commands;
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

        [SetUp]
        public void Setup()
        {
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
            ReturnCode gitReturnCode = ReturnCode.Success;
            git.Arrange(x => x.Clean(false, false, false, false, "dir")).Returns(ReturnCode.FailedToRunGitCommand).MustBeCalled();
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



    }
}
