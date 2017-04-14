using System;
using GitDepend.CommandLine;
using GitDepend.Resources;
using GitDepend.Visitors;

namespace GitDepend.Commands
{
    /// <summary>
    /// This command will do a git log on the named dependencies.
    /// </summary>
    public class LogCommand : ICommand
    {
        /// <summary>
        /// Name of the command
        /// </summary>
        public const string Name = "log";

        private readonly IDependencyVisitorAlgorithm _algorithm;
        private LogSubOptions _options;

        /// <summary>
        /// The constructor for the LogCommand.
        /// </summary>
        /// <param name="options"></param>
        public LogCommand(LogSubOptions options)
        {
            _options = options;
            _algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
        }

        /// <summary>
        /// Executes the log command
        /// </summary>
        /// <returns></returns>
        public ReturnCode Execute()
        {
            var visitor = new LogVisitor(_options.GitArguments, _options.Dependencies);
            _algorithm.TraverseDependencies(visitor, _options.Directory);

            var code = visitor.ReturnCode;
            if (code == ReturnCode.Success)
            {
                var origColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(strings.PROJECT);
                Console.WriteLine(strings.DIRECTORY + _options.Directory);
                Console.WriteLine();
                Console.ForegroundColor = origColor;
            }

            return code;
        }
    }
}
