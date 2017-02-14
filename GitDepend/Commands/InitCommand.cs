using System;
using System.IO;
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
		private readonly GitDependFileFactory _factory;

		/// <summary>
		/// The name of the verb.
		/// </summary>
		public const string Name = "init";

		/// <summary>
		/// Creates a new <see cref="InitCommand"/>
		/// </summary>
		/// <param name="options">The <see cref="InitSubOptions"/> that configures the command.</param>
		/// <param name="fileIo">The <see cref="IFileIo"/> to use.</param>
		public InitCommand(InitSubOptions options, IFileIo fileIo)
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
			var config = _factory.LoadFromDirectory(_options.Directory, out dir, out error) ?? new GitDependFile();

			Console.Write($"build script [{config.Build.Script}]: ");
			config.Build.Script = ReadLine(config.Build.Script);

			Console.Write($"artifacts dir [{config.Packages.Directory}]: ");
			config.Packages.Directory = ReadLine(config.Packages.Directory);

			var path = Path.Combine(dir, "GitDepend.json");

			File.WriteAllText(path, config.ToString());

			return ReturnCodes.Success;
		}

		private static string ReadLine(string defaultValue)
		{
			var input = Console.ReadLine();
			if (string.IsNullOrEmpty(input))
			{
				input = defaultValue;
			}
			return input;
		}

		#endregion
	}
}
