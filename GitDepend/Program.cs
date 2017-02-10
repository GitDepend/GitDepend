using System;
using System.IO;
using GitDepend.Configuration;

namespace GitDepend
{
	class Program
	{
		static void Main(string[] args)
		{
			if (!CommandLine.Parser.Default.ParseArguments(args, Options.Default))
			{
				return;
			}

			if (string.IsNullOrEmpty(Options.Default.Directory))
			{
				Options.Default.Directory = Environment.CurrentDirectory;
			}
			else
			{
				Options.Default.Directory = Path.GetFullPath(Options.Default.Directory);
			}

			string error;
			var file = GitDependFile.LoadFromDir(Options.Default.Directory, out error);

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

			if (Options.Default.ShowConfig)
			{
				Console.WriteLine(file);
				return;
			}
		}
	}
}
