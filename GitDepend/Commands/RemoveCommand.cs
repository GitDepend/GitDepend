using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Resources;
using GitDepend.Visitors;

namespace GitDepend.Commands
{
    /// <summary>
    /// This removes a dependency
    /// </summary>
    /// <seealso cref="GitDepend.Commands.ICommand" />
    public class RemoveCommand : NamedDependenciesCommand<RemoveSubOptions>
    {
        /// <summary>
        /// The name
        /// </summary>
        public const string Name = "remove";

        private RemoveDependencyVisitor _visitor;
        
        private readonly IFileSystem _fileSystem;
        private IGitDependFileFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommand"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public RemoveCommand(RemoveSubOptions options) : base(options)
        {
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
        }

        /// <summary>
        /// Executes after all dependencies have been visited.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected override ReturnCode AfterDependencyTraversal(RemoveSubOptions options)
        {
            string dir;
            ReturnCode code;
            var config = _factory.LoadFromDirectory(options.Directory, out dir, out code);
            
            //visit the project and load the config and delete the configuration.
            int indexToRemove = -1;
            int index = 0;
            foreach (var dep in config.Dependencies)
            {
                var directoryName = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(options.Directory, dep.Directory));
                if (directoryName == _visitor.FoundDependencyDirectory)
                {
                    indexToRemove = index;
                    break;
                }
            }
            if(indexToRemove > -1)
            { 
                config.Dependencies.RemoveAt(indexToRemove);
                _fileSystem.File.WriteAllText(_fileSystem.Path.Combine(options.Directory, "GitDepend.json"), config.ToString());
                return ReturnCode.Success;
            }
            return ReturnCode.NameDidNotMatchRequestedDependency;
        }

        /// <summary>
        /// Creates the visitor that will be used to traverse the dependency graph.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected override NamedDependenciesVisitor CreateVisitor(RemoveSubOptions options)
        {
            _visitor = new RemoveDependencyVisitor(options?.Dependencies?.FirstOrDefault());
            return _visitor;
        }
    }
}
