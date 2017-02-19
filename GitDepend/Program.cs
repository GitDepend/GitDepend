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
            UnityConfig.RegisterTypes(DependencyInjection.Container);

            var parser = new CommandParser();
            var command = parser.GetCommand(args);

            var code = command.Execute();

            Environment.ExitCode = (int)code;
        }
    }
}
