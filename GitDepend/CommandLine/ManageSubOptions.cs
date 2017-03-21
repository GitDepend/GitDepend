using CommandLine;


namespace GitDepend.CommandLine
{
    /// <summary>
    /// Contains the options for the Manage command.
    /// </summary>
    /// <seealso cref="GitDepend.CommandLine.CommonSubOptions" />
    public class ManageSubOptions : NamedDependenciesOptions
    {
        /// <summary>
        /// The identifier of the dependency to update.
        /// </summary>
        [Option('n', "name", Required = false, HelpText = "Name of the dependency to manage.")]
        public string Name { get; set; }
        
        /// <summary>
        /// Sets the url for the given dependency in the GitDepend.json file.
        /// </summary>
        [Option('u', "url", Required = false, HelpText = "Sets the url for the given dependency.")]
        public string SetUrl { get; set; }

        /// <summary>
        /// Sets the directory for the given dependency in the GitDepend.Json file.
        /// </summary>
        [Option('d', "dir", Required = false, HelpText = "Sets the directory for the given dependency.")]
        public string SetDirectory { get; set; }

        /// <summary>
        /// Sets the branch for the given dependency, or updates all.
        /// </summary>
        [Option('b', "branch", Required = false, HelpText = "Sets the branch for the given named dependency")]
        public string SetBranch { get; set; }
    }
}
