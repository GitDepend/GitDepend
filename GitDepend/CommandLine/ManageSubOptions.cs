using CommandLine;


namespace GitDepend.CommandLine
{
    /// <summary>
    /// Contains the options for the Manage command.
    /// </summary>
    /// <seealso cref="GitDepend.CommandLine.CommonSubOptions" />
    public class ManageSubOptions : CommonSubOptions
    {
        /// <summary>
        /// Gets or sets the name of the dependency.
        /// </summary>
        [Option("depname", Required= false, HelpText = "The name of the dependency you want to update/remove. Leave out if you want to apply to all dependencies.")]
        public string DependencyName { get; set; }

        /// <summary>
        /// Adds a dependency to the GitDepend.json file, based on the other dependencyname.
        /// </summary>
        [Option("add", Required = false, HelpText = "This is to add a dependency to the current directory or the dependency named.")]
        public string Add { get; set; }

        /// <summary>
        /// Removes a dependencyin the GitDepend.json file, based on the dependencyname.
        /// </summary>
        [Option("remove", Required = false, HelpText = "Removes the dependency given by its name. If no name is provided, no deletion occurs")]
        public string Remove { get; set; }

        /// <summary>
        /// Sets the url for the given dependency in the GitDepend.json file.
        /// </summary>
        [Option("url", Required = false, HelpText = "Sets the url for the given dependency.")]
        public string SetUrl { get; set; }

        /// <summary>
        /// Sets the directory for the given dependency in the GitDepend.Json file.
        /// </summary>
        [Option("dir", Required = false, HelpText = "Sets the directory for the given dependency.")]
        public string SetDirectory { get; set; }

        /// <summary>
        /// Sets the branch for the given dependency, or updates all.
        /// </summary>
        [Option("branch", Required = false, HelpText = "Sets the branch for the given named dependency, or updates all if no depname is given")]
        public string SetBranch { get; set; }
    }
}
