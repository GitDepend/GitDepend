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
            _git.Arrange(x => x.Pull(Arg.IsAny<IList<string>>())).Returns(ReturnCode.Success);

            var arguments = new List<string>();
            PullBranchVisitor visitor = new PullBranchVisitor(arguments, new List<string>());

            var returnCode = visitor.VisitProject(Lib1Directory, new GitDependFile());

            Assert.AreEqual(ReturnCode.Success, returnCode);

        }

        [Test]
        public void PullBranchVisitor_Succeeds_WhenPullFails()
        {
            _git.Arrange(x => x.Pull(Arg.IsAny<IList<string>>())).Returns(ReturnCode.FailedToRunGitCommand);

            var arguments = new List<string>();
            PullBranchVisitor visitor = new PullBranchVisitor(arguments, new List<string>());

            var returnCode = visitor.VisitProject(Lib1Directory, new GitDependFile());

            Assert.AreEqual(ReturnCode.Success, returnCode);
        }

        [Test]
        public void PullBranchVisitor_Fails_OtherThanFailedToRunGitCommand()
        {
            _git.Arrange(x => x.Pull(Arg.IsAny<IList<string>>())).Returns(ReturnCode.InvalidCommand);

            var arguments = new List<string>();
            PullBranchVisitor visitor = new PullBranchVisitor(arguments, new List<string>());

            var returnCode = visitor.VisitProject(Lib1Directory, new GitDependFile());

            Assert.AreNotEqual(ReturnCode.Success, returnCode);
        }
    }
}
