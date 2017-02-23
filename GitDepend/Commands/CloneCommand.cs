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
        private readonly IDependencyVisitorAlgorithm _algorithm;
        private readonly IConsole _console;

        /// <summary>
        /// Creates a new <see cref="CloneCommand"/>
        /// </summary>
        /// <param name="options">The <see cref="CloneSubOptions"/> that configure this command.</param>
        public CloneCommand(CloneSubOptions options)
        {
            _options = options;
            _algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
            _console = DependencyInjection.Resolve<IConsole>();
        }

        #region Implementation of ICommand

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>The return code.</returns>
        public ReturnCode Execute()
        {
            var visitor = new CheckOutBranchVisitor();
            _algorithm.TraverseDependencies(visitor, _options.Directory);

            if (visitor.ReturnCode == ReturnCode.Success)
            {
                _console.WriteLine("Successfully cloned all dependencies");
            }

            return visitor.ReturnCode;
        }

        #endregion
    }
}
