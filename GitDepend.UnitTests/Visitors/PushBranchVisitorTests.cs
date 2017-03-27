using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.Configuration;
using GitDepend.Visitors;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Visitors
{
    [TestFixture]
    public class PushBranchVisitorTests : TestFixtureBase
    {
        private IGit _git;

        [SetUp]
        public void Setup()
        {
            _git = DependencyInjection.Resolve<IGit>();
        }

        [Test]
        public void PushBranchVisitor_ShouldSucceed()
        {
            var whitelist = new List<string>();
            var pushArguments = new List<string>();

            _git.Arrange(x => x.Push()).Returns(ReturnCode.Success);

            var visitor = new PushBranchVisitor(whitelist);

            var code = visitor.VisitDependency("dep", new Dependency()
            {
                Configuration = new GitDependFile()
                {
                    Name = "name"
                }
            });

            Assert.AreEqual(ReturnCode.Success, code);
        }

        [Test]
        public void PushBranchVisitor_ShouldSucceed_WhenGitFails()
        {
            var whitelist = new List<string>();
            var pushArguments = new List<string>();

            _git.Arrange(x => x.Push()).Returns(ReturnCode.FailedToRunGitCommand);

            var visitor = new PushBranchVisitor(whitelist);

            var code = visitor.VisitDependency("dep", new Dependency()
            {
                Configuration = new GitDependFile()
                {
                    Name = "name"
                }
            });

            Assert.AreEqual(ReturnCode.Success, code);
        }
    }
}
