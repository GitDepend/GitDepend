using CommandLine;

namespace GitDepend.CommandLine
{
    /// <summary>
    /// The options for the list verb
    /// </summary>
    public class ListSubOptons : CommonSubOptions
    {
        /// <summary>
        /// Signals to output more information than the default setting
        /// what is needed.
        /// </summary>
        [Option('v', "verbose", DefaultValue = false, HelpText = "Outputs more information than the default.")]
        public bool Verbose { get; set; }

        /// <summary>
        /// Flag to output entire list regardless of limit files
        /// </summary>
        [Option('a', "all", DefaultValue = false, HelpText = "Outputs more information than the default.")]
        public bool All { get; set; }
    }
}
