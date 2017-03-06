using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Configuration;
using LibGit2Sharp;

namespace GitDepend.Commands
{
    /// <summary>
    /// Adds a dependency to the current configuration.
    /// </summary>
    /// <seealso cref="GitDepend.Commands.ICommand" />
    public class AddCommand : ICommand
    {
        /// <summary>
        /// The name of the verb.
        /// </summary>
        public const string Name = "add";

        private readonly AddSubOptions _options;
        private readonly IGitDependFileFactory _factory;
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddCommand"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public AddCommand(AddSubOptions options)
        {
            _options = options;
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public ReturnCode Execute()
        {
            string dir;
            ReturnCode code;

            var config = _factory.LoadFromDirectory(_options.Directory, out dir, out code);

            if (code == ReturnCode.Success)
            {
                Dependency dep = new Dependency()
                {
                    Directory = _options.DependencyDirectory,
                    Branch = _options.Branch,
                    Url = _options.Url
                };

                config.Dependencies.Add(dep);

                _fileSystem.File.WriteAllText(_fileSystem.Path.Combine(_options.Directory, "GitDepend.json"), config.ToString());

                Console.WriteLine($"Dependency with dir {_options.Directory} has been added successfully.");
            }

            return code;
        }
    }
}
