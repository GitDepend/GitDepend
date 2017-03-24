using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.Visitors;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Visitors
{
    [TestFixture]
    public class CheckOutBranchVisitorTests : TestFixtureBase
    {
        [Test]
        public void VisitDependency_ShouldReturn_Success()
        {
            const string BRANCH = "hotfix/my_branch";
            const bool CREATE = false;
            var instance = new CheckOutBranchVisitor(BRANCH, CREATE);
            var code = instance.VisitDependency(Lib2Directory, Lib1Dependency);

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual(ReturnCode.Success, instance.ReturnCode, "Invalid Return Code");
        }

        [Test]
        public void VisitProject_ShouldReturn_Success_WhenCheckout_Fails()
        {
            const string BRANCH = "hotfix/my_branch";
            const bool CREATE = false;

            var git = Container.Resolve<IGit>();

            git.Arrange(g => g.Checkout(BRANCH, CREATE))
                .Returns(ReturnCode.FailedToRunGitCommand)
                .MustBeCalled();

            var instance = new CheckOutBranchVisitor(BRANCH, CREATE);
            var code = instance.VisitProject(Lib2Directory, Lib2Config);

            git.Assert();
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual(ReturnCode.Success, instance.ReturnCode, "Invalid Return Code");
        }

        [Test]
        public void VisitProject_ShouldReturn_Success_WhenCheckout_Succeeds()
        {
            const string BRANCH = "hotfix/my_branch";
            const bool CREATE = false;

            var git = Container.Resolve<IGit>();

            git.Arrange(g => g.Checkout(BRANCH, CREATE))
                .Returns(ReturnCode.Success)
                .MustBeCalled();

            var instance = new CheckOutBranchVisitor(BRANCH, CREATE);
            var code = instance.VisitProject(Lib2Directory, Lib2Config);

            git.Assert();
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual(ReturnCode.Success, instance.ReturnCode, "Invalid Return Code");
        }
    }
}
