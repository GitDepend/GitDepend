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
        public void Execute_ReturnsError_WhenVerifyCorrectBranchVisitor_Fails()
        {
            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();
            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    Assert.IsNotNull(visitor as VerifyCorrectBranchVisitor, "The first visitor should be of type VerifyCorrectBranchVisitor");
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
                    if (visitor is VerifyCorrectBranchVisitor)
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
            string[] packages =
            {
                "TestPackage.0.1.2",
                "TestPackage.Busi.3.1.2"
            };

            bool checkoutCalled = false;
            bool checkArtifacts = false;
            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();
            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    if (visitor is VerifyCorrectBranchVisitor)
                    {
                        checkoutCalled = true;
                        visitor.ReturnCode = ReturnCode.Success;
                        return;
                    }

                    if (visitor is CheckArtifactsVisitor)
                    {
                        checkArtifacts = true;
                        visitor.ReturnCode = ReturnCode.Success;
                        return;
                    }

                    Assert.IsTrue(visitor is BuildAndUpdateDependenciesVisitor);

                    foreach (var package in packages)
                    {
                        ((BuildAndUpdateDependenciesVisitor) visitor).UpdatedPackages.Add(package);
                    }
                    
                    visitor.ReturnCode = ReturnCode.Success;
                });

            var console = Container.Resolve<IConsole>();

            StringBuilder output = new StringBuilder();
            console.Arrange(c => c.WriteLine(Arg.AnyString))
                .DoInstead((string text) =>
                {
                    output.AppendLine(text);
                });

            var expected = "Updated packages: " + Environment.NewLine +
                           "    " + string.Join(Environment.NewLine + "    ", packages) + Environment.NewLine +
                           "Update complete!" + Environment.NewLine;


            var options = new UpdateSubOptions();
            var instance = new UpdateCommand(options);

            var code = instance.Execute();

            Assert.IsTrue(checkoutCalled, "Dependencies should have been checked out first.");
            Assert.IsTrue(checkArtifacts, "Artifacts should have been checked");
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual(expected, output.ToString());
        }
    }
}
