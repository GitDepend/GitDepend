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
          /// The arguments to be provided to the git pull command
          /// </summary>
          /// [Option("args", HelpText = "The arguments to be provided to the git command")]
          public IList<string> GitArguments { get; set; }
    }
}
