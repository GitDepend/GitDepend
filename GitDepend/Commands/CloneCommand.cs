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
        private readonly IGitDependFileFactory _factory;
        private readonly IGit _git;
        private readonly IFileSystem _fileSystem;
        private readonly IConsole _console;

        /// <summary>
        /// Creates a new <see cref="CloneCommand"/>
        /// </summary>
        /// <param name="options">The <see cref="CloneSubOptions"/> that configure this command.</param>
        public CloneCommand(CloneSubOptions options)
        {
            _options = options;
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _git = DependencyInjection.Resolve<IGit>();
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
            _console = DependencyInjection.Resolve<IConsole>();
        }

        #region Implementation of ICommand

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>The return code.</returns>
        public ReturnCode Execute()
        {
            var alg = new DependencyVisitorAlgorithm();
            var visitor = new CheckOutBranchVisitor();
            alg.TraverseDependencies(visitor, _options.Directory);

            if (visitor.ReturnCode == ReturnCode.Success)
            {
                _console.WriteLine("Successfully cloned all dependencies");
            }

            return visitor.ReturnCode;
        }

        #endregion
    }
}
