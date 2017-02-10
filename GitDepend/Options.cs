using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
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

		[Option('d', "dir", Required = false, HelpText = "The directory to process. The current working directory will be used if this option is ignored.")]
		public string Directory { get; set; }

		[Option("showconfig", Required = false, HelpText = "Displays the full configuration file")]
		public bool ShowConfig { get; set; }

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
