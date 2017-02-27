using System;
using System.IO.Abstractions;
using GitDepend.Busi;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
    /// <summary>
    /// An implementation of <see cref="IVisitor"/> that lists all branches on all dependencies.
    /// </summary>
    public class ListAllBranchesVisitor : IVisitor
    {
        private readonly IGit _git;
        private readonly IFileSystem _fileSystem;
        private readonly IConsole _console;

        /// <summary>
        /// Creates a new <see cref="ListMergedBranchesVisitor"/>
        /// </summary>
        public ListAllBranchesVisitor()
        {
            _git = DependencyInjection.Resolve<IGit>();
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
            _console = DependencyInjection.Resolve<IConsole>();
        }

        #region Implementation of IVisitor

        /// <summary>
        /// The return code
        /// </summary>
        public ReturnCode ReturnCode { get; set; }

        /// <summary>
        /// Visits a project dependency.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="dependency">The <see cref="Dependency"/> to visit.</param>
        /// <returns>The return code.</returns>
        public ReturnCode VisitDependency(string directory, Dependency dependency)
        {
            var dir = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(directory, dependency.Directory));

            var origColor = _console.ForegroundColor;
            _console.ForegroundColor = ConsoleColor.Green;
            _console.WriteLine("dependency:");
            _console.WriteLine($"    name: {dependency.Configuration.Name}");
            _console.WriteLine($"    dir: {dir}");
            _console.WriteLine();
            _console.ForegroundColor = origColor;

            _git.WorkingDirectory = dir;
            return ReturnCode = _git.ListAllBranches();
        }

        /// <summary>
        /// Visists a project.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="config">The <see cref="GitDependFile"/> with project configuration information.</param>
        /// <returns>The return code.</returns>
        public ReturnCode VisitProject(string directory, GitDependFile config)
        {
            return ReturnCode.Success;
        }

        #endregion
    }
}