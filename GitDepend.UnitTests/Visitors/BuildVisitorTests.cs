using System.Collections.Generic;
using System.Diagnostics;
using GitDepend.Busi;
using GitDepend.Configuration;
using GitDepend.Visitors;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Visitors
{
	[TestFixture]
	public class BuildVisitorTests : TestFixtureBase
	{
		[Test]
		public void VisitProject_ShouldReturn_DirectoryDoesNotExist_WhenDirectoryIsNull()
		{
			var instance = new BuildVisitor(string.Empty);
			var code = instance.VisitProject(null, Lib2Config);
			Assert.AreEqual(ReturnCode.DirectoryDoesNotExist, code, "Invalid Return Code");
		}

		[Test]
		public void VisitProject_ShouldReturn_Success_WhenConfigIsNull()
		{
			var instance = new BuildVisitor(string.Empty);
			var code = instance.VisitProject(Lib2Directory, null);
			Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
			Assert.AreEqual(ReturnCode.Success, instance.ReturnCode, "Invalid Return Code");
		}

		[Test]
		public void VisitProject_ShouldRunBuildScript()
		{
			var factory = Container.Resolve<IGitDependFileFactory>();
			var processManager = Container.Resolve<IProcessManager>();
			var fileSystem = RegisterMockFileSystem();
			var instance = new BuildVisitor(string.Empty);

			string dir;
			ReturnCode loadCode = ReturnCode.Success;
			factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode))
				.Returns(Lib1Config);

			EnsureDirectory(fileSystem, Lib1Directory);
			EnsureDirectory(fileSystem, Lib2Directory);
			EnsureFiles(fileSystem, Lib1Directory, new List<string>
			{
				"make.bat"
			});

			bool scriptExecuted = false;
			processManager.Arrange(m => m.Start(Arg.IsAny<ProcessStartInfo>()))
				.Returns((ProcessStartInfo info) =>
				{
					scriptExecuted = true;
					EnsureFiles(fileSystem, Lib1PackagesDirectory, Lib1Packages);

					return new FakeProcess
					{
						ExitCode = 0,
						HasExited = true
					};
				});

			var code = instance.VisitProject(Lib1Directory, Lib1Config);

			Assert.IsTrue(scriptExecuted, "Build Script was not executed");
			Assert.AreEqual(ReturnCode.Success, code, "Invalid ReturnCode");
			Assert.AreEqual(ReturnCode.Success, instance.ReturnCode, "Invalid ReturnCode");
		}
	}
}