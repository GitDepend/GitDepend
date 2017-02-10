using GitDepend.Configuration;

namespace GitDepend.Visitors
{
	public interface IVisitor
	{
		int ReturnCode { get; set; }
		int VisitDependency(Dependency dependency);
	}
}