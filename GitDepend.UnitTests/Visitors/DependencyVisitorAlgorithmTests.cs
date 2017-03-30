using System.Collections.Generic;
using System.Linq;
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
    public class DependencyVisitorAlgorithmTests : TestFixtureBase
    {
        private const string PROJECT_DIRECTORY = @"C:\projects\GitDepend";

        [Test]
        public void TraverseDependencies_ShouldNotThrow_WithNullVisitorAndNullDirectory()
        {
            var instance = new DependencyVisitorAlgorithm();
            instance.TraverseDependencies(null, null);
        }

        [Test]
        public void TraverseDependencies_ShouldNotThrow_WithNullDirectory()
        {
            var visitor = Mock.Create<IVisitor>();
            var instance = new DependencyVisitorAlgorithm();

            instance.TraverseDependencies(visitor, null);
        }

        [Test]
        public void TraverseDependencies_ShouldNotThrow_WithNullVisitor()
        {
            var instance = new DependencyVisitorAlgorithm();
            instance.TraverseDependencies(null, @"C:\testdir");
        }

        [Test]
        public void TraverseDependencies_ShouldReturn_GitRepositoryNotFound_WhenDirectoryDoesNotExist()
        {
            var visitor = Mock.Create<IVisitor>();
            var instance = new DependencyVisitorAlgorithm();

            instance.TraverseDependencies(visitor, PROJECT_DIRECTORY);

            Assert.AreEqual(ReturnCode.GitRepositoryNotFound, visitor.ReturnCode, "Invalid ReturnCode");
        }

        [Test]
        public void TraverseDependencies_ShouldReturn_GitRepositoryNotFound_WhenUnableToLoadFile()
        {
            var visitor = Mock.Create<IVisitor>();
            var factory = Container.Resolve<IGitDependFileFactory>();
            var fileSystem = RegisterMockFileSystem();
            var instance = new DependencyVisitorAlgorithm();

            // LoadFromDirectory returns null
            string dir;
            ReturnCode code = ReturnCode.GitRepositoryNotFound;
            factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out code))
                .Returns(null as GitDependFile);

            // The directory needs to exist
            fileSystem.Directory.CreateDirectory(PROJECT_DIRECTORY);

            instance.TraverseDependencies(visitor, PROJECT_DIRECTORY);

            Assert.AreEqual(ReturnCode.GitRepositoryNotFound, visitor.ReturnCode, "Invalid ReturnCode");
        }

        [Test]
        public void TraverseDependencies_ShouldOnlyVisitEachNodeOnce()
        {
            var fileSystem = RegisterMockFileSystem();
            var factory = Container.Resolve<IGitDependFileFactory>();
            var git = Container.Resolve<IGit>();
            var instance = new DependencyVisitorAlgorithm();

            List<string> visitedDependencies = new List<string>();
            List<string> visitedProjects = new List<string>();

            var visitor = Mock.Create<IVisitor>();
            visitor.Arrange(v => v.VisitDependency(Arg.AnyString, Arg.IsAny<Dependency>()))
                .Returns((string directory, Dependency dependency) =>
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
            var dir = Lib2Config.Dependencies.First(d => d.Configuration.Name == "Lib1").Directory;
            string lib1Dir = fileSystem.Path.GetFullPath(fileSystem.Path.Combine(lib2Dir, dir));

            fileSystem.Directory.CreateDirectory(PROJECT_DIRECTORY);
            fileSystem.Directory.CreateDirectory(fileSystem.Path.Combine(PROJECT_DIRECTORY, ".git"));

            ReturnCode lib1Code;
            ReturnCode lib2Code;
            factory.Arrange(f => f.LoadFromDirectory(lib1Dir, out lib1Dir, out lib1Code))
                .Returns(Lib1Config);

            factory.Arrange(f => f.LoadFromDirectory(lib2Dir, out lib2Dir, out lib2Code))
                .Returns(Lib2Config);

            git.Arrange(g => g.Clone(Arg.AnyString, Arg.AnyString, Arg.AnyString))
                .Returns((string url, string directory, string branch) =>
                {
                    fileSystem.Directory.CreateDirectory(directory);
                    return ReturnCode.Success;
                });

            instance.TraverseDependencies(visitor, PROJECT_DIRECTORY);

            Assert.AreEqual(ReturnCode.Success, visitor.ReturnCode, "Invalid ReturnCode");
            Assert.AreEqual(1, visitedDependencies.Count, "Incorrect number of visited dependencies");
            Assert.AreEqual(2, visitedProjects.Count, "Incorrect number of visited projects");
        }

        [Test]
        public void TraverseDependencies_ShouldReturn_FailedToRunGitCommand_WhenFailingToCloneRepository()
        {
            var visitor = Mock.Create<IVisitor>();
            var fileSystem = RegisterMockFileSystem();
            var factory = Container.Resolve<IGitDependFileFactory>();
            var git = Container.Resolve<IGit>();
            var instance = new DependencyVisitorAlgorithm();

            fileSystem.Directory.CreateDirectory(PROJECT_DIRECTORY);
            fileSystem.Directory.CreateDirectory(fileSystem.Path.Combine(PROJECT_DIRECTORY, ".git"));

            string lib2Dir = PROJECT_DIRECTORY;

            ReturnCode lib1Code;
            factory.Arrange(f => f.LoadFromDirectory(lib2Dir, out lib2Dir, out lib1Code))
                .Returns(Lib2Config);

            git.Arrange(g => g.Clone(Arg.AnyString, Arg.AnyString, Arg.AnyString))
                .Returns(ReturnCode.FailedToRunGitCommand);

            instance.TraverseDependencies(visitor, PROJECT_DIRECTORY);
            Assert.AreEqual(ReturnCode.FailedToRunGitCommand, visitor.ReturnCode, "Invalid ReturnCode");
        }

        [Test]
        public void TraverseDependencies_ShouldReturnFailureCode_WhenVisitingDependencyReturnsFailureCode()
        {
            var visitor = Mock.Create<IVisitor>();
            var fileSystem = RegisterMockFileSystem();
            var factory = Container.Resolve<IGitDependFileFactory>();
            var git = Container.Resolve<IGit>();
            var instance = new DependencyVisitorAlgorithm();

            visitor.Arrange(v => v.VisitDependency(Arg.AnyString, Arg.IsAny<Dependency>()))
                .Returns(ReturnCode.FailedToRunBuildScript);

            fileSystem.Directory.CreateDirectory(PROJECT_DIRECTORY);
            fileSystem.Directory.CreateDirectory(fileSystem.Path.Combine(PROJECT_DIRECTORY, ".git"));

            string lib2Dir = PROJECT_DIRECTORY;

            ReturnCode lib2Code;
            factory.Arrange(f => f.LoadFromDirectory(lib2Dir, out lib2Dir, out lib2Code))
                .Returns(Lib2Config);

            git.Arrange(g => g.Clone(Arg.AnyString, Arg.AnyString, Arg.AnyString))
                .Returns((string url, string directory, string branch) =>
                {
                    fileSystem.Directory.CreateDirectory(directory);
                    return ReturnCode.Success;
                });

            instance.TraverseDependencies(visitor, PROJECT_DIRECTORY);
            Assert.AreEqual(ReturnCode.ConfigurationFileDoesNotExist, visitor.ReturnCode, "Invalid ReturnCode");
        }

        [Test]
        public void TraverseDependencies_ShouldReturnFailureCode_WhenVisitingProjectReturnsFailureCode()
        {
            var visitor = Mock.Create<IVisitor>();
            var fileSystem = RegisterMockFileSystem();
            var factory = Container.Resolve<IGitDependFileFactory>();
            var git = Container.Resolve<IGit>();
            var instance = new DependencyVisitorAlgorithm();

            List<string> visitedDependencies = new List<string>();
            List<string> visitedProjects = new List<string>();

            visitor.Arrange(v => v.VisitDependency(Arg.AnyString, Arg.IsAny<Dependency>()))
                .Returns((string directory, Dependency dependency) =>
                {
                    Assert.IsFalse(visitedDependencies.Contains(dependency.Directory), "This dependency has already been visited");

                    visitedDependencies.Add(dependency.Directory);
                    return ReturnCode.Success;
                });
            string outDirectory;
            ReturnCode returnCode;
            factory.Arrange(x => x.LoadFromDirectory(Lib1Directory, out outDirectory, out returnCode))
                .Returns(Lib1Config);
            visitor.Arrange(v => v.VisitProject(Arg.AnyString, Arg.IsAny<GitDependFile>()))
                .Returns((string directory, GitDependFile config) =>
                {
                    Assert.IsFalse(visitedProjects.Contains(directory), "This project has already been visited");

                    visitedProjects.Add(directory);
                    return ReturnCode.FailedToRunNugetCommand;
                });

            string lib2Dir = PROJECT_DIRECTORY;

            fileSystem.Directory.CreateDirectory(PROJECT_DIRECTORY);
            fileSystem.Directory.CreateDirectory(fileSystem.Path.Combine(PROJECT_DIRECTORY, ".git"));

            ReturnCode lib2Code;
            factory.Arrange(f => f.LoadFromDirectory(lib2Dir, out lib2Dir, out lib2Code))
                .Returns(Lib2Config);

            git.Arrange(g => g.Clone(Arg.AnyString, Arg.AnyString, Arg.AnyString))
                .Returns((string url, string directory, string branch) =>
                {
                    fileSystem.Directory.CreateDirectory(directory);
                    return ReturnCode.Success;
                });

            instance.TraverseDependencies(visitor, PROJECT_DIRECTORY);

            Assert.AreEqual(0, visitedDependencies.Count, "Incorrect number of visited dependencies");
            Assert.AreEqual(1, visitedProjects.Count, "Incorrect number of visited projects");
            Assert.AreEqual(ReturnCode.FailedToRunNugetCommand, visitor.ReturnCode, "Invalid ReturnCode");
        }
    }
}
