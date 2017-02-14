using System;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Visitors;

namespace GitDepend.Commands
{
	/// <summary>
	/// An implementation of <see cref="ICommand"/> that ensures that all dependencies have been cloned
	/// </summary>
	public class CloneCommand : ICommand
	{
		/// <summary>
		/// The name of the verb.
		/// </summary>
		public const string Name = "clone";
		private readonly CloneSubOptions _options;
		private readonly IFileIo _fileIo;

		/// <summary>
		/// Creates a new <see cref="CloneCommand"/>
		/// </summary>
		/// <param name="options">The <see cref="CloneSubOptions"/> that configure this command.</param>
		/// <param name="fileIo">The <see cref="IFileIo"/> to use.</param>
		public CloneCommand(CloneSubOptions options, IFileIo fileIo)
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
