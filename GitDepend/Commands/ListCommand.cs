using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Configuration;
using GitDepend.Resources;

namespace GitDepend.Commands
{
    /// <summary>
    /// An implementation of <see cref="ICommand"/> that lists all dependencies.
    /// </summary>
    public class ListCommand : ICommand
    {
        /// <summary>
        /// The name of the verb.
        /// </summary>
        public const string Name = "list";

        private readonly ListSubOptons _options;
        private readonly IGitDependFileFactory _factory;
        private readonly IConsole _console;
        private readonly IGit _git;
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// Creates a new <see cref="ListCommand"/>
        /// </summary>
        /// <param name="options">The <see cref="ListSubOptons"/> that configure this command.</param>
        public ListCommand(ListSubOptons options)
        {
            _options = options;
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _console = DependencyInjection.Resolve<IConsole>();
            _git = DependencyInjection.Resolve<IGit>();
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
        }

        #region Implementation of ICommand

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>The return code.</returns>
        public ReturnCode Execute()
        {
            _git.WorkingDirectory = _options.Directory;

            string dir;
            ReturnCode code;
            var config = _factory.LoadFromDirectory(_options.Directory, out dir, out code);
            var currBranch = _git.GetCurrentBranch();

            if (code == ReturnCode.Success && config != null)
            {
                _console.WriteLine($"- {config.Name} ({currBranch})");
                _console.WriteLine($"  {_options.Directory}");
                _console.WriteLine();
                foreach (var dependency in config.Dependencies)
                {
                    WriteDependency(dependency, "    ");
                }
            }

            return code;
        }

        private void WriteDependency(Dependency dependency, string indent)
        {
            if (string.IsNullOrEmpty(dependency.Configuration.Name))
            {
                _console.WriteLine(string.Format(strings.DEPENDENCY_MISSING_NAME, indent, dependency.Directory));
            }
            else
            {
                _git.WorkingDirectory = _fileSystem.Path.GetFullPath(dependency.Directory);
                var currBranch = _git.GetCurrentBranch();
                _console.WriteLine($"{indent}- {dependency.Configuration.Name}" + (currBranch == dependency.Branch ? $" ({currBranch})"
                                       : $" ({string.Format(strings.EXPECTED_BRANCH_BUT_WAS_BRANCH, dependency.Branch, currBranch).Trim()})"));

                _console.WriteLine($"{indent}  {_git.WorkingDirectory}");
                _console.WriteLine();
                foreach (var subDependency in dependency.Configuration.Dependencies)
                {
                    WriteDependency(subDependency, indent + "    ");
                }
            }
        }

        #endregion
    }
}
