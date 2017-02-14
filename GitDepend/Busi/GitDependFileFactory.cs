using System;
using System.IO;
using System.Linq;
using GitDepend.Configuration;
using Newtonsoft.Json;

namespace GitDepend.Busi
{
	/// <summary>
	/// Factory class that loads a <see cref="GitDependFile"/> from a file on disk.
	/// </summary>
	public class GitDependFileFactory
	{
		private readonly IFileIo _fileIo;

		/// <summary>
		/// Creates a new <see cref="GitDependFileFactory"/>
		/// </summary>
		/// <param name="fileIo">The <see cref="IFileIo"/> to use.</param>
		public GitDependFileFactory(IFileIo fileIo)
		{
			_fileIo = fileIo;
		}

		/// <summary>
		/// Finds a GitDepend.json file in the given directory and loads it into memory.
		/// </summary>
		/// <param name="directory">The directory to start in.</param>
		/// <param name="dir">The directory where GitDepend.json was found.</param>
		/// <param name="error">An error string indicating what went wrong in the case that the file could not be loaded.</param>
		/// <returns></returns>
		public GitDependFile LoadFromDirectory(string directory, out string dir, out string error)
		{
			if (!_fileIo.DirectoryExists(directory))
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
				isGitRoot = _fileIo.GetDirectories(current, ".git").Any();

				if (!isGitRoot)
				{
					current = _fileIo.GetParentDirectory(current)?.FullName;
				}

			} while (!string.IsNullOrEmpty(current) && !isGitRoot);


			if (!string.IsNullOrEmpty(current) && isGitRoot)
			{
				var file = Path.Combine(current, "GitDepend.json");

				if (_fileIo.FileExists(file))
				{
					try
					{
						var json = File.ReadAllText(file);
						var gitDependFile = JsonConvert.DeserializeObject<GitDependFile>(json);
						error = null;
						dir = current;
						gitDependFile.Build.Script = Path.GetFullPath(Path.Combine(current, gitDependFile.Build.Script));
						gitDependFile.Packages.Directory = Path.GetFullPath(Path.Combine(current, gitDependFile.Packages.Directory));

						foreach (var dependency in gitDependFile.Dependencies)
						{
							dependency.Directory = Path.GetFullPath(Path.Combine(current, dependency.Directory));
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
