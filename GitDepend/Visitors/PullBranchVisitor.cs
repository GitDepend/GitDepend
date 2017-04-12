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
        private string _gitArguments;

        /// <summary>
        /// This visitor pulls in with the given arguments each directory or the named directories in the whitelist.
        /// </summary>
        /// <param name="gitArguments">The list of arguments to provide to git pull</param>
        /// <param name="whitelist">The dependencies to visit</param>
        public PullBranchVisitor(string gitArguments, IList<string> whitelist) : base(whitelist)
        {
            _git = DependencyInjection.Resolve<IGit>();
            _gitArguments = gitArguments;
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
            _git.WorkingDirectory = dependency.Directory;
            var returnCode = _git.Pull(_gitArguments);
            if (returnCode == ReturnCode.FailedToRunGitCommand)
            {
                return ReturnCode = ReturnCode.Success;
            }
            return ReturnCode = returnCode;
        }
    }
}
