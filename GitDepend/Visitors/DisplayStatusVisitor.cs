using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
    /// <summary>
    /// An implementatio of <see cref="IVisitor"/> that displays the status of all dependencies.
    /// </summary>
    public class DisplayStatusVisitor : IVisitor
    {
        private readonly IList<string> _whilelist;
        private readonly IGit _git;
        private readonly IFileSystem _fileSystem;
        private readonly IConsole _console;

        /// <summary>
        /// Creates a new <see cref="DisplayStatusVisitor"/>
        /// </summary>
        /// <param name="whilelist">The projects to visit. If this list is null or empty all projects will be visited.</param>
        public DisplayStatusVisitor(IList<string> whilelist)
        {
            _whilelist = whilelist ?? new List<string>();
            _git = DependencyInjection.Resolve<IGit>();
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
            _console = DependencyInjection.Resolve<IConsole>();
        }

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
            var shouldExecute = _whilelist.Count == 0 ||
                _whilelist.Any(d => string.Equals(d, dependency.Name, StringComparison.CurrentCultureIgnoreCase));

            if (!shouldExecute)
            {
                return ReturnCode.Success;
            }

            var dir = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(directory, dependency.Directory));

            var origColor = _console.ForegroundColor;
            _console.ForegroundColor = ConsoleColor.Green;
            _console.WriteLine("dependency:");
            _console.WriteLine($"    name: {dependency.Name}");
            _console.WriteLine($"    dir: {dir}");
            _console.WriteLine();
            _console.ForegroundColor = origColor;

            _git.WorkingDirectory = directory;
            return ReturnCode = _git.Status();
        }

        /// <summary>
        /// Visists a project.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="config">The <see cref="GitDependFile"/> with project configuration information.</param>
        /// <returns>The return code.</returns>
        public ReturnCode VisitProject(string directory, GitDependFile config)
        {
            return ReturnCode.Success;
        }

        #endregion
    }
}
