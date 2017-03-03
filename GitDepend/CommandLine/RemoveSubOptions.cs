using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Newtonsoft.Json;

namespace GitDepend.CommandLine
{
    /// <summary>
    /// SubOptions for the Remove Command
    /// </summary>
    /// <seealso cref="GitDepend.CommandLine.CommonSubOptions" />
    public class RemoveSubOptions : CommonSubOptions
    {
        /// <summary>
        /// The name of the dependency to remove. Use list command 
        /// </summary>
        [Option('n', "name", Required = true, HelpText = "The name of the dependency to remove.")]
        public string DependencyName { get; set; }
    }
}
