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
    /// This command lets you manage the dependencies and their configuration files.
    /// </summary>
    public class ManageCommand : NamedDependenciesCommand<ManageSubOptions>
    {
        private ManageDependenciesVisitor _visitor;
        /// <summary>
        /// The name of the verb.
        /// </summary>
        public const string Name = "manage";

        private readonly IGitDependFileFactory _factory;
        private readonly IFileSystem _fileSystem;
        private readonly IConsole _console;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ManageCommand"/> class.
        /// </summary>
        /// <param name="options">The <see cref="ManageSubOptions"/> that configure manage</param>
        public ManageCommand(ManageSubOptions options) : base(options)
        {
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
            _console = DependencyInjection.Resolve<IConsole>();
        }

        /// <summary>
        /// Creates the visitor that will be used to traverse the dependency graph.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected override NamedDependenciesVisitor CreateVisitor(ManageSubOptions options)
        {
            _visitor = new ManageDependenciesVisitor(options);
            return _visitor;
        }

        /// <summary>
        /// Executes after all dependencies have been visited.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected override ReturnCode AfterDependencyTraversal(ManageSubOptions options)
        {
            string dir;
            ReturnCode returnCode;
            var config = _factory.LoadFromDirectory(options.Directory, out dir, out returnCode);
            bool updated = false;
            foreach (var dependency in config.Dependencies)
            {
                var path =
                    _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(options.Directory, dependency.Directory));
                if (string.Equals(path, _visitor.NameMatchingDirectory))
                {
                    if (!string.IsNullOrEmpty(options.SetBranch))
                    {
                        dependency.Branch = options.SetBranch;
                        updated = true;
                    }
                    if (!string.IsNullOrEmpty(options.SetUrl))
                    {
                        dependency.Url = options.SetUrl;
                        updated = true;
                    }
                    if (!string.IsNullOrEmpty(options.SetDirectory))
                    {
                        dependency.Directory = options.SetDirectory;
                        updated = true;
                    }
                }
            }

            _fileSystem.File.WriteAllText(_fileSystem.Path.Combine(Options.Directory, "GitDepend.json"), config.ToString());
            _console.WriteLine(strings.CONFIG_UPDATED);
            return !updated ? ReturnCode.NameDidNotMatchRequestedDependency : ReturnCode.Success;
        }
    }
}
