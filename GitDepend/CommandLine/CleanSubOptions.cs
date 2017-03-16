using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace GitDepend.CommandLine
{
    /// <summary>
    /// The options that can be a part of the clean command
    /// </summary>
    /// <seealso cref="GitDepend.CommandLine.CommonSubOptions" />
    public class CleanSubOptions : NamedDependenciesOptions
    {
        /// <summary>
        /// Tells git to tell you all the things that it will remove
        /// </summary>
        [Option('n', DefaultValue = false, HelpText = "Perform a dry run")]
        public bool DryRun { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CleanSubOptions"/> is force.
        /// </summary>
        /// <value>
        ///   <c>true</c> if force; otherwise, <c>false</c>.
        /// </value>
        [Option('f', DefaultValue = false, HelpText = "Force -- similar to -f argument in git")]
        public bool Force { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [remove untracked directories].
        /// </summary>
        /// <value>
        /// <c>true</c> if [remove untracked directories]; otherwise, <c>false</c>.
        /// </value>
        [Option('d', DefaultValue = false, HelpText = "Remove untracked directories -- performs -d argument in git")]
        public bool RemoveUntrackedDirectories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [remove untracked files].
        /// </summary>
        /// <value>
        /// <c>true</c> if [remove untracked files]; otherwise, <c>false</c>.
        /// </value>
        [Option('x', DefaultValue = false, HelpText = "Removes untracked files -- performs -x argument in git")]
        public bool RemoveUntrackedFiles { get; set; }
    }
}
