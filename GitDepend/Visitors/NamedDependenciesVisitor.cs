using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using GitDepend.Busi;
using GitDepend.Configuration;
using GitDepend.Resources;

namespace GitDepend.Visitors
{
    /// <summary>
    /// An implementation of <see cref="IVisitor"/> that only operates on the specified
    /// dependencies.
    /// </summary>
    public abstract class NamedDependenciesVisitor : IVisitor
    {
        private readonly IList<string> _whitelist;

        /// <summary>
        /// The <see cref="IConsole"/> to use in this visitor.
        /// </summary>
        protected readonly IConsole Console;

        /// <summary>
        /// The <see cref="IFileSystem"/> object.
        /// </summary>
        protected readonly IFileSystem FileSystem;

        /// <summary>
        /// Creates a new <see cref="DisplayStatusVisitor"/>
        /// </summary>
        /// <param name="whitelist">The projects to visit. If this list is null or empty all projects will be visited.</param>
        protected NamedDependenciesVisitor(IList<string> whitelist)
        {
            _whitelist = whitelist ?? new List<string>();
            Console = DependencyInjection.Resolve<IConsole>();

            FileSystem = DependencyInjection.Resolve<IFileSystem>();
        }

        /// <summary>
        /// Provides the custom hook for VisitDependency. This will only be called if the dependency
        /// was specified in the whitelist.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="dependency">The <see cref="Dependency"/> to visit.</param>
        /// <returns></returns>
        protected abstract ReturnCode OnVisitDependency(string directory, Dependency dependency);

        #region Implementation of IVisitor

        /// <summary>
        /// The return code
        /// </summary>
        public ReturnCode ReturnCode { get; set; }

        /// <summary>
        /// Visits a project dependency.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="dependency">The <see cref="Dependency"/> to visit.</param>
        /// <returns>The return code.</returns>
        public ReturnCode VisitDependency(string directory, Dependency dependency)
        {
            var shouldExecute = _whitelist.Count == 0 ||
                                _whitelist.Any(d => string.Equals(d, dependency.Configuration.Name, StringComparison.CurrentCultureIgnoreCase));

            if (!shouldExecute)
            {
                return ReturnCode.Success;
            }

            var dir = FileSystem.Path.GetFullPath(FileSystem.Path.Combine(directory, dependency.Directory));

            var origColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(strings.DEPENDENCY);
            Console.WriteLine(strings.NAME + dependency.Configuration.Name);
            Console.WriteLine(strings.DIRECTORY + dir);
            Console.WriteLine();
            Console.ForegroundColor = origColor;

            return ReturnCode = OnVisitDependency(directory, dependency);
        }

        /// <summary>
        /// Visits a project.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="config">The <see cref="GitDependFile"/> with project configuration information.</param>
        /// <returns>The return code.</returns>
        public virtual ReturnCode VisitProject(string directory, GitDependFile config)
        {
            return ReturnCode = ReturnCode.Success;
        }

        #endregion
    }
}