using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.CommandLine;
using GitDepend.Configuration;

namespace GitDepend.Commands
{
	class InitCommand : ICommand
	{
		private readonly InitSubOptions _options;
		public const string Name = "init";

		public InitCommand(InitSubOptions options)
		{
			_options = options;
		}

		#region Implementation of ICommand

		public int Execute()
		{
			string dir;
			string error;
			var config = GitDependFile.LoadFromDir(_options.Directory, out dir, out error) ?? new GitDependFile();

			Console.Write($"build script [{config.Build.Script}]: ");
			config.Build.Script = ReadLine(config.Build.Script);

			Console.Write($"artifacts dir [{config.Packages.Directory}]: ");
			config.Packages.Directory = ReadLine(config.Packages.Directory);

			var path = Path.Combine(dir, "GitDepend.json");

			File.WriteAllText(path, config.ToString());

			return ReturnCodes.Success;
		}

		private static string ReadLine(string defaultValue)
		{
			var input = Console.ReadLine();
			if (string.IsNullOrEmpty(input))
			{
				input = defaultValue;
			}
			return input;
		}

		#endregion
	}
}
