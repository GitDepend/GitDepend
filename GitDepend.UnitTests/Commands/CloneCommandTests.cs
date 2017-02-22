using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class CloneCommandTests : TestFixtureBase
    {
        [Test]
        public void Execute_ShouldReturnError_WhenTraverseDependencies_Fails()
        {
            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();

            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.FailedToRunGitCommand;
                });

            var options = new CloneSubOptions();
            var instance = new CloneCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_Success_WhenTraverseDependencies_Succeeds()
        {
            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();

            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.Success;
                });

            var options = new CloneSubOptions();
            var instance = new CloneCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
        }
    }
}
