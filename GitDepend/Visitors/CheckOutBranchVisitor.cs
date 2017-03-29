using GitDepend.Busi;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
    /// <summary>
    /// An implementation of <see cref="IVisitor"/> that switches to the specified branch
    /// on all dependencies.
    /// </summary>
    public class CheckOutBranchVisitor : IVisitor
    {
        private readonly string _branchName;
        private readonly bool _createBranch;
        private readonly IGit _git;

        /// <summary>
        /// Creates a new <see cref="CheckOutBranchVisitor"/>
        /// </summary>
        /// <param name="branchName">The name of the branch to check out.</param>
        /// <param name="createBranch">Should the branch be created?</param>
        public CheckOutBranchVisitor(string branchName, bool createBranch)
        {
            _branchName = branchName;
            _createBranch = createBranch;
            _git = DependencyInjection.Resolve<IGit>();
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
            _git.WorkingDirectory = directory;
            var code = _git.Checkout(_branchName, _createBranch);
            if (code == ReturnCode.FailedToRunGitCommand)
            {
                return ReturnCode = ReturnCode.Success;
            }
            return ReturnCode = code;
        }

        /// <summary>
        /// Called when the algorithm can't find the configuration file.
        /// </summary>
        /// <returns></returns>
        public ReturnCode MissingConfigurationFile()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}