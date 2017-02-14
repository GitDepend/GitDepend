using System;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
	/// <summary>
	/// An implementation of <see cref="IVisitor"/> that ensures that the correct branch
	/// has been checked out on each dependency.
	/// </summary>
	public class CheckOutBranchVisitor : IVisitor
	{
		private readonly IGit _git;

		/// <summary>
		/// Creates a new <see cref="CheckOutBranchVisitor"/>
		/// </summary>
		/// <param name="git">The <see cref="IGit"/> to use.</param>
		public CheckOutBranchVisitor(IGit git)
		{
			_git = git;
		}

		#region Implementation of IVisitor

		/// <summary>
		/// The return code
		/// </summary>
		public ReturnCode ReturnCode { get; set; }

		/// <summary>
		/// Visits a project dependency.
		/// </summary>
		/// <param name="dependency">The <see cref="Dependency"/> to visit.</param>
		/// <returns>The return code.</returns>
		public ReturnCode VisitDependency(Dependency dependency)
		{
			Console.WriteLine($"Checking out the {dependency.Branch} branch on {dependency.Name}");
			
			_git.WorkingDir = dependency.Directory;
			return _git.Checkout(dependency.Branch);
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