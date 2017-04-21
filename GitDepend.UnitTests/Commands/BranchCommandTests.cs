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
    public class BranchCommandTests : TestFixtureBase
    {
        [Test]
        public void Execute_ShouldReturnError_WhenTraverseDependencies_Fails()
        {
            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();

            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString, false))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.FailedToRunGitCommand;
                });

            var options = new BranchSubOptions();
            var instance = new BranchCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_InvalidArguments_WhenMultipleMutuallyExclusiveArgumentsArePresent()
        {
            var options = new BranchSubOptions()
            {
                Delete = true,
                ListMerged = true
            };
            var instance = new BranchCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.InvalidArguments, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_InvalidArguments_WhenForceDeleteIsPresentWithoutDelete()
        {
            var options = new BranchSubOptions()
            {
                Delete = false,
                ForceDelete = true
            };
            var instance = new BranchCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.InvalidArguments, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_InvalidArguments_WhenDeleteIsSpecifiedWithoutTheBranchName()
        {
            var options = new BranchSubOptions()
            {
                Delete = true,
                BranchName = string.Empty
            };
            var instance = new BranchCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.InvalidArguments, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_Error_WhenDeleteBranch_Fails()
        {
            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();

            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString, false))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    var deleteVisitor = visitor as DeleteBranchVisitor;
                    Assert.IsNotNull(deleteVisitor, "The visitor should be of type DeleteBranchVisitor");
                    visitor.ReturnCode = ReturnCode.FailedToRunGitCommand;
                });

            var options = new BranchSubOptions()
            {
                Delete = true,
                BranchName = "feature/test_branch"
            };
            var instance = new BranchCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_Success_WhenDeleteBranch_Succeeds()
        {
            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();

            var options = new BranchSubOptions()
            {
                Delete = true,
                ForceDelete = true,
                BranchName = "feature/test_branch"
            };

            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString, false))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    var deleteVisitor = visitor as DeleteBranchVisitor;
                    Assert.IsNotNull(deleteVisitor, "The visitor should be of type DeleteBranchVisitor");
                    Assert.AreEqual(deleteVisitor.BranchName, options.BranchName, "Invalid branch name");
                    Assert.AreEqual(deleteVisitor.Force, options.ForceDelete, "Invalid force flag");
                    visitor.ReturnCode = ReturnCode.Success;
                });

            
            var instance = new BranchCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_InvalidArguments_WhenListMergedIsSpecifiedWithABranchName()
        {
            var options = new BranchSubOptions()
            {
                ListMerged = true,
                BranchName = "feature/test_branch"
            };
            var instance = new BranchCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.InvalidArguments, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_Error_WhenListMerged_Fails()
        {
            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();

            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString, false))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    var mergedBranchesVisitor = visitor as ListMergedBranchesVisitor;
                    Assert.IsNotNull(mergedBranchesVisitor, "The visitor should be of type ListMergedBranchesVisitor");
                    visitor.ReturnCode = ReturnCode.FailedToRunGitCommand;
                });

            var options = new BranchSubOptions()
            {
                ListMerged = true
            };
            var instance = new BranchCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_Success_WhenListMerged_Succeeds()
        {
            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();

            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString, false))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    var mergedBranchesVisitor = visitor as ListMergedBranchesVisitor;
                    Assert.IsNotNull(mergedBranchesVisitor, "The visitor should be of type ListMergedBranchesVisitor");
                    visitor.ReturnCode = ReturnCode.Success;
                });

            var options = new BranchSubOptions()
            {
                ListMerged = true
            };
            var instance = new BranchCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_Error_WhenCreateBranch_Fails()
        {
            const string BRANCH_NAME = "feature/test_branch";

            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();

            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString, false))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    var createBranchVisitor = visitor as CreateBranchVisitor;
                    Assert.IsNotNull(createBranchVisitor, "The visitor should be of type CreateBranchVisitor");
                    Assert.AreEqual(BRANCH_NAME, createBranchVisitor.BranchName, "Invalid branch name");
                    visitor.ReturnCode = ReturnCode.FailedToRunGitCommand;
                });

            var options = new BranchSubOptions()
            {
                BranchName = BRANCH_NAME
            };
            var instance = new BranchCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_Success_WhenCreateBranch_Succeeds()
        {
            const string BRANCH_NAME = "feature/test_branch";

            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();

            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString, false))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    var createBranchVisitor = visitor as CreateBranchVisitor;
                    Assert.IsNotNull(createBranchVisitor, "The visitor should be of type CreateBranchVisitor");
                    Assert.AreEqual(BRANCH_NAME, createBranchVisitor.BranchName, "Invalid branch name");
                    visitor.ReturnCode = ReturnCode.Success;
                });

            var options = new BranchSubOptions()
            {
                BranchName = BRANCH_NAME
            };
            var instance = new BranchCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_Error_WhenListAllBranches_Fails()
        {
            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();

            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString, false))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    var listAllBranchesVisitor = visitor as ListAllBranchesVisitor;
                    Assert.IsNotNull(listAllBranchesVisitor, "The visitor should be of type ListAllBranchesVisitor");
                    visitor.ReturnCode = ReturnCode.FailedToRunGitCommand;
                });

            var options = new BranchSubOptions();
            var instance = new BranchCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_Success_WhenListAllBranches_Succeeds()
        {
            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();

            algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString, false))
                .DoInstead((IVisitor visitor, string directory) =>
                {
                    var listAllBranchesVisitor = visitor as ListAllBranchesVisitor;
                    Assert.IsNotNull(listAllBranchesVisitor, "The visitor should be of type ListAllBranchesVisitor");
                    visitor.ReturnCode = ReturnCode.Success;
                });

            var options = new BranchSubOptions();
            var instance = new BranchCommand(options);
            var code = instance.Execute();

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
        }
    }
}
