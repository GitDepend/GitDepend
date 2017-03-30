using GitDepend.Busi;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
    /// <summary>
    /// An implementation of <see cref="IVisitor"/> that creates the specified branch on all dependencies.
    /// </summary>
    public class DeleteBranchVisitor : IVisitor
    {
        private readonly IGit _git;
        private readonly IConsole _console;

        /// <summary>
        /// Creates a new <see cref="DeleteBranchVisitor"/>
        /// </summary>
        /// <param name="branchName">The branch name to delete.</param>
        /// <param name="force">Should the deletion be forced or not.</param>
        public DeleteBranchVisitor(string branchName, bool force)
        {
            BranchName = branchName;
            Force = force;
            _git = DependencyInjection.Resolve<IGit>();
            _console = DependencyInjection.Resolve<IConsole>();
        }

        /// <summary>
        /// The name of the branch that should be deleted.
        /// </summary>
        public string BranchName { get; }

        /// <summary>
        /// Should the deletion be forced or not.
        /// </summary>
        public bool Force { get; }

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
        /// Visits a project.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="config">The <see cref="GitDependFile"/> with project configuration information.</param>
        /// <returns>The return code.</returns>
        public ReturnCode VisitProject(string directory, GitDependFile config)
        {
            _console.WriteLine(Force
                ? $"forcefully deleting the {BranchName} branch from {config.Name}"
                : $"Deleting the {BranchName} branch from {config.Name}");

            _git.WorkingDirectory = directory;
            var code = _git.DeleteBranch(BranchName, Force);
            if (code == ReturnCode.FailedToRunGitCommand)
            {
                return ReturnCode = ReturnCode.Success;
            }
            return ReturnCode = code;
        }

        #endregion
    }
}