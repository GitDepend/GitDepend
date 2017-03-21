using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace GitDepend.CommandLine
{
    /// <summary>
    /// SubOptions for the Add class.
    /// </summary>
    public class AddSubOptions : CommonSubOptions
    {
        /// <summary>
        /// Used to update the url for 
        /// </summary>
        [Option('u', "url", Required = true, HelpText = "The url for the dependency")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the directory.
        /// </summary>
        [Option('d', "directory", Required = true, HelpText = "The directory for the dependency")]
        public string DependencyDirectory { get; set; }

        /// <summary>
        /// Gets or sets the branch.
        /// </summary>
        [Option('b', "branch", Required = true, HelpText = "The branch checked out for the dependency")]
        public string Branch { get; set; }
    }
}
