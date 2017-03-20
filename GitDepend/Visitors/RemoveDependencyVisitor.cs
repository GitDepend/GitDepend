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
    public class RemoveDependencyVisitor : NamedDependenciesVisitor
    {
        private readonly IGitDependFileFactory _factory;
        private readonly string _dependencyNameToRemove;
        /// <summary>
        /// This contains the dependency directory that was found.
        /// </summary>
        public string FoundDependencyDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveDependencyVisitor"/> class.
        /// </summary>
        public RemoveDependencyVisitor(string dependencyNameToRemove) : base(new List<string>{ dependencyNameToRemove})
        {
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _dependencyNameToRemove = dependencyNameToRemove;
            FoundDependencyDirectory = string.Empty;
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
            string dir;
            ReturnCode returnCode;

            //visit dependency and get the configuration
            var configFile = _factory.LoadFromDirectory(directory, out dir, out returnCode);
            if (returnCode == ReturnCode.Success)
            {
                if (configFile.Name == _dependencyNameToRemove)
                {
                    FoundDependencyDirectory = directory;
                }
            }

            return ReturnCode.Success;
        }
    }
}
