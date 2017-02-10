using System;
using GitDepend.CommandLine;
using GitDepend.Configuration;

namespace GitDepend.Commands
{
	class ShowConfigCommand : ICommand
	{
		private readonly ConfigSubOptions _options;
		public const string Name = "config";

		public ShowConfigCommand(ConfigSubOptions options)
		{
			_options = options;
		}

		#region Implementation of ICommand

		public int Execute()
		{
			string dir;
			string error;
			var file = GitDependFile.LoadFromDir(_options.Directory, out dir, out error);

			if (file == null || !string.IsNullOrEmpty(error))
			{
				if (string.IsNullOrEmpty(error))
				{
					Console.Error.WriteLine("I'm not sure why, but I can't load the GitDepend.json file");
					Environment.Exit(1);
				}
				else
				{
					Console.Error.WriteLine(error);
					Environment.Exit(2);
				}
			}

			Console.WriteLine(file);
			return ReturnCodes.Success;
		}

		#endregion
	}
}
