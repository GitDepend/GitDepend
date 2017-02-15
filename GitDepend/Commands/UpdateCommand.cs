using System;
using System.IO.Abstractions;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Visitors;

namespace GitDepend.Commands
{
	/// <summary>
	/// An implementation of <see cref="ICommand"/> that recursively updates all dependencies.
	/// </summary>
	public class UpdateCommand : ICommand
	{
		private readonly UpdateSubOptions _options;
		private readonly IGitDependFileFactory _factory;
		private readonly IGit _git;
		private readonly INuget _nuget;
		private readonly IProcessManager _processManager;
		private readonly IFileSystem _fileSystem;
		private readonly IConsole _console;

		/// <summary>
		/// The verb name
		/// </summary>
		public const string Name = "update";

		/// <summary>
		/// Creates a new <see cref="UpdateCommand"/>
		/// </summary>
		/// <param name="options">The <see cref="UpdateSubOptions"/> that configure the command.</param>
		/// <param name="factory">The <see cref="IGitDependFileFactory"/> to use.</param>
		/// <param name="git">The <see cref="IGit"/> to use.</param>
		/// <param name="nuget">The <see cref="INuget"/> to use.</param>
		/// <param name="processManager">The <see cref="IProcessManager"/> to use.</param>
		/// <param name="fileSystem">The <see cref="IFileSystem"/> to use.</param>
		/// <param name="console">The <see cref="IConsole"/> to use.</param>
		public UpdateCommand(UpdateSubOptions options, IGitDependFileFactory factory, IGit git, INuget nuget, IProcessManager processManager, IFileSystem fileSystem, IConsole console)
		{
			_options = options;
			_factory = factory;
			_git = git;
			_nuget = nuget;
			_processManager = processManager;
			_fileSystem = fileSystem;
			_console = console;
		}

		#region Implementation of ICommand

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <returns>The return code.</returns>
		public ReturnCode Execute()
		{
			var alg = new DependencyVisitorAlgorithm(_factory, _git, _fileSystem, _console);
			IVisitor visitor = new CheckOutBranchVisitor(_git, _fileSystem, _console);
			alg.TraverseDependencies(visitor, _options.Directory);

			if (visitor.ReturnCode != ReturnCode.Success)
			{
				_console.WriteLine("Could not ensure the correct branch on all dependencies.");
				return visitor.ReturnCode;
			}

			alg = new DependencyVisitorAlgorithm(_factory, _git, _fileSystem, _console);
			visitor = new BuildAndUpdateDependenciesVisitor(_factory, _git, _nuget, _processManager, _fileSystem, _console);
			alg.TraverseDependencies(visitor, _options.Directory);

			if (visitor.ReturnCode == ReturnCode.Success)
			{
				_console.WriteLine("Update complete!");
			}

			return visitor.ReturnCode;
		}

		#endregion
	}
}
