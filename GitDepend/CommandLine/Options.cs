using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using GitDepend.CommandLine;
using GitDepend.Commands;
using Newtonsoft.Json;

namespace GitDepend
{
	class Options
	{
		private static Options _default;
		public static Options Default => _default ?? (_default = new Options());

		private Options()
		{
			
		}

		[VerbOption(ShowConfigCommand.Name, HelpText = "Displays the full configuration file")]
		public ConfigSubOptions ConfigVerb { get; set; }

		[VerbOption(InitCommand.Name, HelpText = "Assists you in creating a GitDepend.json")]
		public InitSubOptions InitVerb { get; set; }

		[VerbOption(CloneCommand.Name, HelpText = "Recursively clones all dependencies")]
		public CloneSubOptions CloneVerb { get; set; }

		[ParserState]
		public IParserState LastParserState { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
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
