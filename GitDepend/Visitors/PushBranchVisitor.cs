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
    /// The visitor to go in each dependency and push it's respective branch with the given arguments.
    /// </summary>
    public class PushBranchVisitor : NamedDependenciesVisitor
    {
        private readonly IGit _git;

        /// <summary>
        /// The constructor to create the visitor.
        /// </summary>
        /// <param name="whitelist">The directories to process</param>
        public PushBranchVisitor(IList<string> whitelist) : base(whitelist)
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
            _git.WorkingDirectory = dependency.Directory;

            var code = _git.Push();

            if (code == ReturnCode.FailedToRunGitCommand)
            {
                return ReturnCode.Success;
            }

            return code;
        }
    }
}
