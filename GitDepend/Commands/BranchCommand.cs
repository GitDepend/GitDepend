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
    /// An implementation of <see cref="ICommand"/> that creates a branch on the current project and
    /// all dependencies.
    /// </summary>
    public class BranchCommand : ICommand
    {
        /// <summary>
        /// The name of the verb.
        /// </summary>
        public const string Name = "branch";

        private readonly BranchSubOptions _options;
        private readonly IDependencyVisitorAlgorithm _algorithm;
        private readonly IConsole _console;

        /// <summary>
        /// Creates a enw <see cref="BranchCommand"/>
        /// </summary>
        /// <param name="options"></param>
        public BranchCommand(BranchSubOptions options)
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
            // TODO: Traverse dependencies and create a branch
            //       also create the branch on this project too.

            IVisitor visitor = new CreateBranchVisitor(_options.BranchName);
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
