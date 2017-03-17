﻿using System;
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
    public class RemoveCommand : ICommand
    {
        /// <summary>
        /// The name
        /// </summary>
        public const string Name = "remove";

        private readonly RemoveSubOptions _options;
        
        private readonly IDependencyVisitorAlgorithm _algorithm;
        private readonly IConsole _console;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommand"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public RemoveCommand(RemoveSubOptions options)
        {
            _options = options;
            _algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
            _console = DependencyInjection.Resolve<IConsole>();
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public ReturnCode Execute()
        {
            var visitor = new RemoveDependencyVisitor(_options.DependencyName);

            _algorithm.TraverseDependencies(visitor, _options.Directory);

            if (visitor.ReturnCode == ReturnCode.NameDidNotMatchRequestedDependency)
            {
                _console.WriteLine(strings.ResourceManager.GetString("RET_NAME_DID_NOT_MATCH", CultureInfo.CurrentCulture));
                return visitor.ReturnCode;
            }

            return visitor.ReturnCode;
        }
    }
}
