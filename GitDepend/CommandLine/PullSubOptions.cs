using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace GitDepend.CommandLine
{
    /// <summary>
    /// These options correspond to the pull command
    /// </summary>
    public class PullSubOptions : NamedDependenciesOptions
    {
        /// <summary>
        /// The arguments to be sent to the pull command in git.
        /// </summary>
        [Option('p', "argList", HelpText = "Any arguments you would like to give to the pull command.")]
        public IList<string> PullArguments { get; set; }
    }
}
