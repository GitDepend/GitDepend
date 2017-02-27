using CommandLine;

namespace GitDepend.CommandLine
{
    /// <summary>
    /// The options for the checkout verb
    /// </summary>
    public class CheckOutSubOptions : CommonSubOptions
    {
        /// <summary>
        /// The name of the branch to switch to.
        /// </summary>
        [ValueOption(0)]
        public string BranchName { get; set; }

        /// <summary>
        /// A flag indicating if a new branch should be created.
        /// </summary>
        [Option('b', "create", DefaultValue = false, HelpText = "Create a new branch")]
        public bool CreateBranch { get; set; }
    }
}