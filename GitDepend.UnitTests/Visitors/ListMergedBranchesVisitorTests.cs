using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.Visitors;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Visitors
{
    [TestFixture]
    public class ListMergedBranchesVisitorTests : TestFixtureBase
    {
        [Test]
        public void VisitDependency_ShouldReturn_Error_WhenListMergedBranches_Fails()
        {
            const string WORKING_DIR = @"C:\test\dir";

            var fileSystem = Container.Resolve<IFileSystem>();
            var git = Container.Resolve<IGit>();

            fileSystem.Arrange(f => f.Path.GetFullPath(Arg.AnyString))
                .Returns(WORKING_DIR);

            git.Arrange(g => g.ListMergedBranches())
                .Returns(ReturnCode.FailedToRunGitCommand)
                .MustBeCalled();

            var instance = new ListMergedBranchesVisitor();
            var code = instance.VisitDependency(Lib2Directory, Lib1Dependency);
            git.Assert();
            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code, "Invalid Return Code");
            Assert.AreEqual(WORKING_DIR, git.WorkingDirectory, "Invalid working directory");
        }

        [Test]
        public void VisitDependency_ShouldReturn_Success_WhenListMergedBranches_Succeeds()
        {
            const string WORKING_DIR = @"C:\test\dir";

            var fileSystem = Container.Resolve<IFileSystem>();
            var git = Container.Resolve<IGit>();

            fileSystem.Arrange(f => f.Path.GetFullPath(Arg.AnyString))
                .Returns(WORKING_DIR);

            git.Arrange(g => g.ListMergedBranches())
                .Returns(ReturnCode.Success)
                .MustBeCalled();

            var instance = new ListMergedBranchesVisitor();
            var code = instance.VisitDependency(Lib2Directory, Lib1Dependency);
            git.Assert();
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual(WORKING_DIR, git.WorkingDirectory, "Invalid working directory");
        }

        [Test]
        public void VisitProject_ShouldReturn_Success()
        {
            var instance = new ListMergedBranchesVisitor();
            var code = instance.VisitProject(Lib2Directory, Lib2Config);
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
        }
    }
}
