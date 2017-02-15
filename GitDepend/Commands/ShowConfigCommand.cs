using System;
using System.IO.Abstractions;
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
		private readonly IConsole _console;
		private readonly GitDependFileFactory _factory;

		/// <summary>
		/// The name of the verb.
		/// </summary>
		public const string Name = "config";

		/// <summary>
		/// Creates a new <see cref="ShowConfigCommand"/>
		/// </summary>
		/// <param name="options">The <see cref="ConfigSubOptions"/> that configures the command.</param>
		/// <param name="fileSystem">The <see cref="IFileSystem"/> to use.</param>
		/// <param name="console">The <see cref="IConsole"/> to use.</param>
		public ShowConfigCommand(ConfigSubOptions options, IFileSystem fileSystem, IConsole console)
		{
			_options = options;
			_console = console;
			_factory = new GitDependFileFactory(fileSystem, console);
		}

		#region Implementation of ICommand

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <returns>The return code.</returns>
		public ReturnCode Execute()
		{
			string dir;
			ReturnCode code;
			var file = _factory.LoadFromDirectory(_options.Directory, out dir, out code);

			if (code == ReturnCode.Success)
			{
				_console.WriteLine(file);
			}

			return code;
		}

		#endregion
	}
}
