using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace GitDepend.CommandLine
{
    /// <summary>
    /// The options for the checkout verb
    /// </summary>
    public class BranchSubOptions : CommonSubOptions
    {
        /// <summary>
        /// The name of the branch to create.
        /// </summary>
        [ValueOption(0)]
        public string BranchName { get; set; }
    }
}
