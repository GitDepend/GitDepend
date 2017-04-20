using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
    /// <summary>
    /// This visitor goes through each dependency (or named dependencies) and calls git log with the given arguments.
    /// </summary>
    public class LogVisitor : NamedProjectsVisitor
    {
        private IGit _git;
        private string _gitArguments;

        /// <summary>
        /// This visitor calls log with the given arguments each directory or the named directories in the whitelist.
        /// </summary>
        /// <param name="gitArguments">The list of arguments to provide to git pull</param>
        /// <param name="whitelist">The dependencies to visit</param>
        public LogVisitor(string gitArguments, IList<string> whitelist) : base(whitelist)
        {
            _git = DependencyInjection.Resolve<IGit>();
            _gitArguments = gitArguments;
        }

        /// <summary>
        /// Gets the git log for the project.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        protected override ReturnCode OnVisitProject(string directory, GitDependFile config)
        {
            _git.WorkingDirectory = directory;
            var returnCode = _git.Log(_gitArguments);
            if (returnCode == ReturnCode.FailedToRunGitCommand)
            {
                return ReturnCode = ReturnCode.Success;
            }
            return ReturnCode = returnCode;
        }
    }
}
