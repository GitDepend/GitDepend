using CommandLine;
using CommandLine.Text;
using GitDepend.Commands;
using Newtonsoft.Json;

namespace GitDepend.CommandLine
{
    class Options
    {
        private static Options _default;
        public static Options Default => _default ?? (_default = new Options());

        private Options()
        {

        }

        [VerbOption(CloneCommand.Name, HelpText = "Recursively clones all dependencies")]
        public CloneSubOptions CloneVerb { get; set; }

        [VerbOption(ConfigCommand.Name, HelpText = "Displays the full configuration file")]
        public ConfigSubOptions ConfigVerb { get; set; }

        [VerbOption(InitCommand.Name, HelpText = "Assists you in creating a GitDepend.json")]
        public InitSubOptions InitVerb { get; set; }

        [VerbOption(ListCommand.Name, HelpText = "Lists all repository dependencies")]
        public ListSubOptons ListVerb { get; set; }

        [VerbOption(StatusCommand.Name, HelpText = "Displays git status on dependencies")]
        public StatusSubOptions StatusVerb { get; set; }

        [VerbOption(UpdateCommand.Name, HelpText = "Recursively builds all dependencies, and updates the current project to the newly built artifacts.")]
        public UpdateSubOptions UpdateVerb { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }

        #region Overrides of Object

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        #endregion
    }
}
