using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using GitDepend.Busi;
using GitDepend.Configuration;
using GitDepend.Visitors;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Visitors
{
	[TestFixture]
	public class DependencyVisitorAlgorithmTests
	{
		private GitDependFile _lib1Config;
		private GitDependFile _lib2Config;

		private IGitDependFileFactory _factory;
		private IGit _git;
		private IFileSystem _fileSystem;
		private DependencyVisitorAlgorithm _instance;

		const string PROJECT_DIRECTORY = @"C:\projects\GitDepend";


		[SetUp]
		public void SetUp()
		{
			_lib1Config = new GitDependFile
			{
				Build = {Script = "make.bat"},
				Packages = {Directory = "artifacts/NuGet/Debug"}
			};

			_lib2Config = new GitDependFile
			{
				Build = { Script = "make.bat" },
				Packages = { Directory = "artifacts/NuGet/Debug" },
				Dependencies =
				{
					new Dependency
					{
						Name = "Lib1",
						Directory = "..\\Lib1",
						Url = "git@github.com:kjjuno/Lib1.git",
						Branch = "develop",
						Configuration = _lib1Config
					}
				}
			};

			_factory = Mock.Create<IGitDependFileFactory>();
			_git = Mock.Create<IGit>();
			_fileSystem = new MockFileSystem();
			_instance = new DependencyVisitorAlgorithm(_factory, _git, _fileSystem);
		}

		[Test]
		public void TraverseDependencies_ShouldNotThrow_WithNullVisitorAndNullDirectory()
		{
			_instance.TraverseDependencies(null, null);
		}

		[Test]
		public void TraverseDependencies_ShouldNotThrow_WithNullDirectory()
		{
			var visitor = Mock.Create<IVisitor>();
			
			_instance.TraverseDependencies(visitor, null);
		}

		[Test]
		public void TraverseDependencies_ShouldNotThrow_WithNullVisitor()
		{
			_instance.TraverseDependencies(null, @"C:\testdir");
		}

		[Test]
		public void TraverseDependencies_ShouldReturn_GitRepositoryNotFound_WhenDirectoryDoesNotExist()
		{
			var visitor = Mock.Create<IVisitor>();
			_instance.TraverseDependencies(visitor, PROJECT_DIRECTORY);

			Assert.AreEqual(ReturnCode.GitRepositoryNotFound, visitor.ReturnCode, "Invalid ReturnCode");
		}

		[Test]
		public void TraverseDependencies_ShouldReturn_GitDependFileNotFound_WhenUnableToLoadFile()
		{
			var visitor = Mock.Create<IVisitor>();

			// LoadFromDirectory returns null
			string dir, error;
			_factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out error))
				.Returns(null as GitDependFile);
			
			// The directory needs to exist
			_fileSystem.Directory.CreateDirectory(PROJECT_DIRECTORY);

			_instance.TraverseDependencies(visitor, PROJECT_DIRECTORY);

			Assert.AreEqual(ReturnCode.GitDependFileNotFound, visitor.ReturnCode, "Invalid ReturnCode");
		}

		[Test]
		public void TraverseDependencies_ShouldOnlyVisitEachNodeOnce()
		{
			List<string> visitedDependencies = new List<string>();
			List<string> visitedProjects = new List<string>();

			var visitor = Mock.Create<IVisitor>();
			visitor.Arrange(v => v.VisitDependency(Arg.IsAny<Dependency>()))
				.Returns((Dependency dependency) =>
				{
					Assert.IsFalse(visitedDependencies.Contains(dependency.Directory), "This dependency has already been visited");

					visitedDependencies.Add(dependency.Directory);
					return ReturnCode.Success;
				});

			visitor.Arrange(v => v.VisitProject(Arg.AnyString, Arg.IsAny<GitDependFile>()))
				.Returns((string directory, GitDependFile config) =>
				{
					Assert.IsFalse(visitedProjects.Contains(directory), "This project has already been visited");

					visitedProjects.Add(directory);
					return ReturnCode.Success;
				});

			string lib2Dir = PROJECT_DIRECTORY;
			var dir = _lib2Config.Dependencies.First(d => d.Name == "Lib1").Directory;
			string lib1Dir = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(lib2Dir, dir));

			_fileSystem.Directory.CreateDirectory(PROJECT_DIRECTORY);
			_fileSystem.Directory.CreateDirectory(_fileSystem.Path.Combine(PROJECT_DIRECTORY, ".git"));

			string error;
			_factory.Arrange(f => f.LoadFromDirectory(lib1Dir, out lib1Dir, out error))
				.Returns(_lib1Config);

			_factory.Arrange(f => f.LoadFromDirectory(lib2Dir, out lib2Dir, out error))
				.Returns(_lib2Config);

			_git.Arrange(g => g.Clone(Arg.AnyString, Arg.AnyString, Arg.AnyString))
				.Returns((string url, string directory, string branch) =>
				{
					_fileSystem.Directory.CreateDirectory(directory);
					return ReturnCode.Success;
				});

			_instance.TraverseDependencies(visitor, PROJECT_DIRECTORY);

			Assert.AreEqual(ReturnCode.Success, visitor.ReturnCode, "Invalid ReturnCode");
			Assert.AreEqual(1, visitedDependencies.Count, "Incorrect number of visited dependencies");
			Assert.AreEqual(2, visitedProjects.Count, "Incorrect number of visited projects");
		}

		[Test]
		public void TraverseDependencies_ShouldReturn_FailedToRunGitCommand_WhenFailingToCloneRepository()
		{
			var visitor = Mock.Create<IVisitor>();
			_fileSystem.Directory.CreateDirectory(PROJECT_DIRECTORY);
			_fileSystem.Directory.CreateDirectory(_fileSystem.Path.Combine(PROJECT_DIRECTORY, ".git"));

			string lib2Dir = PROJECT_DIRECTORY;

			string error;
			_factory.Arrange(f => f.LoadFromDirectory(lib2Dir, out lib2Dir, out error))
				.Returns(_lib2Config);

			_git.Arrange(g => g.Clone(Arg.AnyString, Arg.AnyString, Arg.AnyString))
				.Returns(ReturnCode.FailedToRunGitCommand);

			_instance.TraverseDependencies(visitor, PROJECT_DIRECTORY);
			Assert.AreEqual(ReturnCode.FailedToRunGitCommand, visitor.ReturnCode, "Invalid ReturnCode");
		}

		[Test]
		public void TraverseDependencies_ShouldReturnFailureCode_WhenVisitingDependencyReturnsFailureCode()
		{
			var visitor = Mock.Create<IVisitor>();
			visitor.Arrange(v => v.VisitDependency(Arg.IsAny<Dependency>()))
				.Returns(ReturnCode.FailedToRunBuildScript);

			_fileSystem.Directory.CreateDirectory(PROJECT_DIRECTORY);
			_fileSystem.Directory.CreateDirectory(_fileSystem.Path.Combine(PROJECT_DIRECTORY, ".git"));

			string lib2Dir = PROJECT_DIRECTORY;

			string error;
			_factory.Arrange(f => f.LoadFromDirectory(lib2Dir, out lib2Dir, out error))
				.Returns(_lib2Config);

			_git.Arrange(g => g.Clone(Arg.AnyString, Arg.AnyString, Arg.AnyString))
				.Returns((string url, string directory, string branch) =>
				{
					_fileSystem.Directory.CreateDirectory(directory);
					return ReturnCode.Success;
				});

			_instance.TraverseDependencies(visitor, PROJECT_DIRECTORY);
			Assert.AreEqual(ReturnCode.FailedToRunBuildScript, visitor.ReturnCode, "Invalid ReturnCode");
		}

		[Test]
		public void TraverseDependencies_ShouldReturnFailureCode_WhenVisitingProjectReturnsFailureCode()
		{
			List<string> visitedDependencies = new List<string>();
			List<string> visitedProjects = new List<string>();

			var visitor = Mock.Create<IVisitor>();
			visitor.Arrange(v => v.VisitDependency(Arg.IsAny<Dependency>()))
				.Returns((Dependency dependency) =>
				{
					Assert.IsFalse(visitedDependencies.Contains(dependency.Directory), "This dependency has already been visited");

					visitedDependencies.Add(dependency.Directory);
					return ReturnCode.Success;
				});

			visitor.Arrange(v => v.VisitProject(Arg.AnyString, Arg.IsAny<GitDependFile>()))
				.Returns((string directory, GitDependFile config) =>
				{
					Assert.IsFalse(visitedProjects.Contains(directory), "This project has already been visited");

					visitedProjects.Add(directory);
					return ReturnCode.FailedToRunNugetCommand;
				});

			string lib2Dir = PROJECT_DIRECTORY;
			var dir = _lib2Config.Dependencies.First(d => d.Name == "Lib1").Directory;
			string lib1Dir = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(lib2Dir, dir));

			_fileSystem.Directory.CreateDirectory(PROJECT_DIRECTORY);
			_fileSystem.Directory.CreateDirectory(_fileSystem.Path.Combine(PROJECT_DIRECTORY, ".git"));

			string error;
			_factory.Arrange(f => f.LoadFromDirectory(lib1Dir, out lib1Dir, out error))
				.Returns(_lib1Config);

			_factory.Arrange(f => f.LoadFromDirectory(lib2Dir, out lib2Dir, out error))
				.Returns(_lib2Config);

			_git.Arrange(g => g.Clone(Arg.AnyString, Arg.AnyString, Arg.AnyString))
				.Returns((string url, string directory, string branch) =>
				{
					_fileSystem.Directory.CreateDirectory(directory);
					return ReturnCode.Success;
				});

			_instance.TraverseDependencies(visitor, PROJECT_DIRECTORY);

			Assert.AreEqual(0, visitedDependencies.Count, "Incorrect number of visited dependencies");
			Assert.AreEqual(1, visitedProjects.Count, "Incorrect number of visited projects");
			Assert.AreEqual(ReturnCode.FailedToRunNugetCommand, visitor.ReturnCode, "Invalid ReturnCode");
		}
	}
}
