using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
    /// <summary>
    /// This visitor will manage the dependencies based on the manage command.
    /// </summary>
    public class ManageDependenciesVisitor : NamedDependenciesVisitor
    {
        /// <summary>
        /// This contains the only name of the dependency that needs to be updated.
        /// </summary>
        public string NameMatchingDirectory;

        private ManageSubOptions _options;

        /// <summary>
        /// Constructor for the ManageDependenciesVisitor class. The options contains the various command options.
        /// </summary>
        /// <param name="options">The options to perform operations on the depend.</param>
        public ManageDependenciesVisitor(ManageSubOptions options) : base(options.Dependencies)
        {
            _options = options;
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
            if (_options.Name == dependency.Configuration.Name)
            {
                NameMatchingDirectory = dependency.Directory;
            }
            return ReturnCode.Success;
        }
    }
}
