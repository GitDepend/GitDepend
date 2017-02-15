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
		public UpdateCommand(UpdateSubOptions options)
		{
			_options = options;
			_factory = DependencyInjection.Resolve<IGitDependFileFactory>();
			_git = DependencyInjection.Resolve<IGit>();
			_nuget = DependencyInjection.Resolve<INuget>();
			_processManager = DependencyInjection.Resolve<IProcessManager>();
			_fileSystem = DependencyInjection.Resolve<IFileSystem>();
			_console = DependencyInjection.Resolve<IConsole>();
		}

		#region Implementation of ICommand

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <returns>The return code.</returns>
		public ReturnCode Execute()
		{
			var alg = new DependencyVisitorAlgorithm();
			IVisitor visitor = new CheckOutBranchVisitor();
			alg.TraverseDependencies(visitor, _options.Directory);

			if (visitor.ReturnCode != ReturnCode.Success)
			{
				_console.WriteLine("Could not ensure the correct branch on all dependencies.");
				return visitor.ReturnCode;
			}

			alg = new DependencyVisitorAlgorithm();
			visitor = new BuildAndUpdateDependenciesVisitor();
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
