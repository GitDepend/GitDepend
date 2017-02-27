using System.Collections.Generic;
using System.IO.Abstractions;
using GitDepend.Busi;

namespace GitDepend.Visitors
{
    /// <summary>
    /// Implements the visitor algorithm for visiting all repository dependencies. You are guarunteed
    /// to visit all project dependencies before visiting the project.
    /// </summary>
    public class DependencyVisitorAlgorithm : IDependencyVisitorAlgorithm
    {
        private readonly IGitDependFileFactory _factory;
        private readonly IGit _git;
        private readonly IFileSystem _fileSystem;
        private readonly IConsole _console;
        private readonly HashSet<string> _visitedDependencies = new HashSet<string>();
        private readonly HashSet<string> _visitedProjects = new HashSet<string>();

        /// <summary>
        /// Creates a new <see cref="DependencyVisitorAlgorithm"/>
        /// </summary>
        public DependencyVisitorAlgorithm()
        {
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _git = DependencyInjection.Resolve<IGit>();
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
            _console = DependencyInjection.Resolve<IConsole>();
        }

        /// <summary>
        /// Traverses all dependencies beginning in the given directory.
        /// </summary>
        /// <param name="visitor">The <see cref="IVisitor"/> that should be called at each dependency and project visit.</param>
        /// <param name="directory">The directory containing a GitDepend.json file.</param>
        public void TraverseDependencies(IVisitor visitor, string directory)
        {
            // Make sure we are working with something here.
            if (visitor == null || string.IsNullOrEmpty(directory))
            {
                return;
            }

            directory = _fileSystem.Path.GetFullPath(directory);

            // If the directory doesn't exist we are done.
            if (!_fileSystem.Directory.Exists(directory))
            {
                visitor.ReturnCode = ReturnCode.GitRepositoryNotFound;
                return;
            }

            string dir;
            ReturnCode code;
            var config = _factory.LoadFromDirectory(directory, out dir, out code);

            if (code != ReturnCode.Success)
            {
                visitor.ReturnCode = code;
                return;
            }

            foreach (var dependency in config.Dependencies)
            {
                dependency.Directory = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(dir, dependency.Directory));

                // If the dependency does not exist on disk we need to clone it.
                if (!_fileSystem.Directory.Exists(dependency.Directory))
                {
                    _console.WriteLine($"Cloning {dependency.Url} into {dependency.Directory}");

                    code = _git.Clone(dependency.Url, dependency.Directory, dependency.Branch);
                    _console.WriteLine();

                    // If something went wrong with git we are done.
                    if (code != ReturnCode.Success)
                    {
                        visitor.ReturnCode = code;
                        return;
                    }
                }

                // Visit all dependencies of this dependency.
                TraverseDependencies(visitor, dependency.Directory);

                // If something went wrong with a dependency we are done.
                if (visitor.ReturnCode != ReturnCode.Success)
                {
                    return;
                }

                // Make sure to only visit dependencies once.
                if (!_visitedDependencies.Contains(dependency.Directory))
                {
                    _visitedDependencies.Add(dependency.Directory);

                    // Visit the dependency.
                    code = visitor.VisitDependency(dir, dependency);

                    // If something went wrong visiting the dependency we are done.
                    if (code != ReturnCode.Success)
                    {
                        visitor.ReturnCode = code;
                        return;
                    }
                }
            }

            // Make sure to only visit projects once.
            if (!_visitedProjects.Contains(dir))
            {
                _visitedProjects.Add(dir);

                // Visit the project.
                code = visitor.VisitProject(dir, config);
                _console.WriteLine();

                // If something went wrong visiting the project we are done.
                if (code != ReturnCode.Success)
                {
                    visitor.ReturnCode = code;
                }
            }
        }

        /// <summary>
        /// Resets the algorithm to a default state.
        /// </summary>
        public void Reset()
        {
            _visitedDependencies.Clear();
            _visitedProjects.Clear();
        }
    }
}
