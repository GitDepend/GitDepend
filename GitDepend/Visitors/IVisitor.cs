using GitDepend.Configuration;

namespace GitDepend.Visitors
{
	/// <summary>
	/// An object intended to be used with the <see cref="DependencyVisitorAlgorithm"/>. Specific implementations
	/// can perform various actions as they visit dependencies and projects.
	/// </summary>
	public interface IVisitor
	{
		/// <summary>
		/// The return code
		/// </summary>
		ReturnCode ReturnCode { get; set; }

		/// <summary>
		/// Visits a project dependency.
		/// </summary>
		/// <param name="dependency">The <see cref="Dependency"/> to visit.</param>
		/// <returns>The return code.</returns>
		ReturnCode VisitDependency(Dependency dependency);

		/// <summary>
		/// Visists a project.
		/// </summary>
		/// <param name="directory">The directory of the project.</param>
		/// <param name="config">The <see cref="GitDependFile"/> with project configuration information.</param>
		/// <returns>The return code.</returns>
		ReturnCode VisitProject(string directory, GitDependFile config);
	}
}