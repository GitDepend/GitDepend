using System;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Resources;
using GitDepend.Visitors;

namespace GitDepend.Commands
{
    /// <summary>
    /// An implementaton of <see cref="ICommand"/> that operates only on the named dependencies.
    /// </summary>
    public abstract class NamedDependenciesCommand<T> : ICommand where T : NamedDependenciesOptions
    {
        /// <summary>
        /// The options for the command.
        /// </summary>
        protected readonly T Options;

        private readonly IDependencyVisitorAlgorithm _algorithm;

        /// <summary>
        /// The <see cref="IConsole"/> for this command.
        /// </summary>
        protected readonly IConsole Console;

        /// <summary>
        /// Creates a new <see cref="NamedDependenciesCommand{T}"/>
        /// </summary>
        /// <param name="options">The options for the command.</param>
        protected NamedDependenciesCommand(T options)
        {
            Options = options;
            _algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
            Console = DependencyInjection.Resolve<IConsole>();
        }

        #region Implementation of ICommand

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>The return code.</returns>
        public ReturnCode Execute()
        {
            var visitor = CreateVisitor(Options);
            _algorithm.TraverseDependencies(visitor, Options.Directory);

            var code = visitor.ReturnCode;
            if (code == ReturnCode.Success)
            {
                var origColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(strings.PROJECT);
                Console.WriteLine(strings.DIRECTORY + Options.Directory);
                Console.WriteLine();
                Console.ForegroundColor = origColor;

                code = AfterDependencyTraversal(Options);
            }

            return code;
        }

        #endregion

        /// <summary>
        /// Creates the visitor that will be used to traverse the dependency graph.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected abstract NamedDependenciesVisitor CreateVisitor(T options);

        /// <summary>
        /// Executes after all dependencies have been visited.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected virtual ReturnCode AfterDependencyTraversal(T options)
        {
            return ReturnCode.Success;
        }
    }
}