using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Commands;
using GitDepend.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Commands
{
    [TestFixture]
    public class ShowConfigCommandTests : TestFixtureBase
    {
        [Test]
        public void Execute_ShouldPrintDefaultConfig_WhenConfigDoesNotExistInGitRepo()
        {
            var console = Container.Resolve<IConsole>();
            var factory = Container.Resolve<IGitDependFileFactory>();
            string dir;
            ReturnCode loadCode = ReturnCode.Success;
            factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode))
                .Returns(new GitDependFile());

            StringBuilder output = new StringBuilder();
            console.Arrange(c => c.WriteLine(Arg.AnyObject))
                .DoInstead((object obj) =>
                {
                    output.AppendLine(obj.ToString());
                });

            var options = new ConfigSubOptions();
            var instance = new ConfigCommand(options);

            var code = instance.Execute();
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual(new GitDependFile().ToString() + Environment.NewLine, output.ToString());
        }

        [Test]
        public void Execute_ShouldPrintExistingConfig_WhenConfigExistsInGitRepo()
        {
            var console = Container.Resolve<IConsole>();
            var factory = Container.Resolve<IGitDependFileFactory>();
            string dir;
            ReturnCode loadCode = ReturnCode.Success;
            factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode))
                .Returns(Lib2Config);

            StringBuilder output = new StringBuilder();
            console.Arrange(c => c.WriteLine(Arg.AnyObject))
                .DoInstead((object obj) =>
                {
                    output.AppendLine(obj.ToString());
                });

            var options = new ConfigSubOptions();
            var instance = new ConfigCommand(options);

            var code = instance.Execute();
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual(Lib2Config.ToString() + Environment.NewLine, output.ToString());
        }

        [Test]
        public void Execute_ShouldReturnError_WhenLoadFails()
        {
            var console = Container.Resolve<IConsole>();
            var factory = Container.Resolve<IGitDependFileFactory>();
            string dir;
            ReturnCode loadCode = ReturnCode.GitRepositoryNotFound;
            factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode))
                .Returns(null as GitDependFile);

            StringBuilder output = new StringBuilder();
            console.Arrange(c => c.WriteLine(Arg.AnyObject))
                .DoInstead((object obj) =>
                {
                    output.AppendLine(obj.ToString());
                });

            var options = new ConfigSubOptions();
            var instance = new ConfigCommand(options);

            var code = instance.Execute();
            Assert.AreEqual(ReturnCode.GitRepositoryNotFound, code, "Invalid Return Code");
            Assert.AreEqual(string.Empty, output.ToString());
        }
    }
}
