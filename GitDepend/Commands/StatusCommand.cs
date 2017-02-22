using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Visitors;

namespace GitDepend.Commands
{
    /// <summary>
    /// An implementation of <see cref="ICommand"/> that displays git status on all dependencies.
    /// </summary>
    public class StatusCommand : ICommand
    {
        private readonly StatusSubOptions _options;
        private readonly IDependencyVisitorAlgorithm _algorithm;
        private readonly IGit _git;
        private readonly IConsole _console;

        /// <summary>
        /// The name of the verb.
        /// </summary>
        public const string Name = "status";

        /// <summary>
        /// Creates a new <see cref="StatusCommand"/>
        /// </summary>
        /// <param name="options">The <see cref="StatusSubOptions"/> that configure this command.</param>
        public StatusCommand(StatusSubOptions options)
        {
            _options = options;
            _algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
            _git = DependencyInjection.Resolve<IGit>();
            _console = DependencyInjection.Resolve<IConsole>();
        }

        #region Implementation of ICommand

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>The return code.</returns>
        public ReturnCode Execute()
        {
            var visitor = new DisplayStatusVisitor(_options.Dependencies);
            _algorithm.TraverseDependencies(visitor, _options.Directory);

            var code = visitor.ReturnCode;
            if (code == ReturnCode.Success)
            {
                var origColor = _console.ForegroundColor;
                _console.ForegroundColor = ConsoleColor.Green;
                _console.WriteLine("project:");
                _console.WriteLine($"    dir: {_options.Directory}");
                _console.WriteLine();
                _console.ForegroundColor = origColor;

                _git.WorkingDirectory = _options.Directory;
                code = _git.Status();
            }

            return code;
        }

        #endregion
    }
}
