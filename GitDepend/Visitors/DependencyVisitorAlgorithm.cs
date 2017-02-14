using System;
using System.Collections.Generic;
using System.IO;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
	/// <summary>
	/// Implements the visitor algorithm for visiting all repository dependencies. You are guarunteed
	/// to visit all project dependencies before visiting the project.
	/// </summary>
	public class DependencyVisitorAlgorithm
	{
		private readonly HashSet<string> _visitedDependencies = new HashSet<string>();
		private readonly HashSet<string> _visitedProjects = new HashSet<string>();

		/// <summary>
		/// Traverses all dependencies beginning in the given directory.
		/// </summary>
		/// <param name="visitor">The <see cref="IVisitor"/> that should be called at each dependency and project visit.</param>
		/// <param name="directory">The directory containing a GitDepend.json file.</param>
		public void TraverseDependencies(IVisitor visitor, string directory)
		{
			if (visitor == null || string.IsNullOrEmpty(directory))
			{
				return;
			}

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

				if (!_visitedDependencies.Contains(dependency.Directory))
				{
					_visitedDependencies.Add(dependency.Directory);
					code = visitor.VisitDependency(dependency);

					if (code != ReturnCodes.Success)
					{
						return;
					}
				}
			}

			if (!_visitedProjects.Contains(dir))
			{
				_visitedProjects.Add(dir);
				visitor.VisitProject(dir, config);
				Console.WriteLine();
			}
		}
	}
}
