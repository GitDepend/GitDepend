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
		private readonly IConsole _console;

		/// <summary>
		/// Creates a new <see cref="GitDependFileFactory"/>
		/// </summary>
		public GitDependFileFactory()
		{
			_fileSystem = DependencyInjection.Resolve<IFileSystem>();
			_console = DependencyInjection.Resolve<IConsole>();
		}

		/// <summary>
		/// Finds a GitDepend.json file in the given directory and loads it into memory.
		/// </summary>
		/// <param name="directory">The directory to start in.</param>
		/// <param name="dir">The directory where GitDepend.json was found.</param>
		/// <param name="code">The return code indicating if the load was successful, or which error occurred.</param>
		/// <returns>A <see cref="GitDependFile"/> or null if none could be loaded.</returns>
		public GitDependFile LoadFromDirectory(string directory, out string dir, out ReturnCode code)
		{
			if (!_fileSystem.Directory.Exists(directory))
			{
				dir = null;
				code = ReturnCode.DirectoryDoesNotExist;
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
						code = ReturnCode.Success;
						dir = current;
						gitDependFile.Build.Script = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(current, gitDependFile.Build.Script));
						gitDependFile.Packages.Directory = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(current, gitDependFile.Packages.Directory));

						foreach (var dependency in gitDependFile.Dependencies)
						{
							dependency.Directory = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(current, dependency.Directory));
							string subdir;
							ReturnCode subcode;
							dependency.Configuration = LoadFromDirectory(dependency.Directory, out subdir, out subcode);
						}
						return gitDependFile;
					}
					catch (Exception ex)
					{
						code = ReturnCode.UnknownError;
						_console.Error.WriteLine(ex.Message);
						return null;
					}
				}
				code = ReturnCode.Success;
				return new GitDependFile();
			}

			code = ReturnCode.GitRepositoryNotFound;
			return null;
		}
	}
}
