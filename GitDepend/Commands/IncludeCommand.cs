using System;
using System.IO.Abstractions;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Configuration;
using GitDepend.Resources;
using GitDepend.Visitors;

namespace GitDepend.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class IncludeCommand : ICommand
    {
        private readonly IncludeSubOptions _options;
        private readonly IFileSystem _fileSystem;
        private readonly IGitDependFileFactory _factory;
        private readonly IDependencyVisitorAlgorithm _algorithm;
        private readonly IConsole _console;

        /// <summary>
        /// The name of the verb.
        /// </summary>
        public const string Name = "include";

        /// <summary>
        /// Creates a new <see cref="IncludeCommand"/>
        /// </summary>
        /// <param name="options">The <see cref="IncludeSubOptions"/> that configures the command.</param>
        public IncludeCommand(IncludeSubOptions options)
        {
            _options = options;
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _console = DependencyInjection.Resolve<IConsole>();
            _algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
        }

        #region Implementation of ICommand

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>The return code.</returns>
        public ReturnCode Execute()
        {
            IVisitor visitor = new IncludeVisitor(_options.DepNames, _options.Clear);
            _algorithm.TraverseDependencies(visitor, _options.Directory, true);

            if (visitor.ReturnCode == ReturnCode.Success)
            {
                _console.WriteLine(strings.FILES_UPDATED);
            }

            return visitor.ReturnCode;
        }

        #endregion
    }
}
