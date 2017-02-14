using System;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Visitors;

namespace GitDepend.Commands
{
	/// <summary>
	/// An implementation of <see cref="ICommand"/> that recursively updates all dependencies.
	/// </summary>
	public class UpdateCommand : ICommand
	{
		private readonly UpdateSubOptions _options;
		private readonly IFileIo _fileIo;

		/// <summary>
		/// The verb name
		/// </summary>
		public const string Name = "update";

		/// <summary>
		/// Creates a new <see cref="UpdateCommand"/>
		/// </summary>
		/// <param name="options">The <see cref="UpdateSubOptions"/> that configure the command.</param>
		/// <param name="fileIo">The <see cref="IFileIo"/> to use.</param>
		public UpdateCommand(UpdateSubOptions options, IFileIo fileIo)
		{
			_options = options;
			_fileIo = fileIo;
		}

		#region Implementation of ICommand

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <returns>The return code.</returns>
		public int Execute()
		{
			var alg = new DependencyVisitorAlgorithm(_fileIo);
			IVisitor visitor = new CheckOutBranchVisitor();
			alg.TraverseDependencies(visitor, _options.Directory);

			if (visitor.ReturnCode != ReturnCodes.Success)
			{
				Console.WriteLine("Could not ensure the correct branch on all dependencies.");
				return visitor.ReturnCode;
			}

			alg = new DependencyVisitorAlgorithm(_fileIo);
			visitor = new BuildAndUpdateDependenciesVisitor(_fileIo);
			alg.TraverseDependencies(visitor, _options.Directory);

			if (visitor.ReturnCode == ReturnCodes.Success)
			{
				Console.WriteLine("Update complete!");
			}

			return visitor.ReturnCode;
		}

		#endregion
	}
}
