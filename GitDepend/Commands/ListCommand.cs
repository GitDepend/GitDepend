using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Configuration;

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

        /// <summary>
        /// Creates a new <see cref="ListCommand"/>
        /// </summary>
        /// <param name="options">The <see cref="ListSubOptons"/> that configure this command.</param>
        public ListCommand(ListSubOptons options)
        {
            _options = options;
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _console = DependencyInjection.Resolve<IConsole>();
        }

        #region Implementation of ICommand

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>The return code.</returns>
        public ReturnCode Execute()
        {
            string dir;
            ReturnCode code;
            var config = _factory.LoadFromDirectory(_options.Directory, out dir, out code);

            if (code == ReturnCode.Success && config != null)
            {
                _console.WriteLine($"- {config.Name}");
                foreach (var dependency in config.Dependencies)
                {
                    WriteDependency(dependency, "    ");
                }
            }

            return code;
        }

        private void WriteDependency(Dependency dependency, string indent)
        {
            _console.WriteLine($"{indent}- {dependency.Configuration.Name}");
            foreach (var subDependency in dependency.Configuration.Dependencies)
            {
                WriteDependency(subDependency, indent + "    ");
            }
        }

        #endregion
    }
}
