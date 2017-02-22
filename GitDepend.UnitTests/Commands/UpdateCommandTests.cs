using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
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
    public class UpdateCommandTests : TestFixtureBase
    {
        [Test]
        public void Execute_ReturnsError_WhenCheckOutBranchVisitor_Fails()
        {
            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();
            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    Assert.IsNotNull(visitor as CheckOutBranchVisitor, "The first visitor should be of type CheckOutBranchVisitor");
                    visitor.ReturnCode = ReturnCode.FailedToRunGitCommand;
                });


            var options = new UpdateSubOptions();
            var instance = new UpdateCommand(options);

            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ReturnsError_WhenBuildAndUpdateDependenciesVisitor_Fails()
        {
            bool checkoutCalled = false;
            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();
            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    if (visitor is CheckOutBranchVisitor)
                    {
                        checkoutCalled = true;
                        visitor.ReturnCode = ReturnCode.Success;
                        return;
                    }
                    
                    visitor.ReturnCode = ReturnCode.FailedToRunBuildScript;
                });


            var options = new UpdateSubOptions();
            var instance = new UpdateCommand(options);

            var code = instance.Execute();

            Assert.IsTrue(checkoutCalled, "Dependencies should have been checked out first.");
            Assert.AreEqual(ReturnCode.FailedToRunBuildScript, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ReturnsSucccess_WhenBuildAndUpdateDependenciesVisitor_Succeeds()
        {
            bool checkoutCalled = false;
            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();
            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    if (visitor is CheckOutBranchVisitor)
                    {
                        checkoutCalled = true;
                        visitor.ReturnCode = ReturnCode.Success;
                        return;
                    }

                    Assert.IsTrue(visitor is BuildAndUpdateDependenciesVisitor);
                    visitor.ReturnCode = ReturnCode.Success;
                });

            var console = Container.Resolve<IConsole>();

            StringBuilder output = new StringBuilder();
            console.Arrange(c => c.WriteLine(Arg.AnyString))
                .DoInstead((string text) =>
                {
                    output.AppendLine(text);
                });


            var options = new UpdateSubOptions();
            var instance = new UpdateCommand(options);

            var code = instance.Execute();

            Assert.IsTrue(checkoutCalled, "Dependencies should have been checked out first.");
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual("Update complete!" + Environment.NewLine, output.ToString());
        }
    }
}
