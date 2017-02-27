using System.Collections.Generic;
using System.IO.Abstractions;
using GitDepend.Busi;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
    /// <summary>
    /// An implementatio of <see cref="NamedDependenciesVisitor"/> that displays the status of all dependencies.
    /// </summary>
    public class DisplayStatusVisitor : NamedDependenciesVisitor
    {
        private readonly IGit _git;

        /// <summary>
        /// Creates a new <see cref="DisplayStatusVisitor"/>
        /// </summary>
        /// <param name="whilelist">The projects to visit. If this list is null or empty all projects will be visited.</param>
        public DisplayStatusVisitor(IList<string> whilelist) : base(whilelist)
        {
            _git = DependencyInjection.Resolve<IGit>();
        }

        #region Overrides of NamedDependenciesVisitor

        /// <summary>
        /// Visits a project dependency.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="dependency">The <see cref="Dependency"/> to visit.</param>
        /// <returns>The return code.</returns>
        protected override ReturnCode OnVisitDependency(string directory, Dependency dependency)
        {
            _git.WorkingDirectory = directory;
            return ReturnCode = _git.Status();
        }

        #endregion
    }
}
