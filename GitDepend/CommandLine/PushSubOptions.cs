using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace GitDepend.CommandLine
{
    /// <summary>
    /// The options for push command
    /// </summary>
    public class PushSubOptions : NamedDependenciesOptions
    {
        /// <summary>
        /// The arguments which can be run by git push
        /// </summary>
        [Option('a', "args", HelpText = "The arguments to be run with git push")]
        public IList<string> PushArguments { get; set; }
    }
}
