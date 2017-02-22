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
            var fileSystem = DependencyInjection.Resolve<IFileSystem>();
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
