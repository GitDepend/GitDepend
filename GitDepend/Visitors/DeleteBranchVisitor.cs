using System;
using GitDepend.Busi;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
    /// <summary>
    /// An implementation of <see cref="IVisitor"/> that creates the specified branch on all dependencies.
    /// </summary>
    public class DeleteBranchVisitor : IVisitor
    {
        private readonly string _branchName;
        private readonly bool _force;
        private readonly IGit _git;
        private readonly IConsole _console;

        /// <summary>
        /// Creates a new <see cref="DeleteBranchVisitor"/>
        /// </summary>
        /// <param name="branchName">The branch name to delete.</param>
        /// <param name="force">Should the deletion be forced or not.</param>
        public DeleteBranchVisitor(string branchName, bool force)
        {
            _branchName = branchName;
            _force = force;
            _git = DependencyInjection.Resolve<IGit>();
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
            return ReturnCode.Success;
        }

        /// <summary>
        /// Visists a project.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="config">The <see cref="GitDependFile"/> with project configuration information.</param>
        /// <returns>The return code.</returns>
        public ReturnCode VisitProject(string directory, GitDependFile config)
        {
            _console.WriteLine(_force
                ? $"forcefully deleting the {_branchName} branch from {config.Name}"
                : $"Deleting the {_branchName} branch from {config.Name}");

            _git.WorkingDirectory = directory;
            return ReturnCode = _git.Delete(_branchName, _force);
        }

        #endregion
    }
}