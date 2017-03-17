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
        /// <returns>An implementation of <see cref="ICommand"/> that matches the given arguments.</returns>
        public ICommand GetCommand(string[] args)
        {
            string invokedVerb = null;
            object invokedVerbInstance = null;

            if (!global::CommandLine.Parser.Default.ParseArgumentsStrict(args, Options.Default,
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
                var fileSystem = DependencyInjection.Resolve<IFileSystem>();

                options.Directory = string.IsNullOrEmpty(options.Directory)
                    ? Environment.CurrentDirectory
                    : fileSystem.Path.GetFullPath(options.Directory);
            }

            ICommand command = null;

            switch (invokedVerb)
            {
                case AddCommand.Name:
                    command = new AddCommand(options as AddSubOptions);
                    break;
                case BranchCommand.Name:
                    command = new BranchCommand(options as BranchSubOptions);
                    break;
                case CheckOutCommand.Name:
                    command = new CheckOutCommand(options as CheckOutSubOptions);
                    break;
                case CloneCommand.Name:
                    command = new CloneCommand(options as CloneSubOptions);
                    break;
                case ConfigCommand.Name:
                    command = new ConfigCommand(options as ConfigSubOptions);
                    break;
                case InitCommand.Name:
                    command = new InitCommand(options as InitSubOptions);
                    break;
                case ListCommand.Name:
                    command = new ListCommand(options as ListSubOptons);
                    break;
                case RemoveCommand.Name:
                    command = new RemoveCommand(options as RemoveSubOptions);
                    break;
                case StatusCommand.Name:
                    command = new StatusCommand(options as StatusSubOptions);
                    break;
                case SyncCommand.Name:
                    command = new SyncCommand(options as SyncSubOptions);
                    break;
                case UpdateCommand.Name:
                    command = new UpdateCommand(options as UpdateSubOptions);
                    break;
            }

            return command;
        }
    }
}
