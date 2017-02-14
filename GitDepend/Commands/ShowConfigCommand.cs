using System;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Configuration;

namespace GitDepend.Commands
{
	/// <summary>
	/// An implementation of <see cref="ICommand"/> that displays the fully calculated configuration file.
	/// </summary>
	public class ShowConfigCommand : ICommand
	{
		private readonly ConfigSubOptions _options;
		private readonly GitDependFileFactory _factory;

		/// <summary>
		/// The name of the verb.
		/// </summary>
		public const string Name = "config";

		/// <summary>
		/// Creates a new <see cref="ShowConfigCommand"/>
		/// </summary>
		/// <param name="options">The <see cref="ConfigSubOptions"/> that configures the command.</param>
		/// <param name="fileIo">The <see cref="IFileIo"/> to use.</param>
		public ShowConfigCommand(ConfigSubOptions options, IFileIo fileIo)
		{
			_options = options;
			_factory = new GitDependFileFactory(fileIo);
		}

		#region Implementation of ICommand

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <returns>The return code.</returns>
		public int Execute()
		{
			string dir;
			string error;
			var file = _factory.LoadFromDirectory(_options.Directory, out dir, out error);

			if (file == null || !string.IsNullOrEmpty(error))
			{
				if (string.IsNullOrEmpty(error))
				{
					Console.Error.WriteLine("I'm not sure why, but I can't load the GitDepend.json file");
					Environment.Exit(1);
				}
				else
				{
					Console.Error.WriteLine(error);
					Environment.Exit(2);
				}
			}

			Console.WriteLine(file);
			return ReturnCodes.Success;
		}

		#endregion
	}
}
