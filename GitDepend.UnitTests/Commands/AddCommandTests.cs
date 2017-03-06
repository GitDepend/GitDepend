using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.CommandLine;
using GitDepend.Commands;
using NUnit.Framework;

namespace GitDepend.UnitTests.Commands
{
    [TestFixture]
    public class AddCommandTests : TestFixtureBase
    {
        [Test]
        public void AddCommand_Succeeds()
        {
            var options = new AddSubOptions()
            {
                Branch = "master",
                DependencyDirectory = "../lib3",
                Url = "https://test-url.com/repos/lib3"
            };

            var command = new AddCommand(options);

            var code = command.Execute();

            Assert.AreEqual(ReturnCode.Success, code);
        }


    }
}
