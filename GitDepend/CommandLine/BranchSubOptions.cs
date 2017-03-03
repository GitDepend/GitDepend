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

        /// <summary>
        /// A flag indicating if the specified branch should be deleted.
        /// </summary>
        [Option('d', "delete", DefaultValue = false, HelpText = "Indicates that the specified branch should be deleted.", MutuallyExclusiveSet = "op")]
        public bool Delete { get; set; }

        /// <summary>
        /// A flag indicating if the specified branch should be deleted regardless of whether it has been merged or not.
        /// </summary>
        [Option("force", DefaultValue = false, HelpText = "Indicates that the specified branch should be delete, regardless of whether it has been merged or not.")]
        public bool ForceDelete { get; set; }

        /// <summary>
        /// A flat indicating that all merged branches should be listed.
        /// </summary>
        [Option("merged", DefaultValue = false, HelpText = "List all merged branches", MutuallyExclusiveSet = "op")]
        public bool ListMerged { get; set; }
    }
}
