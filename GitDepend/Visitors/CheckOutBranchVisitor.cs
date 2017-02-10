using System;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
	public class CheckOutBranchVisitor : IVisitor
	{
		#region Implementation of IVisitor

		public int ReturnCode { get; set; }	

		public int VisitDependency(Dependency dependency)
		{
			Console.WriteLine($"Checking out the {dependency.Branch} branch on {dependency.Name}");

			var git = new Git(dependency.Directory);
			return git.Checkout(dependency.Branch);
		}

		#endregion
	}
}