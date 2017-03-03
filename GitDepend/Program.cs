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

                if (code != ReturnCode.Success)
                {
                    var strings = DependencyInjection.Resolve<IUiStrings>();
                    var console = DependencyInjection.Resolve<IConsole>();

                    var key = code.GetResxKey();
                    var str = strings.GetString(key);
                    console.WriteLine(str);
                }

                Environment.ExitCode = (int)code;
            }
        }
    }
}
