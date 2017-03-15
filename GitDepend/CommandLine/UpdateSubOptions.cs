using System.Collections.Generic;
using CommandLine;

namespace GitDepend.CommandLine
{
    /// <summary>
    /// The options for the update verb
    /// </summary>
    public class UpdateSubOptions : NamedDependenciesOptions
    {
        /// <summary>
        /// Forces the build of all projects, regardless of whether existing packages seem to match
        /// what is needed.
        /// </summary>
        [Option('f', "force", DefaultValue = false, HelpText = "Forces dependencies to build before updating packages.")]
        public bool Force { get; set; }
    }
}
