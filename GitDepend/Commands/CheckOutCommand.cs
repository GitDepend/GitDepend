using System.Collections.Generic;
using GitDepend.CommandLine;
using GitDepend.Visitors;

namespace GitDepend.Commands
{
    /// <summary>
    /// An implementation of <see cref="ICommand"/> that switches branches on all dependencies.
    /// </summary>
    public class CheckOutCommand : ICommand
    {
        /// <summary>
        /// The name of the verb.
        /// </summary>
        public const string Name = "checkout";

        private readonly CheckOutSubOptions _options;
        private readonly IDependencyVisitorAlgorithm _algorithm;

        /// <summary>
        /// Creates a new <see cref="CheckOutCommand"/>
        /// </summary>
        /// <param name="options">The <see cref="CheckOutSubOptions"/> that controls the behavior of this command.</param>
        public CheckOutCommand(CheckOutSubOptions options)
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
            if (string.IsNullOrEmpty(_options.BranchName))
            {
                return ReturnCode.InvalidArguments;
            }

            IVisitor visitor = new CheckOutBranchVisitor(_options.BranchName, _options.CreateBranch);
            _algorithm.TraverseDependencies(visitor, _options.Directory);

            if (visitor.ReturnCode != ReturnCode.Success)
            {
                return visitor.ReturnCode;
            }

            _algorithm.Reset();

            visitor = new VerifyCorrectBranchVisitor(new List<string>());
            _algorithm.TraverseDependencies(visitor, _options.Directory);

            return visitor.ReturnCode;
        }

        #endregion
    }
}