using System;
using System.Collections.Generic;
using System.IO;
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
    public class RemoveDependencyVisitor : IVisitor
    {
        private readonly IGitDependFileFactory _factory;
        private readonly string _dependencyNameToRemove;
        private string _foundDependencyDirectory;
        private IFileSystem _fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveDependencyVisitor"/> class.
        /// </summary>
        public RemoveDependencyVisitor(string dependencyNameToRemove)
        {
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _dependencyNameToRemove = dependencyNameToRemove;
            _foundDependencyDirectory = string.Empty;
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
        }

        /// <summary>
        /// The return code
        /// </summary>
        public ReturnCode ReturnCode { get; set; }

        /// <summary>
        /// Visits a project dependency.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="dependency">The <see cref="Dependency" /> to visit.</param>
        /// <returns>
        /// The return code.
        /// </returns>
        public ReturnCode VisitDependency(string directory, Dependency dependency)
        {
            string dir;
            ReturnCode returnCode;

            //visit dependency and get the configuration
            var configFile = _factory.LoadFromDirectory(directory, out dir, out returnCode);
            if (returnCode == ReturnCode.Success)
            {
                if (configFile.Name == _dependencyNameToRemove)
                {
                    _foundDependencyDirectory = directory;
                }
            }

            return ReturnCode.Success;
        }

        /// <summary>
        /// Visits a project, removes the dependency named by the user.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="config">The <see cref="GitDependFile" /> with project configuration information.</param>
        /// <returns>
        /// The return code.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ReturnCode VisitProject(string directory, GitDependFile config)
        {
            if (string.IsNullOrEmpty(_foundDependencyDirectory))
            {
                return ReturnCode.NameDidNotMatchRequestedDependency;
            }
            //visit the project and load the config and delete the configuration.
            int indexToRemove = -1;
            int index = 0;
            foreach (var dep in config.Dependencies)
            {
                var directoryName = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(directory, dep.Directory));
                var dependencyDirectoryName = _fileSystem.Path.GetDirectoryName(_foundDependencyDirectory);
                if (directoryName == dependencyDirectoryName)
                {
                    indexToRemove = index;
                    break;
                }
            }

            config.Dependencies.RemoveAt(indexToRemove);

            return ReturnCode.Success;
        }
    }
}
