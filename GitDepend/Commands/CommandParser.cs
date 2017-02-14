using System;
using System.IO.Abstractions;
using GitDepend.Busi;
using GitDepend.CommandLine;

namespace GitDepend.Commands
{
	/// <summary>
	/// Parses command line arguments and returns the appropriate implementation of <see cref="ICommand"/> for execution.
	/// </summary>
	public class CommandParser
	{
		/// <summary>
		/// Gets the implementation of <see cref="ICommand"/> that corresponds with the given arguments.
		/// </summary>
		/// <param name="args">The command line arguments.</param>
		/// <param name="factory">The <see cref="IGitDependFileFactory"/> to use.</param>
		/// <param name="git">The <see cref="IGit"/> to use.</param>
		/// <param name="nuget">The <see cref="INuget"/> to use.</param>
		/// <param name="processManager">The <see cref="IProcessManager"/> to use.</param>
		/// <param name="fileSystem">The <see cref="IFileSystem"/> to use.</param>
		/// <returns>An implementation of <see cref="ICommand"/> that matches the given arguments.</returns>
		public ICommand GetCommand(string[] args, IGitDependFileFactory factory, IGit git, INuget nuget, IProcessManager processManager, IFileSystem fileSystem)
		{
			string invokedVerb = null;
			object invokedVerbInstance = null;

			if (!global::CommandLine.Parser.Default.ParseArguments(args, Options.Default,
				(verb, verbOptions) =>
				{
					invokedVerb = verb;
					invokedVerbInstance = verbOptions;
				}))
			{
				Environment.Exit(global::CommandLine.Parser.DefaultExitCodeFail);
			}

			var options = invokedVerbInstance as CommonSubOptions;

			if (options != null)
			{
				options.Directory = string.IsNullOrEmpty(options.Directory)
					? Environment.CurrentDirectory
					: fileSystem.Path.GetFullPath(options.Directory);
			}

			ICommand command = null;

			switch (invokedVerb)
			{
				case InitCommand.Name:
					command = new InitCommand(options as InitSubOptions, fileSystem);
					break;
				case ShowConfigCommand.Name:
					command = new ShowConfigCommand(options as ConfigSubOptions, fileSystem);
					break;
				case CloneCommand.Name:
					command = new CloneCommand(options as CloneSubOptions, factory, git, fileSystem);
					break;
				case UpdateCommand.Name:
					command = new UpdateCommand(options as UpdateSubOptions, factory, git, nuget, processManager, fileSystem);
					break;
			}

			return command;
		}
	}
}
