using System;
using GitDepend.Busi;
using GitDepend.Commands;

namespace GitDepend
{
	class Program
	{
		static void Main(string[] args)
		{
			var fileIo = new FileIo();
			var parser = new CommandParser();
			var command = parser.GetCommand(args, fileIo);

			var code = command.Execute();

			Environment.ExitCode = code;
		}
	}
}
