using System;
using System.IO.Abstractions;
using System.Linq;
using GitDepend.Configuration;
using Newtonsoft.Json;

namespace GitDepend.Busi
{
	/// <summary>
	/// Factory class that loads a <see cref="GitDependFile"/> from a file on disk.
	/// </summary>
	public class GitDependFileFactory : IGitDependFileFactory
	{
		private readonly IFileSystem _fileSystem;

		/// <summary>
		/// Creates a new <see cref="GitDependFileFactory"/>
		/// </summary>
		/// <param name="fileSystem">The <see cref="IFileSystem"/> to use.</param>
		public GitDependFileFactory(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
		}

		/// <summary>
		/// Finds a GitDepend.json file in the given directory and loads it into memory.
		/// </summary>
		/// <param name="directory">The directory to start in.</param>
		/// <param name="dir">The directory where GitDepend.json was found.</param>
		/// <param name="error">An error string indicating what went wrong in the case that the file could not be loaded.</param>
		/// <returns>A <see cref="GitDependFile"/> or null if none could be loaded.</returns>
		public GitDependFile LoadFromDirectory(string directory, out string dir, out string error)
		{
			if (!_fileSystem.Directory.Exists(directory))
			{
				dir = null;
				error = $"{directory} does not exist";
				return null;
			}

			dir = directory;
			var current = directory;
			bool isGitRoot;
			do
			{
				isGitRoot = _fileSystem.Directory.GetDirectories(current, ".git").Any();

				if (!isGitRoot)
				{
					current = _fileSystem.Directory.GetParent(current)?.FullName;
				}

			} while (!string.IsNullOrEmpty(current) && !isGitRoot);


			if (!string.IsNullOrEmpty(current) && isGitRoot)
			{
				var file = _fileSystem.Path.Combine(current, "GitDepend.json");

				if (_fileSystem.File.Exists(file))
				{
					try
					{
						var json = _fileSystem.File.ReadAllText(file);
						var gitDependFile = JsonConvert.DeserializeObject<GitDependFile>(json);
						error = null;
						dir = current;
						gitDependFile.Build.Script = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(current, gitDependFile.Build.Script));
						gitDependFile.Packages.Directory = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(current, gitDependFile.Packages.Directory));

						foreach (var dependency in gitDependFile.Dependencies)
						{
							dependency.Directory = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(current, dependency.Directory));
							string subdir;
							string suberror;
							dependency.Configuration = LoadFromDirectory(dependency.Directory, out subdir, out suberror);
						}
						return gitDependFile;
					}
					catch (Exception ex)
					{
						error = ex.Message;
						Console.Error.WriteLine(ex.Message);
						return null;
					}
				}
				error = null;
				return new GitDependFile();
			}

			error = "This is not a git repository";
			return null;
		}
	}
}
