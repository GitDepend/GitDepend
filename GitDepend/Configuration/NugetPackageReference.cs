using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDepend.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class NugetPackageReference
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the target framework.
        /// </summary>
        /// <value>
        /// The target framework.
        /// </value>
        public string TargetFramework { get; set; }


    }
}
