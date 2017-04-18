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
        private string _gitArguments;

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
            _gitArguments = options.GitArguments;
        }

        /// <summary>
        /// Creates the visitor that will be used to traverse the dependency graph.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected override NamedDependenciesVisitor CreateVisitor(CleanSubOptions options)
        {
            return new CleanDependencyVisitor(Options.GitArguments, Options.Dependencies);
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
            _git.WorkingDirectory = options.Directory;
            if (options.Dependencies == null || options.Dependencies.Count == 0)
            {
                return GitClean(options);
            }
            if (options.Dependencies != null && config != null && options.Dependencies.Contains(config.Name) && code == ReturnCode.Success)
            {
                return GitClean(options);
            }
            return code;
        }

        private ReturnCode GitClean(CleanSubOptions options)
        {
            return _git.Clean(_gitArguments);
        }
    }
}
