using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class DisplayStatusVisitorTests : TestFixtureBase
    {
        [Test]
        public void VisitDependency_ShouldReturn_Success_WhenWhitelistDoesNotIncludeDependencyName()
        {
            var git = Container.Resolve<IGit>();

            git.Arrange(g => g.Status())
                .OccursNever("git.Status() should not have been called");

            IList<string> whilelist = new List<string>() { "No_EXISTE!" };
            var instance = new DisplayStatusVisitor(whilelist);

            var code = instance.VisitDependency(Lib2Directory, Lib1Dependency);
            Assert.AreEqual(ReturnCode.Success, code, "Invalid ReturnCode");
            Assert.AreEqual(ReturnCode.Success, instance.ReturnCode, "Invalid ReturnCode");
        }

        [Test]
        public void VisitDependency_ShouldReturn_FailedToRunGitCommand_WhenGitStatusFails()
        {
            var git = Container.Resolve<IGit>();

            git.Arrange(g => g.Status())
                .Returns(ReturnCode.FailedToRunGitCommand);

            IList<string> whilelist = new List<string>() { Lib1Dependency.Configuration.Name, "OTHER" };
            var instance = new DisplayStatusVisitor(whilelist);

            var code = instance.VisitDependency(Lib2Directory, Lib1Dependency);
            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code, "Invalid ReturnCode");
            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, instance.ReturnCode, "Invalid ReturnCode");
        }

        [Test]
        public void VisitDependency_ShouldReturn_Success_WhenGitStatusSucceeds()
        {
            var git = Container.Resolve<IGit>();

            git.Arrange(g => g.Status())
                .Returns(ReturnCode.Success);

            IList<string> whilelist = new List<string>();
            var instance = new DisplayStatusVisitor(whilelist);

            var code = instance.VisitDependency(Lib2Directory, Lib1Dependency);
            Assert.AreEqual(ReturnCode.Success, code, "Invalid ReturnCode");
            Assert.AreEqual(ReturnCode.Success, instance.ReturnCode, "Invalid ReturnCode");
        }

        [Test]
        public void VisitProject_ShouldReturn_Success()
        {
            IList<string> whilelist = new List<string>();
            var instance = new DisplayStatusVisitor(whilelist);

            var code = instance.VisitProject(Lib2Directory, Lib2Config);
            Assert.AreEqual(ReturnCode.Success, code, "Invalid ReturnCode");
            Assert.AreEqual(ReturnCode.Success, instance.ReturnCode, "Invalid ReturnCode");
        }
    }
}
