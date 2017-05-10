using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.Configuration;
using GitDepend.Resources;

namespace GitDepend.Visitors
{
	/// <summary>
	/// Implementation of <see cref="IVisitor"/> that runs the build for dependencies.
	/// </summary>
	public class BuildVisitor : IVisitor
	{
		private readonly string _projectToBuild;

		private readonly IProcessManager _processManager;
		private readonly IFileSystem _fileSystem;

		/// <summary>
		/// Builds the visitor
		/// </summary>
		/// <param name="projectToBuild">Dependencies we should visit.</param>
		public BuildVisitor(string projectToBuild)
		{
			_projectToBuild = projectToBuild;
			_processManager = DependencyInjection.Resolve<IProcessManager>();
			_fileSystem = DependencyInjection.Resolve<IFileSystem>();
		}

		#region Implementation of IVisitor

		/// <summary>
		/// The return code.
		/// </summary>
		public ReturnCode ReturnCode { get; set; }

		/// <summary>
		/// Visits a project dependency.  This does nothing for this verb.
		/// </summary>
		/// <param name="directory">The directory of the project.</param>
		/// <param name="dependency">The <see cref="Dependency"/> to visit.</param>
		/// <returns>The return code.</returns>
		public ReturnCode VisitDependency(string directory, Dependency dependency)
		{
			return ReturnCode.Success;
		}

		/// <summary>
		/// Visits and builds a project, only if it is part of the whitelist.
		/// </summary>
		/// <param name="directory">The directory of the project.</param>
		/// <param name="config">The <see cref="GitDependFile"/> with project configuration information.</param>
		/// <returns>The return code.</returns>
		public ReturnCode VisitProject(string directory, GitDependFile config)
		{
			if (config == null)
			{
				return ReturnCode = ReturnCode.Success;
			}
			
			var shouldExecute = string.IsNullOrEmpty(_projectToBuild) ||
			                    string.Equals(_projectToBuild, config.Name, StringComparison.CurrentCultureIgnoreCase);

			if (!shouldExecute)
			{
				return ReturnCode.Success;
			}

			if (string.IsNullOrEmpty(directory) || !_fileSystem.Directory.Exists(directory))
			{
				return ReturnCode = ReturnCode.DirectoryDoesNotExist;
			}

			int exitCode;

			var buildScript = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(directory, config.Build.Script));
			if (!_fileSystem.File.Exists(buildScript))
			{
				return ReturnCode.BuildScriptNotFound;
			}

			var info = new ProcessStartInfo(buildScript, config.Build.Arguments)
			{
				WorkingDirectory = directory,
				UseShellExecute = false
			};

			using (var proc = _processManager.Start(info))
			{
				proc.WaitForExit();
				exitCode = proc.ExitCode;
			}
			
			return ReturnCode = exitCode == 0
				? ReturnCode.Success
				: ReturnCode.FailedToRunBuildScript;
		}

		#endregion
	}
}
