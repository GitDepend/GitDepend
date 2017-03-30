using System;
using System.IO.Abstractions;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Resources;
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
            IVisitor visitor = new NullVisitor();
            _algorithm.TraverseDependencies(visitor, _options.Directory);

            if (visitor.ReturnCode == ReturnCode.Success)
            {
                _console.WriteLine(strings.CLONED_ALL_DEPS);
            }

            visitor = new CheckOutDependencyBranchVisitor();
            _algorithm.Reset();
            _algorithm.TraverseDependencies(visitor, _options.Directory);

            if (visitor.ReturnCode == ReturnCode.Success)
            {
                _console.WriteLine(strings.DEPS_CORRECT_BRANCH);
            }

            return visitor.ReturnCode;
        }

        #endregion
    }
}
