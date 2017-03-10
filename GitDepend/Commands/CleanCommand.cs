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
    /// This command will go through the dependency tree and clean each repository, or only the one named.
    /// </summary>
    /// <seealso cref="GitDepend.Commands.ICommand" />
    public class CleanCommand : ICommand
    {
        private CleanSubOptions _options;
        private readonly IDependencyVisitorAlgorithm _algorithm;

        /// <summary>
        /// The name
        /// </summary>
        public const string Name = "clean";

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanCommand" /> class.
        /// </summary>
        public CleanCommand(CleanSubOptions options)
        {
            _options = options;
            _algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>
        /// The return code.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public ReturnCode Execute()
        {
            var visitor = new CleanDependencyVisitor(_options.DryRun, _options.Force, _options.RemoveUntrackedFiles, _options.RemoveUntrackedDirectories);
            
            _algorithm.TraverseDependencies(visitor, _options.Directory);

            return visitor.ReturnCode;
        }
    }
}
