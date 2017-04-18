using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="GitDepend.Visitors.IVisitor" />
    public class CleanDependencyVisitor : NamedDependenciesVisitor
    {
        private readonly IGit _git;
        private string _gitArguments;

        /// <summary>
        /// The name matched
        /// </summary>
        public bool NameMatched = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanDependencyVisitor" /> class.
        /// </summary>
        /// <param name="gitArguments">Arguments to pass through to git clean.</param>
        /// <param name="whitelist">The dependency name to clean.</param>
        public CleanDependencyVisitor(string gitArguments, IList<string> whitelist) : base(whitelist)
        {
            _git = DependencyInjection.Resolve<IGit>();
            _gitArguments = gitArguments;

        }

        /// <summary>
        /// Visits a project.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="config">The <see cref="GitDependFile" /> with project configuration information.</param>
        /// <returns>
        /// The return code.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public override ReturnCode VisitProject(string directory, GitDependFile config)
        {
            return ReturnCode.Success;
        }

        /// <summary>
        /// Provides the custom hook for VisitDependency. This will only be called if the dependency
        /// was specified in the whitelist.f
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="dependency">The <see cref="Dependency" /> to visit.</param>
        /// <returns></returns>
        protected override ReturnCode OnVisitDependency(string directory, Dependency dependency)
        {
            _git.WorkingDirectory = dependency.Directory;

            return _git.Clean(_gitArguments);
        }
    }
}
