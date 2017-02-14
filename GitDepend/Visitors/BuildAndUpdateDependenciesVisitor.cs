using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using GitDepend.Busi;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
	/// <summary>
	/// Implementation of <see cref="IVisitor"/> that runs the build for dependencies, and updates the project
	/// to consume the latest dependency artifacts.
	/// </summary>
	public class BuildAndUpdateDependenciesVisitor : IVisitor
	{
		private static readonly Regex Pattern = new Regex(@"^(?<id>.*?)\.(?<version>(?:\d\.){2,3}\d(?:-.*?)?)$", RegexOptions.Compiled);
		private readonly IGitDependFileFactory _factory;
		private readonly IGit _git;
		private readonly INuget _nuget;
		private readonly IFileSystem _fileSystem;

		/// <summary>
		/// Creates a new <see cref="BuildAndUpdateDependenciesVisitor"/>
		/// </summary>
		/// /// <param name="factory">The <see cref="IGitDependFileFactory"/> to use.</param>
		/// <param name="git">The <see cref="IGit"/> to use.</param>
		/// <param name="nuget">The <see cref="INuget"/> to use.</param>
		/// <param name="fileSystem">The <see cref="IFileSystem"/> to use.</param>
		public BuildAndUpdateDependenciesVisitor(IGitDependFileFactory factory, IGit git, INuget nuget, IFileSystem fileSystem)
		{
			_factory = factory;
			_git = git;
			_nuget = nuget;
			_fileSystem = fileSystem;
		}

		private string CacheDirectory
		{
			get
			{
				var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GitDepend");

				var cacheDir = Path.Combine(dir, "cache");
				if (_fileSystem.Directory.Exists(cacheDir))
				{
					return cacheDir;
				}

				try
				{
					_fileSystem.Directory.CreateDirectory(cacheDir);
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine(ex.Message);
					return null;
				}

				return cacheDir;
			}
		}

		#region Implementation of IVisitor

		/// <summary>
		/// The return code
		/// </summary>
		public ReturnCode ReturnCode { get; set; }

		/// <summary>
		/// Visits a project dependency.
		/// </summary>
		/// <param name="dependency">The <see cref="Dependency"/> to visit.</param>
		/// <returns>The return code.</returns>
		public ReturnCode VisitDependency(Dependency dependency)
		{
			string dir;
			string error;
			var config = _factory.LoadFromDirectory(dependency.Directory, out dir, out error);

			if (config == null)
			{
				return ReturnCode.GitRepositoryNotFound;
			}

			var info = new ProcessStartInfo(Path.Combine(dependency.Directory, config.Build.Script), config.Build.Arguments)
			{
				WorkingDirectory = dependency.Directory,
				UseShellExecute = false
			};
			var proc = Process.Start(info);
			proc?.WaitForExit();

			
			var artifactsDir = dependency.Configuration.Packages.Directory;
			foreach (var file in _fileSystem.Directory.GetFiles(artifactsDir, "*.nupkg"))
			{
				var name = Path.GetFileName(file);
				if (!string.IsNullOrEmpty(name))
				{
					_fileSystem.File.Copy(file, Path.Combine(CacheDirectory, name), true);
				}
			}

			var code = proc?.ExitCode ?? (int) ReturnCode.FailedToRunBuildScript;
			return (ReturnCode)code;
		}

		/// <summary>
		/// Visists a project.
		/// </summary>
		/// <param name="directory">The directory of the project.</param>
		/// <param name="config">The <see cref="GitDependFile"/> with project configuration information.</param>
		/// <returns>The return code.</returns>
		public ReturnCode VisitProject(string directory, GitDependFile config)
		{
			foreach (var dependency in config.Dependencies)
			{
				var dir = dependency.Configuration.Packages.Directory;
				foreach (var file in _fileSystem.Directory.GetFiles(dir, "*.nupkg"))
				{
					var name = Path.GetFileNameWithoutExtension(file);


					if (string.IsNullOrEmpty(name))
					{
						continue;
					}

					var match = Pattern.Match(name);

					if (!match.Success)
					{
						continue;
					}

					var id = match.Groups["id"].Value;
					var version = match.Groups["version"].Value;

					var nugetConfig = Path.Combine(directory, "NuGet.config");
					if (!_fileSystem.File.Exists(nugetConfig))
					{
						nugetConfig = null;
					}

					var nuget = new Nuget { ConfigFile = nugetConfig };

					foreach (var solution in _fileSystem.Directory.GetFiles(directory, "*.sln", SearchOption.AllDirectories))
					{
						nuget.Update(solution, id, version, CacheDirectory);
					}
				}
			}

			Console.WriteLine("================================================================================");
			Console.WriteLine($"Making update commit on {directory}");
			_git.WorkingDir = directory;
			_git.Add("*.csproj", @"*\packages.config");
			Console.WriteLine("================================================================================");
			_git.Status();
			_git.Commit("GitDepend: updating dependencies");

			return ReturnCode.Success;
		}

		#endregion
	}
}
