using System;
using System.IO;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
	public class DependencyVisitorAlgorithm
	{
		public void TraverseDependencies(IVisitor visitor, string directory)
		{
			directory = Path.GetFullPath(directory);

			if (!Directory.Exists(directory))
			{
				visitor.ReturnCode = ReturnCodes.GitRepositoryNotFound;
				return;
			}

			string dir;
			string error;
			var config = GitDependFile.LoadFromDir(directory, out dir, out error);

			if (config == null)
			{
				Console.Error.WriteLine("Could not find GitDepend.json");
				visitor.ReturnCode = ReturnCodes.GitDependFileNotFound;
				return;
			}

			foreach (var dependency in config.Dependencies)
			{
				int code;
				dependency.Directory = Path.GetFullPath(Path.Combine(dir, dependency.Directory));

				if (!Directory.Exists(dependency.Directory))
				{
					Console.WriteLine($"Cloning {dependency.Name} into {dependency.Directory}");
					var git = new Git();

					code = git.Clone(dependency.Url, dependency.Directory, dependency.Branch);
					Console.WriteLine();
					if (code != ReturnCodes.Success)
					{
						visitor.ReturnCode = code;
						return;
					}
				}

				TraverseDependencies(visitor, dependency.Directory);

				code = visitor.VisitDependency(dependency);
				Console.WriteLine();

				if (code != ReturnCodes.Success)
				{
					return;
				}
			}

			visitor.VisitProject(dir, config);
		}
	}
}
