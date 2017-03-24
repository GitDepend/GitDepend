using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
    /// <summary>
    /// This visitor goes through each dependency (or named dependencies and pulls with the given arguments.
    /// </summary>
    public class PullBranchVisitor : NamedDependenciesVisitor
    {
        private IGit _git;

        /// <summary>
        /// This visitor pulls in with the given arguments each directory or the named directories in the whitelist.
        /// </summary>
        /// <param name="whitelist">The dependencies to visit</param>
        public PullBranchVisitor(IList<string> whitelist) : base(whitelist)
        {
            _git = DependencyInjection.Resolve<IGit>();
        }

        /// <summary>
        /// Provides the custom hook for VisitDependency. This will only be called if the dependency
        /// was specified in the whitelist.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="dependency">The <see cref="Dependency"/> to visit.</param>
        /// <returns></returns>
        protected override ReturnCode OnVisitDependency(string directory, Dependency dependency)
        {
            return ReturnCode.Success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ReturnCode VisitProject(string directory, GitDependFile config)
        {
            _git.WorkingDirectory = directory;
            var returnCode = _git.Pull();
            if (returnCode == ReturnCode.FailedToRunGitCommand)
            {
                return ReturnCode = ReturnCode.Success;
            }
            return ReturnCode = returnCode;
        }
    }
}
