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
        void TraverseDependencies(IVisitor visitor, string directory);

        /// <summary>
        /// Resets the algorithm to a default state.
        /// </summary>
        void Reset();
    }
}