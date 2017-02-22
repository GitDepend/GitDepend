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

            var code = command.Execute();

            Environment.ExitCode = (int)code;
        }
    }
}
