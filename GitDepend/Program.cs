using System;
using System.IO.Abstractions;
using GitDepend.Busi;
using GitDepend.Commands;
using Microsoft.Practices.Unity;

namespace GitDepend
{
    class Program
    {
        static void Main(string[] args)
        {
            DependencyInjection.Container = new UnityContainer();
            UnityConfig.RegisterTypes(DependencyInjection.Container);

            var parser = new CommandParser();
            var command = parser.GetCommand(args);

            if (command == null)
            {
                Environment.ExitCode = (int)ReturnCode.InvalidCommand;
                Console.Error.WriteLine("Invalid Command!");
            }
            else
            {
                var code = command.Execute();
                Environment.ExitCode = (int)code;
            }
        }
    }
}
