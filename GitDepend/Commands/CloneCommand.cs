using System;
using GitDepend.CommandLine;
using GitDepend.Visitors;

namespace GitDepend.Commands
{
	public class CloneCommand : ICommand
	{
		public const string Name = "clone";
		private readonly CloneSubOptions _options;

		public CloneCommand(CloneSubOptions options)
		{
			_options = options;
		}

		#region Implementation of ICommand

		public int Execute()
		{
			var alg = new DependencyVisitorAlgorithm();
			var visitor = new CheckOutBranchVisitor();
			alg.TraverseDependencies(visitor, _options.Directory);

			if (visitor.ReturnCode == ReturnCodes.Success)
			{
				Console.WriteLine("Successfully cloned all dependencies");
			}

			return visitor.ReturnCode;
		}

		#endregion
	}
}
