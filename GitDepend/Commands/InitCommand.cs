using System;
using System.IO.Abstractions;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Configuration;

namespace GitDepend.Commands
{
	/// <summary>
	/// An implementation of <see cref="ICommand"/> assists the user to create or modify their GitDepend.json file.
	/// </summary>
	public class InitCommand : ICommand
	{
		private readonly InitSubOptions _options;
		private readonly IFileSystem _fileSystem;
		private readonly IConsole _console;
		private readonly GitDependFileFactory _factory;

		/// <summary>
		/// The name of the verb.
		/// </summary>
		public const string Name = "init";

		/// <summary>
		/// Creates a new <see cref="InitCommand"/>
		/// </summary>
		/// <param name="options">The <see cref="InitSubOptions"/> that configures the command.</param>
		/// <param name="fileSystem">The <see cref="IFileSystem"/> to use.</param>
		/// <param name="console">The <see cref="IConsole"/> to use.</param>
		public InitCommand(InitSubOptions options, IFileSystem fileSystem, IConsole console)
		{
			_options = options;
			_fileSystem = fileSystem;
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
			var config = _factory.LoadFromDirectory(_options.Directory, out dir, out code) ?? new GitDependFile();

			if (code != ReturnCode.Success)
			{
				return code;
			}

			_console.Write($"build script [{config.Build.Script}]: ");
			config.Build.Script = ReadLine(config.Build.Script);

			_console.Write($"artifacts dir [{config.Packages.Directory}]: ");
			config.Packages.Directory = ReadLine(config.Packages.Directory);

			var path = _fileSystem.Path.Combine(dir, "GitDepend.json");

			_fileSystem.File.WriteAllText(path, config.ToString());

			return ReturnCode.Success;
		}

		private string ReadLine(string defaultValue)
		{
			var input = _console.ReadLine();
			if (string.IsNullOrEmpty(input))
			{
				input = defaultValue;
			}
			return input;
		}

		#endregion
	}
}
