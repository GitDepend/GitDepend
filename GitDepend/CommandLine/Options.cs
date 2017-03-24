using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using CommandLine;
using CommandLine.Text;
using GitDepend.Busi;
using GitDepend.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace GitDepend.CommandLine
{
    class Options
    {
        private static Options _default;
        public static Options Default => _default ?? (_default = new Options());

        private Options()
        {
        }

        [VerbOption(AddCommand.Name, HelpText = "Add a dependency to an existing configuration")]
        public AddSubOptions AddVerb { get; set; } = new AddSubOptions();

        [VerbOption(BranchCommand.Name, HelpText = "List, create, or delete branches")]
        public BranchSubOptions BranchVerb { get; set; } = new BranchSubOptions();

        [VerbOption(CheckOutCommand.Name, HelpText = "Switch branches")]
        public CheckOutSubOptions CheckOutVerb { get; set; } = new CheckOutSubOptions();

        [VerbOption(CleanCommand.Name, HelpText = "Remove named or all dependencies")]
        public CleanSubOptions CleanVerb { get; set; } = new CleanSubOptions();

        [VerbOption(CloneCommand.Name, HelpText = "Recursively clones all dependencies")]
        public CloneSubOptions CloneVerb { get; set; } = new CloneSubOptions();

        [VerbOption(ConfigCommand.Name, HelpText = "Displays the full configuration file")]
        public ConfigSubOptions ConfigVerb { get; set; } = new ConfigSubOptions();

        [VerbOption(InitCommand.Name, HelpText = "Assists you in creating a GitDepend.json")]
        public InitSubOptions InitVerb { get; set; } = new InitSubOptions();

        [VerbOption(ListCommand.Name, HelpText = "Lists all repository dependencies")]
        public ListSubOptons ListVerb { get; set; } = new ListSubOptons();

        [VerbOption(ManageCommand.Name, HelpText = "Manage dependency url, directory, branch in config.")]
        public ManageSubOptions ManageVerb { get; set; } = new ManageSubOptions();

        [VerbOption(PushCommand.Name, HelpText = "Performs a git push on all of the dependencies.")]
        public PushSubOptions PushVerb { get; set; } = new PushSubOptions();

        [VerbOption(RemoveCommand.Name, HelpText = "Removes a dependency based on its name.")]
        public RemoveSubOptions RemoveVerb { get; set; } = new RemoveSubOptions();

        [VerbOption(StatusCommand.Name, HelpText = "Displays git status on dependencies")]
        public StatusSubOptions StatusVerb { get; set; } = new StatusSubOptions();

        [VerbOption(SyncCommand.Name, HelpText = "Sets the referenced branch to the currently checked out branch on dependencies.")]
        public SyncSubOptions SyncVerb { get; set; } = new SyncSubOptions();

        [VerbOption(UpdateCommand.Name, HelpText = "Recursively builds all dependencies, and updates the current project to the newly built artifacts.")]
        public UpdateSubOptions UpdateVerb { get; set; } = new UpdateSubOptions();

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var versionUpdateChecker = DependencyInjection.Resolve<IVersionUpdateChecker>();

            var appendString = versionUpdateChecker.CheckVersion("gitdepend", "kjjuno");

            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current)) + appendString;
        }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            var args = Environment.GetCommandLineArgs();

            if (args.Length == 3 && string.Equals(args[1], "help", StringComparison.OrdinalIgnoreCase))
            {
                verb = args[2];
            }

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
