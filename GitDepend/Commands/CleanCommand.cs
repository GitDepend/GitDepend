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
    /// This command will go through the dependency tree and clean each repository, or only the one named.
    /// </summary>
    /// <seealso cref="GitDepend.Commands.ICommand" />
    public class CleanCommand : NamedDependenciesCommand<CleanSubOptions>
    {
        private readonly IGit _git;
        private readonly IGitDependFileFactory _factory;

        /// <summary>
        /// The name
        /// </summary>
        public const string Name = "clean";

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanCommand" /> class.
        /// </summary>
        public CleanCommand(CleanSubOptions options) : base(options)
        {
            _git = DependencyInjection.Resolve<IGit>();
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
        }

        /// <summary>
        /// Creates the visitor that will be used to traverse the dependency graph.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected override NamedDependenciesVisitor CreateVisitor(CleanSubOptions options)
        {
            return new CleanDependencyVisitor(Options.DryRun, Options.Force, Options.RemoveUntrackedFiles, Options.RemoveUntrackedDirectories, Options.Dependencies);
        }


        /// <summary>
        /// Executes after all dependencies have been visited.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected override ReturnCode AfterDependencyTraversal(CleanSubOptions options)
        {
            string dir;
            ReturnCode code;
            var config = _factory.LoadFromDirectory(options.Directory, out dir, out code);
            if (options.Dependencies.Contains(config.Name) && code == ReturnCode.Success)
            {
                _git.WorkingDirectory = options.Directory;
                return _git.Clean(options.DryRun, options.Force, options.RemoveUntrackedFiles, options.RemoveUntrackedDirectories);
            }
            return code;
        }
    }
}
