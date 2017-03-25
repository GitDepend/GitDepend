using GitDepend.Busi;
using GitDepend.Configuration;
using GitDepend.Resources;

namespace GitDepend.Visitors
{
    /// <summary>
    /// An implementation of <see cref="IVisitor"/> that creates the specified branch on all dependencies.
    /// </summary>
    public class CreateBranchVisitor : IVisitor
    {
        private readonly IGit _git;
        private readonly IConsole _console;

        /// <summary>
        /// Creates a new <see cref="CreateBranchVisitor"/>
        /// </summary>
        /// <param name="branchName">The branch name to create.</param>
        public CreateBranchVisitor(string branchName)
        {
            BranchName = branchName;
            _git = DependencyInjection.Resolve<IGit>();
            _console = DependencyInjection.Resolve<IConsole>();
        }

        /// <summary>
        /// The name of the branch that will be created.
        /// </summary>
        public string BranchName { get; }

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
            _console.WriteLine(strings.CREATING_BRANCH_ON_REPONAME, BranchName, config.Name);
            _git.WorkingDirectory = directory;
            var code = _git.CreateBranch(BranchName);
            if (code == ReturnCode.FailedToRunGitCommand)
            {
                return ReturnCode = ReturnCode.Success;
            }
            return ReturnCode = code;
        }

        #endregion
    }
}
