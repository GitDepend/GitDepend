using System;
using System.IO.Abstractions;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Visitors;

namespace GitDepend.Commands
{
	/// <summary>
	/// An implementation of <see cref="ICommand"/> that ensures that all dependencies have been cloned
	/// </summary>
	public class CloneCommand : ICommand
	{
		/// <summary>
		/// The name of the verb.
		/// </summary>
		public const string Name = "clone";
		private readonly CloneSubOptions _options;
		private readonly IGitDependFileFactory _factory;
		private readonly IGit _git;
		private readonly IFileSystem _fileSystem;
		private readonly IConsole _console;

		/// <summary>
		/// Creates a new <see cref="CloneCommand"/>
		/// </summary>
		/// <param name="options">The <see cref="CloneSubOptions"/> that configure this command.</param>
		/// <param name="factory">The <see cref="IGitDependFileFactory"/> to use.</param>
		/// <param name="git">The <see cref="IGit"/> to use.</param>
		/// <param name="fileSystem">The <see cref="IFileSystem"/> to use.</param>
		/// <param name="console">The <see cref="IConsole"/> to use.</param>
		public CloneCommand(CloneSubOptions options, IGitDependFileFactory factory, IGit git, IFileSystem fileSystem, IConsole console)
		{
			_options = options;
			_factory = factory;
			_git = git;
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
			var visitor = new CheckOutBranchVisitor(_git, _fileSystem, _console);
			alg.TraverseDependencies(visitor, _options.Directory);

			if (visitor.ReturnCode == ReturnCode.Success)
			{
				_console.WriteLine("Successfully cloned all dependencies");
			}

			return visitor.ReturnCode;
		}

		#endregion
	}
}
