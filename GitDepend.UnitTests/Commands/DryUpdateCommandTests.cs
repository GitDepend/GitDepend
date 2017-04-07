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
	public class DryUpdateCommandTests : TestFixtureBase
	{
		[Test]
		public void Execute_ReturnsError_WhenVerifyCorrectBranchVisitor_Fails()
		{
			var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();
			algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString))
				.DoInstead((IVisitor visitor, string directory) =>
				{
					Assert.IsNotNull(visitor as VerifyCorrectBranchDryVisitor, "The first visitor should be of type VerifyCorrectBranchDryVisitor");
					visitor.ReturnCode = ReturnCode.FailedToRunGitCommand;
				});


			var options = new UpdateSubOptions();
			var instance = new DryUpdateCommand(options);

			var code = instance.Execute();

			Assert.AreEqual(ReturnCode.FailedToRunGitCommand, code, "Invalid Return Code");
		}

		[Test]
		public void Execute_ReturnsAllUpToDate_WhenAllAreUpToDate()
		{
			var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();
			var console = Container.Resolve<IConsole>();
			algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString))
				.DoInstead((IVisitor visitor, string directory) =>
				{

				});

			StringBuilder output = new StringBuilder();
			console.Arrange(c => c.WriteLine(Arg.AnyString, Arg.IsAny<object[]>()))
				.DoInstead((string format, object[] args) =>
				{
					output.AppendLine(string.Format(format, args));
				});

			var options = new UpdateSubOptions();
			var instance = new DryUpdateCommand(options);

			var code = instance.Execute();

			string expected = "Branch Changes:\r\n"
			                  + "\tAll dependencies on the correct branch\r\n"
			                  + "Dependencies to build:\r\n"
			                  + "\tAll packages are up to date\r\n"
							  + "Projects that would need to update:\r\n"
			                  + "\tAll packages are up to date\r\n";

			Assert.AreEqual(expected, output.ToString(), "Output different than expected.");
		}

		[Test]
		public void Execute_ReturnsPossibleUpdates_WhenUpdatesAreNeeded()
		{
			var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();
			var console = Container.Resolve<IConsole>();
			algorithm.Arrange(a => a.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString))
				.DoInstead((IVisitor visitor, string directory) =>
				{
					if (visitor is VerifyCorrectBranchDryVisitor)
					{
						((VerifyCorrectBranchDryVisitor)visitor).Changes.Add("Would change the branch file in repo1 from branch b1 to branch b2");
						((VerifyCorrectBranchDryVisitor)visitor).Changes.Add("Would change the config file in repo1 for dependency dep1 from branch b1 to branch b2");
					}
					else if (visitor is CheckArtifactsVisitor)
					{
						((CheckArtifactsVisitor) visitor).DependenciesThatNeedBuilding.Add("core");
						((CheckArtifactsVisitor)visitor).DependenciesThatNeedBuilding.Add("claims.estimate");
						((CheckArtifactsVisitor)visitor).ProjectsThatNeedNugetUpdate.Add("core");
						((CheckArtifactsVisitor)visitor).ProjectsThatNeedNugetUpdate.Add("claims.estimate");
					}
				});

			StringBuilder output = new StringBuilder();
			console.Arrange(c => c.WriteLine(Arg.AnyString, Arg.IsAny<object[]>()))
				.DoInstead((string format, object[] args) =>
				{
					output.AppendLine(string.Format(format, args));
				});

			var options = new UpdateSubOptions();
			var instance = new DryUpdateCommand(options);

			var code = instance.Execute();

			string expected = "Branch Changes:\r\n"
			                  + "\tWould change the branch file in repo1 from branch b1 to branch b2\r\n"
			                  + "\tWould change the config file in repo1 for dependency dep1 from branch b1 to branch b2\r\n"
			                  + "Dependencies to build:\r\n"
			                  + "\tcore\r\n"
			                  + "\tclaims.estimate\r\n"
			                  + "Projects that would need to update:\r\n"
			                  + "\tcore\r\n"
			                  + "\tclaims.estimate\r\n";

			Assert.AreEqual(expected, output.ToString(), "Output different than expected.");
		}
	}
}
