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
        private readonly IList<string> _dependencyNamesToRemove;
        /// <summary>
        /// This contains the dependency directory that was found.
        /// </summary>
        public List<string> FoundDependencyDirectories;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveDependencyVisitor"/> class.
        /// </summary>
        public RemoveDependencyVisitor(IList<string> dependencyNamesToRemove) : base(dependencyNamesToRemove)
        {
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _dependencyNamesToRemove = dependencyNamesToRemove;
            FoundDependencyDirectories = new List<string>();
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
            var configFile = _factory.LoadFromDirectory(dependency.Directory, out dir, out returnCode);
            if (returnCode == ReturnCode.Success)
            {
                foreach (var dependencyName in _dependencyNamesToRemove)
                {
                    if (string.Equals(configFile.Name, dependencyName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        FoundDependencyDirectories.Add(dir);
                    }
                }
            }

            return ReturnCode.Success;
        }
    }
}
