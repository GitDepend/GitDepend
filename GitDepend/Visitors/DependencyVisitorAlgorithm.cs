using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using GitDepend.Busi;

namespace GitDepend.Visitors
{
	/// <summary>
	/// Implements the visitor algorithm for visiting all repository dependencies. You are guarunteed
	/// to visit all project dependencies before visiting the project.
	/// </summary>
	public class DependencyVisitorAlgorithm
	{
		private readonly IGitDependFileFactory _factory;
		private readonly IGit _git;
		private readonly IFileSystem _fileSystem;
		private readonly HashSet<string> _visitedDependencies = new HashSet<string>();
		private readonly HashSet<string> _visitedProjects = new HashSet<string>();

		/// <summary>
		/// Creates a new <see cref="DependencyVisitorAlgorithm"/>
		/// </summary>
		/// <param name="factory">The <see cref="IGitDependFileFactory"/> to use.</param>
		/// <param name="git">The <see cref="IGit"/> to use.</param>
		/// <param name="fileSystem">The <see cref="IFileSystem"/> to use.</param>
		public DependencyVisitorAlgorithm(IGitDependFileFactory factory, IGit git, IFileSystem fileSystem)
		{
			_factory = factory;
			_git = git;
			_fileSystem = fileSystem;
		}

		/// <summary>
		/// Traverses all dependencies beginning in the given directory.
		/// </summary>
		/// <param name="visitor">The <see cref="IVisitor"/> that should be called at each dependency and project visit.</param>
		/// <param name="directory">The directory containing a GitDepend.json file.</param>
		public void TraverseDependencies(IVisitor visitor, string directory)
		{
			// Make sure we are working with something here.
			if (visitor == null || string.IsNullOrEmpty(directory))
			{
				return;
			}

			directory = _fileSystem.Path.GetFullPath(directory);

			// If the directory doesn't exist we are done.
			if (!_fileSystem.Directory.Exists(directory))
			{
				visitor.ReturnCode = ReturnCode.GitRepositoryNotFound;
				return;
			}

			string dir;
			string error;
			var config = _factory.LoadFromDirectory(directory, out dir, out error);

			if (config == null)
			{
				Console.Error.WriteLine("Could not find GitDepend.json");
				visitor.ReturnCode = ReturnCode.GitDependFileNotFound;
				return;
			}

			ReturnCode code;

			foreach (var dependency in config.Dependencies)
			{
				dependency.Directory = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(dir, dependency.Directory));

				// If the dependency does not exist on disk we need to clone it.
				if (!_fileSystem.Directory.Exists(dependency.Directory))
				{
					Console.WriteLine($"Cloning {dependency.Name} into {dependency.Directory}");

					code = _git.Clone(dependency.Url, dependency.Directory, dependency.Branch);
					Console.WriteLine();

					// If something went wrong with git we are done.
					if (code != ReturnCode.Success)
					{
						visitor.ReturnCode = code;
						return;
					}
				}

				// Visit all dependencies of this dependency.
				TraverseDependencies(visitor, dependency.Directory);

				// If something went wrong with a dependency we are done.
				if (visitor.ReturnCode != ReturnCode.Success)
				{
					return;
				}

				// Make sure to only visit dependencies once.
				if (!_visitedDependencies.Contains(dependency.Directory))
				{
					_visitedDependencies.Add(dependency.Directory);

					// Visit the dependency.
					code = visitor.VisitDependency(dependency);

					// If something went wrong visiting the dependency we are done.
					if (code != ReturnCode.Success)
					{
						visitor.ReturnCode = code;
						return;
					}
				}
			}

			// Make sure to only visit projects once.
			if (!_visitedProjects.Contains(dir))
			{
				_visitedProjects.Add(dir);

				// Visit the project.
				code = visitor.VisitProject(dir, config);
				Console.WriteLine();

				// If something went wrong visiting the project we are done.
				if (code != ReturnCode.Success)
				{
					visitor.ReturnCode = code;
				}
			}
		}
	}
}
