using GitDepend.Configuration;
using GitDepend.Visitors;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Visitors
{
	[TestFixture]
	public class CheckOutBranchVisitorTests
	{
		private IGit _git;
		private CheckOutBranchVisitor _instance;
		private Dependency _dependency;

		[SetUp]
		public void SetUp()
		{
			_git = Mock.Create<IGit>();
			_dependency = new Dependency
			{
				Name = "Lib2",
				Directory = @"C:\projects\Lib2",
				Url = "git@github.com:kjjuno/Lib2.git",
				Branch = "develop"
			};
			_instance = new CheckOutBranchVisitor(_git);
		}

		[Test]
		public void VisitDependency_ShouldCallGitCloneOnCorrectDirectory()
		{
			string checkedOutDirectory = string.Empty;
			string checkedOutBranch = string.Empty;

			_git.Arrange(g => g.Checkout(Arg.AnyString))
				.Returns((string branch) =>
				{
					checkedOutDirectory = _git.WorkingDir;
					checkedOutBranch = branch;
					return ReturnCode.Success;
				});

			var code = _instance.VisitDependency(_dependency);
			Assert.AreEqual(ReturnCode.Success, code, "Invalid code returned from VisitDependency");
			Assert.AreEqual(ReturnCode.Success, _instance.ReturnCode, "Invalid Return Code");
			Assert.AreEqual(_dependency.Branch, checkedOutBranch, "Invalid branch checked out");
			Assert.AreEqual(_dependency.Directory, checkedOutDirectory, "Invalid directory checked out.");
		}

		[Test]
		public void VisitDependency_ShouldReturn_FailedToRunGitCommand_WhenGitCheckoutFails()
		{
			_git.Arrange(g => g.Checkout(Arg.AnyString))
				.Returns(ReturnCode.FailedToRunGitCommand);

			var code = _instance.VisitDependency(_dependency);
			Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code, "Invalid code returned from VisitDependency");
			Assert.AreEqual(ReturnCode.FailedToRunGitCommand, _instance.ReturnCode, "Invalid Return Code");
		}

		[Test]
		public void VisitProject_ShouldReturn_Success()
		{
			string directory = @"C:\projects\GitDepend";
			var code = _instance.VisitProject(directory, new GitDependFile());
			Assert.AreEqual(ReturnCode.Success, code, "Invalid code returned from VisitProject");
			Assert.AreEqual(ReturnCode.Success, _instance.ReturnCode, "Invalid Return Code");
		}
	}
}
