using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.CommandLine;
using GitDepend.Configuration;

namespace GitDepend.Commands
{
	public class CloneCommand : ICommand
	{
		public const string Name = "clone";
		private readonly CloneSubOptions _options;

		public CloneCommand(CloneSubOptions options)
		{
			_options = options;
		}

		#region Implementation of ICommand

		public int Execute()
		{
			var code = CloneDependencies(_options.Directory);

			if (code == ReturnCodes.Success)
			{
				Console.WriteLine("Successfully cloned all dependencies");
			}

			return code;
		}

		private static int CloneDependencies(string directory)
		{
			directory = Path.GetFullPath(directory);

			if (!Directory.Exists(directory))
			{
				return ReturnCodes.GitRepositoryNotFound;
			}

			string dir;
			string error;
			var config = GitDependFile.LoadFromDir(directory, out dir, out error);

			if (config == null)
			{
				Console.Error.WriteLine("Could not find GitDepend.json");
				return ReturnCodes.GitDependFileNotFound;
			}

			foreach (var dependency in config.Dependencies)
			{
				var dependencyDir = Path.GetFullPath(Path.Combine(dir, dependency.Directory));

				if (!Directory.Exists(dependencyDir))
				{
					Console.WriteLine($"Cloning {dependency.Name} into {dependencyDir}");

					var info = new ProcessStartInfo("git", $"clone {dependency.Url} {dependencyDir} -b {dependency.Branch}")
					{
						UseShellExecute = false,
					};
					var proc = Process.Start(info);
					proc?.WaitForExit();
					Console.WriteLine();
				}
				else
				{
					Console.WriteLine($"Checking out the {dependency.Branch} branch on {dependency.Name}");

					var info = new ProcessStartInfo("git", $"checkout {dependency.Branch}")
					{
						WorkingDirectory = dependencyDir,
						UseShellExecute = false,
					};
					var proc = Process.Start(info);
					proc?.WaitForExit();
					Console.WriteLine();
					Console.WriteLine($"{dependencyDir} already exists");
				}

				var depCode = CloneDependencies(dependencyDir);

				if (depCode != ReturnCodes.Success)
				{
					return depCode;
				}
			}

			return ReturnCodes.Success;
		}

		#endregion
	}
}
