using System;
using System.IO.Abstractions;
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
		private readonly IGitDependFileFactory _factory;
		private readonly IGit _git;
		private readonly IFileSystem _fileIo;

		/// <summary>
		/// Creates a new <see cref="CloneCommand"/>
		/// </summary>
		/// <param name="options">The <see cref="CloneSubOptions"/> that configure this command.</param>
		/// <param name="factory">The <see cref="IGitDependFileFactory"/> to use.</param>
		/// <param name="git">The <see cref="IGit"/> to use.</param>
		/// <param name="fileIo">The <see cref="IFileSystem"/> to use.</param>
		public CloneCommand(CloneSubOptions options, IGitDependFileFactory factory, IGit git, IFileSystem fileIo)
		{
			_options = options;
			_factory = factory;
			_git = git;
			_fileIo = fileIo;
		}

		#region Implementation of ICommand

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <returns>The return code.</returns>
		public ReturnCode Execute()
		{
			var alg = new DependencyVisitorAlgorithm(_factory, _git, _fileIo);
			var visitor = new CheckOutBranchVisitor(_git);
			alg.TraverseDependencies(visitor, _options.Directory);

			if (visitor.ReturnCode == ReturnCode.Success)
			{
				Console.WriteLine("Successfully cloned all dependencies");
			}

			return visitor.ReturnCode;
		}

		#endregion
	}
}
