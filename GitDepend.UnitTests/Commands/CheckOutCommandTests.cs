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
    public class CheckOutCommandTests : TestFixtureBase
    {
        [Test]
        public void Execute_ShouldReturn_InvalidArguments_WhenNoBranchNameIsPresent()
        {
            var options = new CheckOutSubOptions();
            var instance = new CheckOutCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.InvalidArguments, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_Error_WhenTraverseDependencies_Fails()
        {
            const string BRANCH_NAME = "feature/testing_2";

            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();

            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.FailedToRunGitCommand;
                })
                .MustBeCalled();

            var options = new CheckOutSubOptions
            {
                BranchName = BRANCH_NAME
            };
            var instance = new CheckOutCommand(options);
            var code = instance.Execute();

            algorithm.Assert();
            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_Success_WhenTraverseDependencies_Succeeds()
        {
            const string BRANCH_NAME = "feature/testing_2";

            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();

            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.Success;
                })
                .MustBeCalled();

            var options = new CheckOutSubOptions
            {
                BranchName = BRANCH_NAME,
                CreateBranch = true
            };
            var instance = new CheckOutCommand(options);
            var code = instance.Execute();

            algorithm.Assert();
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
        }
    }
}
