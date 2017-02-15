using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.Configuration;
using GitDepend.Visitors;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Visitors
{
	[TestFixture]
	public class BuildAndUpdateDependenciesVisitorTests : TestFixtureBase
	{
		private IGitDependFileFactory _factory;
		private IGit _git;
		private INuget _nuget;
		private IProcessManager _processManager;
		private MockFileSystem _fileSystem;
		private BuildAndUpdateDependenciesVisitor _instance;
		private IConsole _console;

		[SetUp]
		public void SetUp()
		{
			_factory = Mock.Create<IGitDependFileFactory>();
			_git = Mock.Create<IGit>();
			_nuget = Mock.Create<INuget>();
			_processManager = Mock.Create<IProcessManager>();
			_fileSystem = new MockFileSystem();
			_console = Mock.Create<IConsole>();
			_instance = new BuildAndUpdateDependenciesVisitor(_factory, _git, _nuget, _processManager, _fileSystem, _console);
		}

		[Test]
		public void VisitDependency_ShouldReturn_GitDependFileNotFound_WhenUnableToLoadConfigFile()
		{
			string dir;
			ReturnCode loadCode = ReturnCode.GitRepositoryNotFound;
			_factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode))
				.Returns(null as GitDependFile);

			var code = _instance.VisitDependency(Lib2Directory, Lib1Dependency);
			Assert.AreEqual(ReturnCode.GitRepositoryNotFound, code, "Invalid ReturnCode");
			Assert.AreEqual(ReturnCode.GitRepositoryNotFound, _instance.ReturnCode, "Invalid ReturnCode");
		}

		[Test]
		public void VisitDependency_ShouldRunBuildScript()
		{
			string dir;
			ReturnCode loadCode = ReturnCode.Success;
			_factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode))
				.Returns(Lib1Config);

			bool scriptExecuted = false;
			_processManager.Arrange(m => m.Start(Arg.IsAny<ProcessStartInfo>()))
				.Returns((ProcessStartInfo info) =>
				{
					scriptExecuted = true;
					EnsureFiles(_fileSystem, Lib1PackagesDirectory, Lib1Packages);

					return new FakeProcess
					{
						ExitCode = 0,
						HasExited = true
					};
				});


			var code = _instance.VisitDependency(Lib2Directory, Lib1Dependency);

			var cachedPackages = _fileSystem.Directory.GetFiles(_instance.GetCacheDirectory(), "*.nupkg");

			Assert.AreEqual(Lib1Packages.Count, cachedPackages.Length, "Invalid number of cached nuget packages");

			foreach (var package in cachedPackages)
			{
				var name = _fileSystem.Path.GetFileName(package);
				// Make sure that name shows up in the original list.
				Assert.IsTrue(Lib1Packages.Any(p => _fileSystem.Path.GetFileName(p) == name));
			}

			Assert.IsTrue(scriptExecuted, "Build Script was not executed");
			Assert.AreEqual(ReturnCode.Success, code, "Invalid ReturnCode");
			Assert.AreEqual(ReturnCode.Success, _instance.ReturnCode, "Invalid ReturnCode");
		}

		[Test]
		public void VisitDependency_ShouldReturn_CouldNotCreateCacheDirectory_IfCacheDirectoryFailsToCreate()
		{
			string dir;
			ReturnCode loadCode = ReturnCode.Success;
			_factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode))
				.Returns(Lib1Config);

			var fileSystem = Mock.Create<IFileSystem>();
			fileSystem.Directory.Arrange(d => d.CreateDirectory(Arg.AnyString))
				.Throws<IOException>("Access Denied");

			var instance = new BuildAndUpdateDependenciesVisitor(_factory, _git, _nuget, _processManager, fileSystem, _console);

			var code = instance.VisitDependency(Lib2Directory, Lib1Dependency);
			Assert.AreEqual(ReturnCode.CouldNotCreateCacheDirectory, code, "Invalid ReturnCode");
			Assert.AreEqual(ReturnCode.CouldNotCreateCacheDirectory, instance.ReturnCode, "Invalid ReturnCode");
		}

		[Test]
		public void VisitProject_ShouldReturn_DirectoryDoesNotExist_WhenDirectoryIsNull()
		{
			var code = _instance.VisitProject(null, Lib2Config);
			Assert.AreEqual(ReturnCode.DirectoryDoesNotExist, code, "Invalid Return Code");
		}

		[Test]
		public void VisitProject_ShouldReturn_DirectoryDoesNotExist_WhenDirectoryDoesNotExist()
		{
			var code = _instance.VisitProject(Lib2Directory, Lib2Config);
			Assert.AreEqual(ReturnCode.DirectoryDoesNotExist, code, "Invalid Return Code");
		}

		[Test]
		public void VisitProject_ShouldReturn_Success_WhenConfigIsNull()
		{
			var code = _instance.VisitProject(Lib2Directory, null);
			Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
			Assert.AreEqual(ReturnCode.Success, _instance.ReturnCode, "Invalid Return Code");
		}

		[Test]
		public void VisitProject_ShouldCallNugetUpdate_ForEachNugetPackageInDependencyArtifactsFolder()
		{
			EnsureFiles(_fileSystem, Lib1PackagesDirectory, Lib1Packages);
			EnsureFiles(_fileSystem, Lib1PackagesDirectory, new List<string>
			{
				".nupkg",
				"bad.input.nupkg"
			});

			EnsureFiles(_fileSystem, Lib2Directory, Lib2Solutions);
			Dictionary<string, List<string>> updatedPackages = new Dictionary<string, List<string>>();

			_nuget.Arrange(n => n.Update(Arg.AnyString, Arg.AnyString, Arg.AnyString, Arg.AnyString))
				.Returns((string solution, string id, string version, string sourceDirectory) =>
				{
					Assert.AreEqual(Lib2Directory, _nuget.WorkingDirectory, "Invalid working directory for Nuget.exe");

					var key = $"{id}.{version}";
					if (updatedPackages.ContainsKey(key))
					{
						var list = updatedPackages[key];

						Assert.IsFalse(list.Contains(solution));
						list.Add(solution);
					}
					else
					{
						updatedPackages.Add(key, new List<string> {solution});
					}
					return ReturnCode.Success;
				});

			_fileSystem.Directory.CreateDirectory(Lib2Directory);

			var code = _instance.VisitProject(Lib2Directory, Lib2Config);
			Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
			Assert.AreEqual(ReturnCode.Success, _instance.ReturnCode, "Invalid Return Code");

			Assert.AreEqual(Lib1Packages.Count, updatedPackages.Keys.Count, "Invalid number of updated packages");

			foreach (var kvp in updatedPackages)
			{
				Assert.IsTrue(Lib1Packages.Any(p => _fileSystem.Path.GetFileNameWithoutExtension(p) == kvp.Key));

				foreach (var solution in kvp.Value)
				{
					Assert.IsTrue(Lib2Solutions.Any(s => s == _fileSystem.Path.GetFileName(solution)), $"{solution} was not found in Lib2Solutions");
				}
			}
		}

		[Test]
		public void VisitProject_ShouldReturn_CouldNotCreateCacheDirectory_WhenCacheDirectoryFailsToInitialize()
		{
			var fileSystem = Mock.Create<IFileSystem>();
			fileSystem.Directory.Arrange(d => d.Exists(Arg.AnyString)).Returns(true);
			fileSystem.Directory.Arrange(d => d.GetFiles(Arg.AnyString, "*.nupkg"))
				.Returns(Lib1Packages.ToArray());
			fileSystem.Directory.Arrange(d => d.GetFiles(Arg.AnyString, "*.sln", Arg.IsAny<SearchOption>()))
				.Returns(Lib2Solutions.ToArray());

			fileSystem.Path.Arrange(p => p.GetFileNameWithoutExtension(Arg.AnyString))
				.Returns(path => _fileSystem.Path.GetFileNameWithoutExtension(path));

			fileSystem.Directory.Arrange(d => d.CreateDirectory(Arg.AnyString))
				.Throws<IOException>("Access Denied");

			var instance = new BuildAndUpdateDependenciesVisitor(_factory, _git, _nuget, _processManager, fileSystem, _console);

			var code = instance.VisitProject(Lib2Directory, Lib2Config);
			Assert.AreEqual(ReturnCode.CouldNotCreateCacheDirectory, code, "Invalid Return Code");
			Assert.AreEqual(ReturnCode.CouldNotCreateCacheDirectory, instance.ReturnCode, "Invalid Return Code");
		}
	}
}
