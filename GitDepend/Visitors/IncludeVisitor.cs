using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Configuration;
using GitDepend.Resources;

namespace GitDepend.Visitors
{
    /// <summary>
    /// An implementation of <see cref="IVisitor"/> that creates a key include file for a specified dependency.
    /// </summary>
    public class IncludeVisitor : IVisitor
    {
        private readonly IFileSystem _fileSystem;
        private IList<string> _projectNames;
        private bool _clean;

        /// <summary>
        /// Creates a new <see cref="IncludeVisitor"/>
        /// </summary>
        /// <param name="projectNames">The projects to create the key file in.</param>
        /// /// <param name="clean">Whether or not to remove the key files from the whole dependency stack.</param>
        public IncludeVisitor(IList<string> projectNames, bool clean)
        {
            _projectNames = projectNames;
            _clean = clean;
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
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
        /// Visits a project.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="config">The <see cref="GitDependFile"/> with project configuration information.</param>
        /// <returns>The return code.</returns>
        public ReturnCode VisitProject(string directory, GitDependFile config)
        {
            string filePath = _fileSystem.Path.Combine(directory, ".DepInclude");
            if (_projectNames.Contains(config.Name))
            {
                if (_clean)
                {
                    if (_fileSystem.File.Exists(filePath))
                    {
                        _fileSystem.File.Delete(filePath);
                    }
                }
                else
                {
                    _fileSystem.File.WriteAllText(filePath, string.Empty);
                }
            }
            return ReturnCode = ReturnCode.Success;
        }

        #endregion
    }
}
