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
    /// 
    /// </summary>
    /// <seealso cref="GitDepend.Visitors.IVisitor" />
    public class CleanDependencyVisitor : NamedDependenciesVisitor, IVisitor
    {
        private readonly IGit _git;
        private bool _cleanDirectory;
        private bool _cleanFiles;
        private bool _force;
        private bool _dryRun;
        private IList<string> _dependencyNameToClean;

        /// <summary>
        /// The name matched
        /// </summary>
        public bool NameMatched = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanDependencyVisitor" /> class.
        /// </summary>
        /// <param name="dryRun">if set to <c>true</c> [dry run].</param>
        /// <param name="force">if set to <c>true</c> [force].</param>
        /// <param name="cleanFiles">if set to <c>true</c> [clean files].</param>
        /// <param name="cleanDirectory">if set to <c>true</c> [clean directory].</param>
        /// <param name="dependencyNameToClean">The dependency name to clean.</param>
        public CleanDependencyVisitor(bool dryRun, bool force, bool cleanFiles, bool cleanDirectory, IList<string> dependencyNameToClean) : base(dependencyNameToClean)
        {
            _git = DependencyInjection.Resolve<IGit>();
            _dryRun = dryRun;
            _force = force;
            _cleanFiles = cleanFiles;
            _cleanDirectory = cleanDirectory;
            _dependencyNameToClean = dependencyNameToClean;
        }

        /// <summary>
        /// Visists a project.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="config">The <see cref="GitDependFile" /> with project configuration information.</param>
        /// <returns>
        /// The return code.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public new ReturnCode VisitProject(string directory, GitDependFile config)
        {
            _git.WorkingDirectory = directory;
            if (_dependencyNameToClean.Count > 0)
            {
                foreach (var item in _dependencyNameToClean)
                {
                    if (item == config.Name)
                    {
                        return _git.Clean(_dryRun, _force, _cleanFiles, _cleanDirectory);
                    }
                }
            }
            else
            {
                return _git.Clean(_dryRun, _force, _cleanFiles, _cleanDirectory);
            }
            return ReturnCode.Success;
        }

        /// <summary>
        /// Provides the custom hook for VisitDependency. This will only be called if the dependency
        /// was specified in the whitelist.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="dependency">The <see cref="Dependency" /> to visit.</param>
        /// <returns></returns>
        protected override ReturnCode OnVisitDependency(string directory, Dependency dependency)
        {
            _git.WorkingDirectory = directory;
            if (_dependencyNameToClean.Count > 0)
            {
                foreach (var item in _dependencyNameToClean)
                {
                    if (item == dependency.Configuration.Name)
                    {
                        return _git.Clean(_dryRun, _force, _cleanFiles, _cleanDirectory);
                    }
                }
            }
            else
            {
                return _git.Clean(_dryRun, _force, _cleanFiles, _cleanDirectory);
            }
            return ReturnCode.Success;
        }
    }
}
