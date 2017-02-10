using System;
using GitDepend.Commands;

namespace GitDepend
{
	class Program
	{
		static void Main(string[] args)
		{
			var parser = new CommandParser();
			var command = parser.GetCommand(args);

			var code = command.Execute();

			Environment.ExitCode = code;
		}
	}
}
