using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Resources;
using GitDepend.Visitors;

namespace GitDepend.Commands
{
    /// <summary>
    /// An implementation of <see cref="NamedDependenciesCommand{T}"/> that syncs the referenced branch in GitDepend.json
    /// to the currently checked out branch in the dependency repository.
    /// </summary>
    public class SyncCommand : NamedDependenciesCommand<SyncSubOptions>
    {
        private readonly IGitDependFileFactory _factory;
        private readonly IGit _git;
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// The name of the verb.
        /// </summary>
        public const string Name = "sync";

        /// <summary>
        /// Creates a new <see cref="NamedDependenciesCommand{T}"/>
        /// </summary>
        /// <param name="options">The options for the command.</param>
        public SyncCommand(SyncSubOptions options) : base(options)
        {
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _git = DependencyInjection.Resolve<IGit>();
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
        }

        #region Overrides of NamedDependenciesCommand<SyncSubOptions>

        /// <summary>
        /// Creates the visitor that will be used to traverse the dependency graph.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected override NamedDependenciesVisitor CreateVisitor(SyncSubOptions options)
        {
            return new VerifyCorrectBranchVisitor(options.Dependencies);
        }

        #region Overrides of NamedDependenciesCommand<SyncSubOptions>

        /// <summary>
        /// Executes after all dependencies have been visited.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected override ReturnCode AfterDependencyTraversal(SyncSubOptions options)
        {
            string dir;
            ReturnCode code;
            var config = _factory.LoadFromDirectory(options.Directory, out dir, out code);

            if (code != ReturnCode.Success)
            {
                return code;
            }

            bool dirty = false;
            foreach (var dep in config.Dependencies)
            {
                var path = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(dir, dep.Directory));
                _git.WorkingDirectory = path;
                var branch = _git.GetCurrentBranch();

                if (dep.Branch != branch)
                {
                    dep.Branch = branch;
                    dirty = true;
                    Console.WriteLine(strings.USING_BRANCH_FOR_CONFIG, dep.Branch, dep.Configuration.Name);
                }
            }

            if (dirty)
            {
                _fileSystem.File.WriteAllText(_fileSystem.Path.Combine(dir, "GitDepend.json"), config.ToString());
            }

            Console.WriteLine(strings.SYNC_SUCCESS);

            return ReturnCode.Success;
        }

        #endregion

        #endregion
    }
}
