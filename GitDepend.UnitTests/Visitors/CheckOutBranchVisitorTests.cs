using System.IO.Abstractions.TestingHelpers;
using GitDepend.Busi;
using GitDepend.Configuration;
using GitDepend.Visitors;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Visitors
{
	[TestFixture]
	public class CheckOutBranchVisitorTests : TestFixtureBase
	{
		private IGit _git;
		private MockFileSystem _fileSystem;
		private IConsole _console;
		private CheckOutBranchVisitor _instance;

		[SetUp]
		public void SetUp()
		{
			_git = Mock.Create<IGit>();
			_fileSystem = new MockFileSystem();
			_console = Mock.Create<IConsole>();
			_instance = new CheckOutBranchVisitor(_git, _fileSystem, _console);
		}

		[Test]
		public void VisitDependency_ShouldCallGitCloneOnCorrectDirectory()
		{
			string checkedOutDirectory = string.Empty;
			string checkedOutBranch = string.Empty;

			_git.Arrange(g => g.Checkout(Arg.AnyString))
				.Returns((string branch) =>
				{
					checkedOutDirectory = _git.WorkingDirectory;
					checkedOutBranch = branch;
					return ReturnCode.Success;
				});

			var code = _instance.VisitDependency(Lib2Directory, Lib1Dependency);
			Assert.AreEqual(ReturnCode.Success, code, "Invalid code returned from VisitDependency");
			Assert.AreEqual(ReturnCode.Success, _instance.ReturnCode, "Invalid Return Code");
			Assert.AreEqual(Lib1Dependency.Branch, checkedOutBranch, "Invalid branch checked out");
			Assert.AreEqual(Lib1Directory, checkedOutDirectory, "Invalid directory checked out.");
		}

		[Test]
		public void VisitDependency_ShouldReturn_FailedToRunGitCommand_WhenGitCheckoutFails()
		{
			_git.Arrange(g => g.Checkout(Arg.AnyString))
				.Returns(ReturnCode.FailedToRunGitCommand);

			var code = _instance.VisitDependency(Lib2Directory, Lib1Dependency);
			Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code, "Invalid code returned from VisitDependency");
			Assert.AreEqual(ReturnCode.FailedToRunGitCommand, _instance.ReturnCode, "Invalid Return Code");
		}

		[Test]
		public void VisitProject_ShouldReturn_Success()
		{
			var code = _instance.VisitProject(Lib2Directory, Lib2Config);
			Assert.AreEqual(ReturnCode.Success, code, "Invalid code returned from VisitProject");
			Assert.AreEqual(ReturnCode.Success, _instance.ReturnCode, "Invalid Return Code");
		}
	}
}
