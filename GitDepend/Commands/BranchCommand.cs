using System;
using System.Linq;
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
        private readonly IGit _git;
        private readonly IDependencyVisitorAlgorithm _algorithm;
        private readonly IConsole _console;

        /// <summary>
        /// Creates a enw <see cref="BranchCommand"/>
        /// </summary>
        /// <param name="options"></param>
        public BranchCommand(BranchSubOptions options)
        {
            _options = options;
            _git = DependencyInjection.Resolve<IGit>();
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
            bool[] flags =
            {
                _options.Delete,
                _options.ListMerged
            };

            // only one mutually exclusive flag can be set.
            if (flags.Count(b => b) > 1)
            {
                return ReturnCode.InvalidArguments;
            }

            // force delete can only be specified if delete is specified.
            if (!_options.Delete && _options.ForceDelete)
            {
                return ReturnCode.InvalidArguments;
            }

            // if delete is specified the branch name must also be specified.
            if (_options.Delete && string.IsNullOrEmpty(_options.BranchName))
            {
                return ReturnCode.InvalidArguments;
            }

            if (_options.ListMerged && !string.IsNullOrEmpty(_options.BranchName))
            {
                return ReturnCode.InvalidArguments;
            }

            IVisitor visitor;
            string successMessage;
            Func<ReturnCode> postTraverse = null;

            if (_options.Delete)
            {
                visitor = new DeleteBranchVisitor(_options.BranchName, _options.ForceDelete);
                successMessage = $"Successfully deleted the {_options.BranchName} branch from all repositories.";
            }
            else if (_options.ListMerged)
            {
                visitor = new ListMergedBranchesVisitor();
                successMessage = string.Empty;
                postTraverse = () =>
                {
                    _git.WorkingDirectory = _options.Directory;
                    return _git.ListMergedBranches();
                };
            }
            else if (!string.IsNullOrEmpty(_options.BranchName))
            {
                visitor = new CreateBranchVisitor(_options.BranchName);
                successMessage = $"Successfully switched to the {_options.BranchName} branch in all repositories.";
            }
            else
            {
                visitor = new ListAllBranchesVisitor();
                successMessage = string.Empty;
                postTraverse = () =>
                {
                    _git.WorkingDirectory = _options.Directory;
                    return _git.ListAllBranches();
                };
            }

            _algorithm.TraverseDependencies(visitor, _options.Directory);

            var code = visitor.ReturnCode;
            if (code == ReturnCode.Success && postTraverse != null)
            {
                var origColor = _console.ForegroundColor;
                _console.ForegroundColor = ConsoleColor.Green;
                _console.WriteLine("project:");
                _console.WriteLine($"    dir: {_options.Directory}");
                _console.WriteLine();
                _console.ForegroundColor = origColor;

                code = postTraverse();
            }

            if (code == ReturnCode.Success)
            {
                _console.WriteLine(successMessage);
            }


            return visitor.ReturnCode;
        }

        #endregion
    }
}
