using System.Collections.Generic;

namespace GitDepend.Visitors
{
    /// <summary>
    /// An algorithm that traverses a project and all its dependencies.
    /// </summary>
    public interface IDependencyVisitorAlgorithm
    {
        /// <summary>
        /// Traverses all dependencies beginning in the given directory.
        /// </summary>
        /// <param name="visitor">The <see cref="IVisitor"/> that should be called at each dependency and project visit.</param>
        /// <param name="directory">The directory containing a GitDepend.json file.</param>
        /// <param name="ignoreFilterFiles">Whether or not the .DepInclude files should be ignored.</param>
        void TraverseDependencies(IVisitor visitor, string directory, bool ignoreFilterFiles = false);

        /// <summary>
        /// Resets the algorithm to a default state.
        /// </summary>
        void Reset();

        /// <summary>
        /// List of projects to filter on
        /// </summary>
        List<string> FilterProjects { get; }

        /// <summary>
        /// Builds a list of projects that should be included in a filtered run
        /// </summary>
        /// <param name="directory">Starting directory of the dependency tree</param>
        /// <returns></returns>
        ReturnCode PreVisitDependencies(string directory);
    }
}