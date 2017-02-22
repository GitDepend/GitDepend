using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            return visitor.ReturnCode;
        }

        #endregion
    }
}
