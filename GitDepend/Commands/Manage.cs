using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;

namespace GitDepend.Commands
{
    /// <summary>
    /// This command lets you manage the dependencies and their configuration files.
    /// </summary>
    public class Manage : ICommand
    {
        /// <summary>
        /// The name of the verb.
        /// </summary>
        public const string Name = "manage";

        private readonly ManageSubOptions _options;
        private readonly IGitDependFileFactory _factory;
        private readonly IConsole _console;

        /// <summary>
        /// Initializes a new instance of the <see cref="Manage"/> class.
        /// </summary>
        /// <param name="options">The <see cref="ManageSubOptions"/> that configure manage</param>
        public Manage(ManageSubOptions options)
        {
            _options = options;
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _console = DependencyInjection.Resolve<IConsole>();

        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public ReturnCode Execute()
        {
            throw new NotImplementedException();
        }
    }
}
