using System.IO.Abstractions.TestingHelpers;
using GitDepend.Busi;
using GitDepend.Configuration;
using GitDepend.Visitors;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Visitors
{
    [TestFixture]
    public class CheckOutDependencyBranchVisitorTests : TestFixtureBase
    {
        [Test]
        public void VisitDependency_ShouldCallGitCloneOnCorrectDirectory()
        {
            var git = Container.Resolve<IGit>();
            RegisterMockFileSystem();
            var instance = new CheckOutDependencyBranchVisitor();

            string checkedOutDirectory = string.Empty;
            string checkedOutBranch = string.Empty;

            git.Arrange(g => g.Checkout(Arg.AnyString, Arg.AnyBool))
                .Returns((string branch, bool create) =>
                {
                    checkedOutDirectory = git.WorkingDirectory;
                    checkedOutBranch = branch;
                    return ReturnCode.Success;
                });

            var code = instance.VisitDependency(Lib2Directory, Lib1Dependency);

            Assert.AreEqual(ReturnCode.Success, code, "Invalid code returned from VisitDependency");
            Assert.AreEqual(ReturnCode.Success, instance.ReturnCode, "Invalid Return Code");
            Assert.AreEqual(Lib1Dependency.Branch, checkedOutBranch, "Invalid branch checked out");
            Assert.AreEqual(Lib1Directory, checkedOutDirectory, "Invalid directory checked out.");
        }

        [Test]
        public void VisitDependency_ShouldReturn_FailedToRunGitCommand_WhenGitCheckoutFails()
        {
            var git = Container.Resolve<IGit>();
            var instance = new CheckOutDependencyBranchVisitor();

            git.Arrange(g => g.Checkout(Arg.AnyString, Arg.AnyBool))
                .Returns(ReturnCode.FailedToRunGitCommand);

            var code = instance.VisitDependency(Lib2Directory, Lib1Dependency);
            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code, "Invalid code returned from VisitDependency");
            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, instance.ReturnCode, "Invalid Return Code");
        }

        [Test]
        public void VisitProject_ShouldReturn_Success()
        {
            var instance = new CheckOutDependencyBranchVisitor();
            var code = instance.VisitProject(Lib2Directory, Lib2Config);
            Assert.AreEqual(ReturnCode.Success, code, "Invalid code returned from VisitProject");
            Assert.AreEqual(ReturnCode.Success, instance.ReturnCode, "Invalid Return Code");
        }
    }
}
