using System;
using System.IO.Abstractions;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Resources;
using GitDepend.Visitors;

namespace GitDepend.Commands
{
	/// <summary>
	/// An implementation of <see cref="ICommand"/> that creates a branch on the current project and
	/// all dependencies.
	/// </summary>
	public class BuildCommand : ICommand
	{
		/// <summary>
		/// The name of the verb.
		/// </summary>
		public const string Name = "build";

		private readonly BuildSubOptions _options;
		private readonly IGit _git;
		private readonly IDependencyVisitorAlgorithm _algorithm;
		private readonly IConsole _console;
		private readonly IFileSystem _fileSystem;

		/// <summary>
		/// Creates a enw <see cref="BuildCommand"/>
		/// </summary>
		/// <param name="options">The <see cref="BuildSubOptions"/> that controls the behavior of this command.</param>
		public BuildCommand(BuildSubOptions options)
		{
			_options = options;
			_git = DependencyInjection.Resolve<IGit>();
			_algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
			_console = DependencyInjection.Resolve<IConsole>();
			_fileSystem = DependencyInjection.Resolve<IFileSystem>();
		}

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <returns>The return code.</returns>
		public ReturnCode Execute()
		{
			// "dep build <dependency> --all" is not allowed because it is ambiguous.
			if (_options.All && !string.IsNullOrEmpty(_options.DependencyName))
			{
				return ReturnCode.InvalidArguments;
			}

			IVisitor visitor;

			if (_options.All)
			{
				// build all dependencies
				visitor = new BuildVisitor(string.Empty);
			}
			else if (!string.IsNullOrEmpty(_options.DependencyName))
			{
				// build the named dependency
				visitor = new BuildVisitor(_options.DependencyName);
			}
			else
			{
				// build only current project
				visitor = new BuildVisitor(_fileSystem.Path.GetFullPath(_options.Directory));
			}

			_algorithm.TraverseDependencies(visitor, _options.Directory);
			
			return visitor.ReturnCode;
		}
	}
}