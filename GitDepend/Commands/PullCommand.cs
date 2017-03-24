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
    /// This command will do a git pull on the named dependencies.
    /// </summary>
    public class PullCommand : NamedDependenciesCommand<PullSubOptions>
    {
        /// <summary>
        /// Name of the command
        /// </summary>
        public const string Name = "pull";

        /// <summary>
        /// The constructor for the PullCommand.
        /// </summary>
        /// <param name="options"></param>
        public PullCommand(PullSubOptions options) : base(options)
        {
        }

        /// <summary>
        /// Creates the visitor that will be used to traverse the dependency graph.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected override NamedDependenciesVisitor CreateVisitor(PullSubOptions options)
        {
            return new PullBranchVisitor(options.PullArguments, options.Dependencies);
        }
    }
}
