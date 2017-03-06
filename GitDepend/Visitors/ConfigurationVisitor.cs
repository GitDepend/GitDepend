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
    public class ConfigurationVisitor : IVisitor
    {
        private readonly IGitDependFileFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationVisitor"/> class.
        /// </summary>
        public ConfigurationVisitor()
        {
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();

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
            _factory.LoadFromDirectory(directory, out dir, out returnCode);


        }

        /// <summary>
        /// Visits a project.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="config">The <see cref="GitDependFile" /> with project configuration information.</param>
        /// <returns>
        /// The return code.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ReturnCode VisitProject(string directory, GitDependFile config)
        {
            //visit the project and load the config and delete the configuration.
            throw new NotImplementedException();
        }
    }
}
