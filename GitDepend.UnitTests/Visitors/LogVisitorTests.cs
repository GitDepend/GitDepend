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
    public class LogVisitorTests : TestFixtureBase
    {
        private IGit _git;

        [SetUp]
        public void Setup()
        {
            _git = DependencyInjection.Resolve<IGit>();
        }

        [Test]
        public void LogVisitor_Succeeds_WhenPullSucceeds()
        {
            _git.Arrange(x => x.Log("")).Returns(ReturnCode.Success);

            var dependencies = new List<string>();
            LogVisitor visitor = new LogVisitor("", dependencies);

            var returnCode = visitor.VisitProject(Lib1Directory, new GitDependFile());

            Assert.AreEqual(ReturnCode.Success, returnCode);

        }

        [Test]
        public void LogVisitor_Succeeds_WhenPullFails()
        {
            _git.Arrange(x => x.Log("")).Returns(ReturnCode.FailedToRunGitCommand);

            var dependencies = new List<string>();
            LogVisitor visitor = new LogVisitor("", dependencies);

            var returnCode = visitor.VisitProject(Lib1Directory, new GitDependFile()
            {
                Name = "name"
            });

            Assert.AreEqual(ReturnCode.Success, returnCode);
        }

        [Test]
        public void LogVisitor_Fails_OtherThanFailedToRunGitCommand()
        {
            _git.Arrange(x => x.Log("")).Returns(ReturnCode.InvalidCommand);

            LogVisitor visitor = new LogVisitor("", new List<string>());

            var returnCode = visitor.VisitProject(Lib1Directory, new GitDependFile()
            {
                Name = "name"
            });

            Assert.AreNotEqual(ReturnCode.Success, returnCode);
        }

        [Test]
        public void LogVisitor_NullArguments_ShouldStillSucceed()
        {
            _git.Arrange(x => x.Log("")).Returns(ReturnCode.Success);

            List<string> arguments = null;
            var visitor = new LogVisitor("", new List<string>());
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
        public void LogVisitor_VisitDependency_ShouldReturnSuccess()
        {
            List<string> arguments = null;
            var visitor = new LogVisitor("", new List<string>());
            var returnCode = visitor.VisitProject(Lib1Directory, new GitDependFile());

            Assert.AreEqual(ReturnCode.Success, returnCode);
        }

    }
}
