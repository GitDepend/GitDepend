﻿using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
    /// <summary>
    /// An implementation of <see cref="IVisitor"/> that creates the specified branch on all dependencies.
    /// </summary>
    public class CreateBranchVisitor : IVisitor
    {
        private readonly string _branchName;
        private readonly IGit _git;
        private readonly IConsole _console;

        /// <summary>
        /// Creates a new <see cref="CreateBranchVisitor"/>
        /// </summary>
        /// <param name="branchName">The branch name to create.</param>
        public CreateBranchVisitor(string branchName)
        {
            _branchName = branchName;
            _git = DependencyInjection.Resolve<IGit>();
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
            return ReturnCode.Success;
        }

        /// <summary>
        /// Visists a project.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="config">The <see cref="GitDependFile"/> with project configuration information.</param>
        /// <returns>The return code.</returns>
        public ReturnCode VisitProject(string directory, GitDependFile config)
        {
            _console.WriteLine($"Creating the {_branchName} branch on {config.Name}");
            _git.WorkingDirectory = directory;
            return ReturnCode = _git.Branch(_branchName);
        }

        #endregion
    }
}
