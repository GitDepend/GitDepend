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
    public class DeleteBranchVisitorTests : TestFixtureBase
    {
        [Test]
        public void VisitDependency_ShouldReturn_Success()
        {
            const string BRANCH = "feature/my_branch";
            const bool FORCE = false;

            var instance = new DeleteBranchVisitor(BRANCH, FORCE);
            var code = instance.VisitDependency(Lib2Directory, Lib1Dependency);

            Assert.AreEqual(BRANCH, instance.BranchName, "Invalid branch name");
            Assert.AreEqual(FORCE, instance.Force, "Invalid force flag");
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
        }

        [Test]
        public void VisitProject_ShouldReturn_Success_WhenDeleteBranch_Fails()
        {
            const string BRANCH = "feature/my_branch";
            const bool FORCE = false;

            var git = Container.Resolve<IGit>();

            git.Arrange(g => g.DeleteBranch(Arg.AnyString, Arg.AnyBool))
                .Returns(ReturnCode.FailedToRunGitCommand)
                .MustBeCalled();

            var instance = new DeleteBranchVisitor(BRANCH, FORCE);
            var code = instance.VisitProject(Lib2Directory, Lib2Config);
            git.Assert();

            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code, "Invalid Return Code");
            Assert.AreEqual(Lib2Directory, git.WorkingDirectory, "Invalid working directory");
            Assert.AreEqual(BRANCH, instance.BranchName, "Invalid branch name");
            Assert.AreEqual(FORCE, instance.Force, "Invalid force flag");
        }

        [Test]
        public void VisitProject_ShouldReturn_Success_WhenCreateBranch_Succeeds()
        {
            const string BRANCH = "feature/my_branch";

            var git = Container.Resolve<IGit>();

            git.Arrange(g => g.CreateBranch(Arg.AnyString))
                .Returns(ReturnCode.Success)
                .MustBeCalled();

            var instance = new CreateBranchVisitor(BRANCH);
            var code = instance.VisitProject(Lib2Directory, Lib2Config);
            git.Assert();
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual(Lib2Directory, git.WorkingDirectory, "Invalid working directory");
        }
    }
}
