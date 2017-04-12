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
          [Option("args", Required = false, HelpText = "The arguments to be passed to the pull command for each dependancy.")]
          public string GitArguments { get; set; }
    }
}
