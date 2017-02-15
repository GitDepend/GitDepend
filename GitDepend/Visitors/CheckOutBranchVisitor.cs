using System;
using System.IO.Abstractions;
using GitDepend.Busi;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
	/// <summary>
	/// An implementation of <see cref="IVisitor"/> that ensures that the correct branch
	/// has been checked out on each dependency.
	/// </summary>
	public class CheckOutBranchVisitor : IVisitor
	{
		private readonly IGit _git;
		private readonly IFileSystem _fileSystem;
		private readonly IConsole _console;

		/// <summary>
		/// Creates a new <see cref="CheckOutBranchVisitor"/>
		/// </summary>
		/// <param name="git">The <see cref="IGit"/> to use.</param>
		/// <param name="fileSystem">The <see cref="IFileSystem"/> to use.</param>
		/// <param name="console">The <see cref="IConsole"/> to use.</param>
		public CheckOutBranchVisitor(IGit git, IFileSystem fileSystem, IConsole console)
		{
			_git = git;
			_fileSystem = fileSystem;
			_console = console;
		}

		#region Implementation of IVisitor

		/// <summary>
		/// The return code
		/// </summary>
		public ReturnCode ReturnCode { get; set; }

		/// <summary>
		/// Visits a project dependency.
		/// </summary>
		/// <param name="directory">The directory of the project.</param>
		/// <param name="dependency">The <see cref="Dependency"/> to visit.</param>
		/// <returns>The return code.</returns>
		public ReturnCode VisitDependency(string directory, Dependency dependency)
		{
			_console.WriteLine($"Checking out the {dependency.Branch} branch on {dependency.Name}");
			
			_git.WorkingDirectory = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(directory, dependency.Directory));
			return ReturnCode = _git.Checkout(dependency.Branch);
		}

		/// <summary>
		/// Visists a project.
		/// </summary>
		/// <param name="directory">The directory of the project.</param>
		/// <param name="config">The <see cref="GitDependFile"/> with project configuration information.</param>
		/// <returns>The return code.</returns>
		public ReturnCode VisitProject(string directory, GitDependFile config)
		{
			return ReturnCode.Success;
		}

		#endregion
	}
}