using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Commands;
using GitDepend.Visitors;
using Microsoft.Practices.Unity;

namespace GitDepend
{
    /// <summary>
    /// Configures Unity for the project.
    /// </summary>
    public static class UnityConfig
    {
        /// <summary>
        /// Registers object mappings on the given <see cref="UnityContainer"/>
        /// </summary>
        /// <param name="container">The <see cref="UnityContainer"/> to use for object mappings.</param>
        public static void RegisterTypes(UnityContainer container)
        {
            // Busi
            container
                .RegisterType<IConsole, ConsoleWrapper>()
                .RegisterType<IGit, Git>()
                .RegisterType<IGitDependFileFactory, GitDependFileFactory>()
                .RegisterType<INuget, Nuget>()
                .RegisterType<IProcessManager, ProcessManager>()
                .RegisterType<IUiStrings, UiStrings>()
                .RegisterType<IVersionUpdateChecker, GitHubVersionUpdateChecker>();

            // Visitor
            container
                .RegisterType<IDependencyVisitorAlgorithm, DependencyVisitorAlgorithm>();

            // External
            container
                .RegisterType<IFileSystem, FileSystem>();
        }
    }
}
