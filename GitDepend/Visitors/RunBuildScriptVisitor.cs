using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
	public class RunBuildScriptVisitor : IVisitor
	{
		private static readonly Regex Pattern = new Regex(@"^(?<id>.*?)\.(?<version>(?:\d\.){2,3}\d(?:-.*?)?)$", RegexOptions.Compiled);

		#region Implementation of IVisitor

		public int ReturnCode { get; set; }
		public int VisitDependency(Dependency dependency)
		{
			string dir;
			string error;
			var config = GitDependFile.LoadFromDir(dependency.Directory, out dir, out error);

			if (config == null)
			{
				return ReturnCodes.GitRepositoryNotFound;
			}

			var info = new ProcessStartInfo(Path.Combine(dependency.Directory, config.Build.Script), config.Build.Arguments)
			{
				WorkingDirectory = dependency.Directory,
				UseShellExecute = false
			};
			var proc = Process.Start(info);
			proc?.WaitForExit();
			return proc?.ExitCode ?? ReturnCodes.FailedToRunBuildScript;
		}

		public int VisitProject(string directory, GitDependFile config)
		{
			foreach (var dependency in config.Dependencies)
			{
				var dir = dependency.Configuration.Packages.Directory;
				foreach (var file in Directory.GetFiles(dir, "*.nupkg"))
				{
					var name = Path.GetFileNameWithoutExtension(file);


					if (!string.IsNullOrEmpty(name))
					{
						var match = Pattern.Match(name);

						if (match.Success)
						{
							var id = match.Groups["id"].Value;
							var version = match.Groups["version"].Value;

							var nugetConfig = Path.Combine(directory, "NuGet.config");
							if (!File.Exists(nugetConfig))
							{
								nugetConfig = null;
							}

							var nuget = new Nuget(nugetConfig);

							foreach (var solution in Directory.GetFiles(directory, "*.sln", SearchOption.AllDirectories))
							{
								nuget.Update(solution, id, version, dependency.Configuration.Packages.Directory);
							}
						}
					}
				}
			}

			var git = new Git(directory);
			git.Add("*.csproj", @"*\packages.config");

			return ReturnCodes.Success;
		}

		#endregion
	}
}
