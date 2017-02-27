using System.Collections.Generic;
using System.Linq;
using GitDepend.Busi;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
    /// <summary>
    /// An implementation of <see cref="NamedDependenciesVisitor"/> that synchronizes the branch specified for a dependency
    /// in GitDepend.json with the currently checked out branch for that dependency.
    /// </summary>
    public class SyncConfigWithCurrentBranchVisitor : NamedDependenciesVisitor
    {
        private readonly IGitDependFileFactory _factory;
        private readonly IGit _git;

        /// <summary>
        /// Creates a new <see cref="DisplayStatusVisitor"/>
        /// </summary>
        /// <param name="whilelist">The projects to visit. If this list is null or empty all projects will be visited.</param>
        public SyncConfigWithCurrentBranchVisitor(IList<string> whilelist) : base(whilelist)
        {
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _git = DependencyInjection.Resolve<IGit>();
        }

        #region Overrides of NamedDependenciesVisitor

        /// <summary>
        /// Provides the custom hook for VisitDependency. This will only be called if the dependency
        /// was specified in the whitelist.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="dependency">The <see cref="Dependency"/> to visit.</param>
        /// <returns></returns>
        protected override ReturnCode OnVisitDependency(string directory, Dependency dependency)
        {
            string dir;
            ReturnCode code;
            var config = _factory.LoadFromDirectory(directory, out dir, out code);

            if (code != ReturnCode.Success)
            {
                return code;
            }

            bool dirty = false;
            foreach (var dep in config.Dependencies)
            {
                var path = FileSystem.Path.GetFullPath(FileSystem.Path.Combine(directory, dep.Directory));
                _git.WorkingDirectory = path;
                var branch = _git.GetCurrentBranch();

                if (dep.Branch != branch)
                {
                    dep.Branch = branch;
                    dirty = true;
                    Console.WriteLine($"using {dep.Branch} for {dep.Configuration.Name}");
                }
            }

            if (dirty)
            {
                FileSystem.File.WriteAllText(FileSystem.Path.Combine(directory, "GitDepend.json"), config.ToString());
            }

            Console.WriteLine($"all dependency branches synchronized successfully!");

            return ReturnCode.Success;
        }

        #endregion
    }
}