using System;
using System.IO.Abstractions;
using GitDepend.Busi;
using GitDepend.Commands;

namespace GitDepend
{
	class Program
	{
		static void Main(string[] args)
		{
			var fileSystem = new FileSystem();
			var factory = new GitDependFileFactory(fileSystem);
			var processManager = new ProcessManager();
			var git = new Git(processManager);
			var nuget = new Nuget(processManager, fileSystem);

			var parser = new CommandParser();
			var command = parser.GetCommand(args, factory, git, nuget, processManager, fileSystem);

			var code = command.Execute();

			Environment.ExitCode = (int)code;
		}
	}
}
