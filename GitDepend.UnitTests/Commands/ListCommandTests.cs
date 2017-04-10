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
    public class ListCommandTests : TestFixtureBase
    {
        [Test]
        public void Execute_ShouldReturn_Error_WhenLoadFromDirectoryFails()
        {
            var factory = Container.Resolve<IGitDependFileFactory>();

            string dir = null;
            ReturnCode loadCode = ReturnCode.DirectoryDoesNotExist;
            factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode))
                .Returns(null as GitDependFile);

            var options = new ListSubOptons();
            var instance = new ListCommand(options);

            var code = instance.Execute();
            Assert.AreEqual(ReturnCode.DirectoryDoesNotExist, code, "Invalid Return Code");
        }

        [Test]
        public void Execute_ShouldReturn_Success_And_PrintDependencies_WhenLoadFromDirectorySucceeds()
        {
            var factory = Container.Resolve<IGitDependFileFactory>();
            var console = Container.Resolve<IConsole>();

            StringBuilder output = new StringBuilder();
            console.Arrange(c => c.WriteLine(Arg.AnyString, Arg.IsAny<object[]>()))
                .DoInstead((string format, object[] args) =>
                {
                    output.AppendLine(string.Format(format, args));
                });

            string dir = Lib2Directory;
            ReturnCode loadCode = ReturnCode.Success;
            factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode))
                .Returns(Lib2Config);

            var options = new ListSubOptons();
            var instance = new ListCommand(options);

            var code = instance.Execute();

            const string EXPECTED = "- Lib2 ()\r\n  \r\n" +
                                    "    - Lib1 (expected develop but was)\r\n      \r\n";
            var actual = output.ToString();

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual(EXPECTED, actual, "Invalid Output");
        }
    }
}
