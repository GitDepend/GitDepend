using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Commands;
using GitDepend.Configuration;
using GitDepend.Visitors;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Visitors
{
	[TestFixture]
	public class VerifyCorrectBranchDryVisitorTests : TestFixtureBase
	{
		[Test]
		public void VisitDependancy_ReturnsUpToDate_WhenBranchesMatch()
		{
			IGit git = Container.Resolve<IGit>();
			git.Arrange(a => a.GetCurrentBranch()).DoInstead(() => { }).Returns("branch1");

			var console = Container.Resolve<IConsole>();
			StringBuilder output = new StringBuilder();
			console.Arrange(c => c.WriteLine(Arg.AnyString, Arg.IsAny<object[]>()))
				.DoInstead((string format, object[] args) =>
				{
					output.AppendLine(string.Format(format, args));
				});

			Dependency dep = new Dependency()
			{
				Directory = "dir",
				Url = "",
				Branch = "branch1",
				Configuration =  new GitDependFile()
				{
					Name = "name"
				}
			};

			var verifyVisitor = new VerifyCorrectBranchDryVisitor(new List<string>());
			verifyVisitor.VisitDependency("path", dep);

			string expected = "dependency:\r\n    name: name\r\n    dir: \r\nVerifying the checked out branch\r\neverything looks good.\r\n";
			string actual = output.ToString();
			Assert.AreEqual(expected, actual, "Output different than expected.");
			Assert.AreEqual(0, verifyVisitor.Changes.Count, "Expected zero changes.");
		}

		[Test]
		public void VisitDependancy_ConfigChange_WhenBranchesDiffer_InputOne()
		{
			IGit git = Container.Resolve<IGit>();
			git.Arrange(a => a.GetCurrentBranch()).DoInstead(() => { }).Returns("branch1");

			var console = Container.Resolve<IConsole>();
			StringBuilder output = new StringBuilder();
			console.Arrange(c => c.WriteLine(Arg.AnyString, Arg.IsAny<object[]>()))
				.DoInstead((string format, object[] args) =>
				{
					output.AppendLine(string.Format(format, args));
				});

			console.Arrange(c => c.ReadLine()).DoInstead(() => { }).Returns("1");

			Dependency dep = new Dependency()
			{
				Directory = "dir",
				Url = "",
				Branch = "branch2",
				Configuration = new GitDependFile()
				{
					Name = "depName"
				}
			};

			var verifyVisitor = new VerifyCorrectBranchDryVisitor(new List<string>());
			verifyVisitor.ParentRepoName = "Parent";
			verifyVisitor.VisitDependency("path", dep);

			string expected = "dependency:\r\n    name: depName\r\n    dir: \r\nVerifying the checked out branch\r\nInvalid Branch!\r\n" +
			                  "    expected branch2 but was branch1\r\nWhat should I do?\r\n1. Update config to point to branch1\r\n2. Switch branch to branch2\r\n3. Give up. I'll figure it out myself.\r\n";
			string actual = output.ToString();
			Assert.AreEqual(expected, actual, "Output different than expected.");
			Assert.AreEqual(1, verifyVisitor.Changes.Count, "Expected one change.");
			Assert.AreEqual("Would change the config file in Parent for dependency depName from branch branch2 to branch branch1", verifyVisitor.Changes[0], "Change list was not as expected.");
		}

		[Test]
		public void VisitDependancy_BranchChange_WhenBranchesDiffer_InputTwo()
		{
			IGit git = Container.Resolve<IGit>();
			git.Arrange(a => a.GetCurrentBranch()).DoInstead(() => { }).Returns("branch1");

			var console = Container.Resolve<IConsole>();
			StringBuilder output = new StringBuilder();
			console.Arrange(c => c.WriteLine(Arg.AnyString, Arg.IsAny<object[]>()))
				.DoInstead((string format, object[] args) =>
				{
					output.AppendLine(string.Format(format, args));
				});

			console.Arrange(c => c.ReadLine()).DoInstead(() => { }).Returns("2");

			Dependency dep = new Dependency()
			{
				Directory = "dir",
				Url = "",
				Branch = "branch2",
				Configuration = new GitDependFile()
				{
					Name = "depName"
				}
			};

			var verifyVisitor = new VerifyCorrectBranchDryVisitor(new List<string>());
			verifyVisitor.ParentRepoName = "Parent";
			verifyVisitor.VisitDependency("path", dep);

			string expected = "dependency:\r\n    name: depName\r\n    dir: \r\nVerifying the checked out branch\r\nInvalid Branch!\r\n" +
							  "    expected branch2 but was branch1\r\nWhat should I do?\r\n1. Update config to point to branch1\r\n2. Switch branch to branch2\r\n3. Give up. I'll figure it out myself.\r\n";
			string actual = output.ToString();
			Assert.AreEqual(expected, actual, "Output different than expected.");
			Assert.AreEqual(1, verifyVisitor.Changes.Count, "Expected one change.");
			Assert.AreEqual("Would change the branch in depName from branch branch1 to branch branch2", verifyVisitor.Changes[0], "Change list was not as expected.");
		}

		[Test]
		public void VisitDependancy_FixMismatch_WhenBranchesDiffer_InputThree()
		{
			IGit git = Container.Resolve<IGit>();
			git.Arrange(a => a.GetCurrentBranch()).DoInstead(() => { }).Returns("branch1");

			var console = Container.Resolve<IConsole>();
			StringBuilder output = new StringBuilder();
			console.Arrange(c => c.WriteLine(Arg.AnyString, Arg.IsAny<object[]>()))
				.DoInstead((string format, object[] args) =>
				{
					output.AppendLine(string.Format(format, args));
				});

			console.Arrange(c => c.ReadLine()).DoInstead(() => { }).Returns("3");

			Dependency dep = new Dependency()
			{
				Directory = "dir",
				Url = "",
				Branch = "branch2",
				Configuration = new GitDependFile()
				{
					Name = "depName"
				}
			};

			var verifyVisitor = new VerifyCorrectBranchDryVisitor(new List<string>());
			verifyVisitor.ParentRepoName = "Parent";
			verifyVisitor.VisitDependency("path", dep);

			string expected = "dependency:\r\n    name: depName\r\n    dir: \r\nVerifying the checked out branch\r\nInvalid Branch!\r\n" +
							  "    expected branch2 but was branch1\r\nWhat should I do?\r\n1. Update config to point to branch1\r\n2. Switch branch to branch2\r\n3. Give up. I'll figure it out myself.\r\n";
			string actual = output.ToString();
			Assert.AreEqual(expected, actual, "Output different than expected.");
			Assert.AreEqual(1, verifyVisitor.Changes.Count, "Expected one change.");
			Assert.AreEqual("Need to fix dependancy branch mismatch in Parent for depName", verifyVisitor.Changes[0], "Change list was not as expected.");
		}
	}
}
