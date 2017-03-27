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
    /// Performs a git push on all of the dependencies.
    /// </summary>
    public class PushCommand : NamedDependenciesCommand<PushSubOptions>
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        public const string Name = "push";

        /// <summary>
        /// The constructor for the push command.
        /// </summary>
        /// <param name="options">The options for the push command.</param>
        public PushCommand(PushSubOptions options) : base(options)
        {
        }

        /// <summary>
        /// Creates the visitor that will be used to traverse the dependency graph.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected override NamedDependenciesVisitor CreateVisitor(PushSubOptions options)
        {
            return new PushBranchVisitor(options.Dependencies, options.PushArguments);
        }
    }
}
