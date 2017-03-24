using System;
using System.Collections;
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
    public class PullBranchVisitorTests : TestFixtureBase
    {
        private IGit _git;

        [SetUp]
        public void Setup()
        {
            _git = DependencyInjection.Resolve<IGit>();
        }

        [Test]
        public void PullBranchVisitor_Succeeds_WhenPullSucceeds()
        {
            _git.Arrange(x => x.Pull()).Returns(ReturnCode.Success);

            var arguments = new List<string>();
            PullBranchVisitor visitor = new PullBranchVisitor(new List<string>());

            var returnCode = visitor.VisitProject(Lib1Directory, new GitDependFile());

            Assert.AreEqual(ReturnCode.Success, returnCode);

        }

        [Test]
        public void PullBranchVisitor_Succeeds_WhenPullFails()
        {
            _git.Arrange(x => x.Pull()).Returns(ReturnCode.FailedToRunGitCommand);

            var arguments = new List<string>();
            PullBranchVisitor visitor = new PullBranchVisitor(new List<string>());

            var returnCode = visitor.VisitProject(Lib1Directory, new GitDependFile());

            Assert.AreEqual(ReturnCode.Success, returnCode);
        }

        [Test]
        public void PullBranchVisitor_Fails_OtherThanFailedToRunGitCommand()
        {
            _git.Arrange(x => x.Pull()).Returns(ReturnCode.InvalidCommand);

            PullBranchVisitor visitor = new PullBranchVisitor(new List<string>());

            var returnCode = visitor.VisitDependency(Lib1Directory, new Dependency()
            {
                Configuration = new GitDependFile()
                {
                    Name = "name"
                }
            });

            Assert.AreNotEqual(ReturnCode.Success, returnCode);
        }

        [Test]
        public void PullBranchVisitor_NullArguments_ShouldStillSucceed()
        {
            _git.Arrange(x => x.Pull()).Returns(ReturnCode.Success);

            List<string> arguments = null;
            var visitor = new PullBranchVisitor(new List<string>());
            var returnCode = visitor.VisitDependency(Lib1Directory, new Dependency()
            {
                Configuration = new GitDependFile()
                {
                    Name = "name"
                }
            });

            Assert.AreEqual(ReturnCode.Success, returnCode);
        }

        [Test]
        public void PullBranchVisitor_VisitDependency_ShouldReturnSuccess()
        {
            List<string> arguments = null;
            var visitor = new PullBranchVisitor(new List<string>());
            var returnCode = visitor.VisitProject(Lib1Directory, new GitDependFile());

            Assert.AreEqual(ReturnCode.Success, returnCode);
        }


    }
}
