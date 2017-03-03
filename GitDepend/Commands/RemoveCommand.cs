using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;

namespace GitDepend.Commands
{
    /// <summary>
    /// This removes a dependency
    /// </summary>
    /// <seealso cref="GitDepend.Commands.ICommand" />
    public class RemoveCommand : ICommand
    {
        private readonly RemoveSubOptions _options;
        private readonly IGitDependFileFactory _factory;
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommand"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="fileSystem">The file system.</param>
        public RemoveCommand(RemoveSubOptions options, IGitDependFileFactory factory, IFileSystem fileSystem)
        {
            _options = options;
            _factory = factory;
            _fileSystem = fileSystem;
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
