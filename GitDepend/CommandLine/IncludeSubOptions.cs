using System.Collections;
using System.Collections.Generic;
using CommandLine;
using Newtonsoft.Json;

namespace GitDepend.CommandLine
{
    /// <summary>
    /// The options for the init verb
    /// </summary>
    public class IncludeSubOptions : CommonSubOptions
    {
        /// <summary>
        /// The arguments to be provided to the git pull command
        /// </summary>
        [Option('c', "clear", DefaultValue = false, HelpText = "Reset stack to include all files.")]
        public bool Clear { get; set; }

        /// <summary>
        /// The name of the dependency to include.
        /// </summary>
        [ValueList(typeof(List<string>))]
        public IList<string> DepNames { get; set; }
    }
}
