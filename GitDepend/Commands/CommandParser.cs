using System;
using System.IO;
using GitDepend.CommandLine;

namespace GitDepend.Commands
{
	public class CommandParser
	{
		public ICommand GetCommand(string[] args)
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
					: Path.GetFullPath(options.Directory);
			}

			ICommand command = null;

			switch (invokedVerb)
			{
				case InitCommand.Name:
					command = new InitCommand(options as InitSubOptions);
					break;
				case ShowConfigCommand.Name:
					command = new ShowConfigCommand(options as ConfigSubOptions);
					break;
				case CloneCommand.Name:
					command = new CloneCommand(options as CloneSubOptions);
					break;
				case UpdateCommand.Name:
					command = new UpdateCommand(options as UpdateSubOptions);
					break;
			}

			return command;
		}
	}
}
