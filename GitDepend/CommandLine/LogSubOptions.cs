using CommandLine;

namespace GitDepend.CommandLine
{
    /// <summary>
    /// These options correspond to the log command
    /// </summary>
    public class LogSubOptions : NamedDependenciesOptions
    {
        /// <summary>
        /// The arguments to be provided to the git log command
        /// </summary>
        [Option("args", Required = false, HelpText = "The arguments to be passed to the log command for each dependancy.")]
        public string GitArguments { get; set; }
    }
}
