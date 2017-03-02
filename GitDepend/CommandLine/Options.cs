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

        [VerbOption(BranchCommand.Name, HelpText = "List, create, or delete branches")]
        public BranchSubOptions BranchVerb { get; set; }

        [VerbOption(CheckOutCommand.Name, HelpText = "Switch branches")]
        public CheckOutSubOptions CheckOutVerb { get; set; }

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

        [VerbOption(SyncCommand.Name, HelpText = "Sets the referenced branch to the currently checked out branch on dependencies.")]
        public SyncSubOptions SyncVerb { get; set; }

        [VerbOption(UpdateCommand.Name, HelpText = "Recursively builds all dependencies, and updates the current project to the newly built artifacts.")]
        public UpdateSubOptions UpdateVerb { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var versionUpdateChecker = DependencyInjection.Resolve<IVersionUpdateChecker>();

            var appendString = versionUpdateChecker.CheckVersion("gitdepend", "kjjuno");
            appendString += versionUpdateChecker.CheckVersion("choco", "chocolatey");

            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current)) + appendString;
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
