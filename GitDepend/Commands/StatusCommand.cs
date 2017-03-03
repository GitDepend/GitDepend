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
    /// An implementation of <see cref="NamedDependenciesCommand{T}"/> that displays git status on all dependencies.
    /// </summary>
    public class StatusCommand : NamedDependenciesCommand<StatusSubOptions>
    {
        /// <summary>
        /// The name of the verb.
        /// </summary>
        public const string Name = "status";

        private readonly StatusSubOptions _options;
        private readonly IGit _git;

        /// <summary>
        /// Creates a new <see cref="StatusCommand"/>
        /// </summary>
        /// <param name="options">The <see cref="StatusSubOptions"/> that configure this command.</param>
        public StatusCommand(StatusSubOptions options) : base(options)
        {
            _options = options;
            _git = DependencyInjection.Resolve<IGit>();
        }

        #region Overrides of NamedDependenciesCommand<StatusSubOptions>

        /// <summary>
        /// Creates the visitor that will be used to traverse the dependency graph.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected override NamedDependenciesVisitor CreateVisitor(StatusSubOptions options)
        {
            return new DisplayStatusVisitor(options.Dependencies);
        }

        #region Overrides of NamedDependenciesCommand<StatusSubOptions>

        /// <summary>
        /// Executes after all dependencies have been visited.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected override ReturnCode AfterDependencyTraversal(StatusSubOptions options)
        {
            _git.WorkingDirectory = _options.Directory;
            return _git.Status();
        }

        #endregion

        #endregion
    }
}
