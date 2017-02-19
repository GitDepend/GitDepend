using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Busi
{
    [TestFixture]
    public class NugetTests : TestFixtureBase
    {
        [Test]
        public void DefaultValuesTest()
        {
            var instance = new Nuget();

            Assert.IsNull(instance.WorkingDirectory);
        }

        [Test]
        public void UpdateTest()
        {
            string command = null;
            string arguments = null;
            string workingDirectory = null;
            var processManager = Container.Resolve<IProcessManager>();
            processManager.Arrange(p => p.Start(Arg.IsAny<ProcessStartInfo>()))
                .Returns((ProcessStartInfo info) =>
                {
                    command = info.FileName;
                    arguments = info.Arguments;
                    workingDirectory = info.WorkingDirectory;

                    return new FakeProcess();
                });

            var workingDir = @"C:\projects\TestProject\";
            var solution = "TestProject.sln";
            var id = "Newtonsoft.Json";
            var version = "9.0.2-alpha0003";
            var sourceDirectory = @"C:\NuGet";

            var instance = new Nuget() { WorkingDirectory = workingDir };
            var code = instance.Update(solution, id, version, sourceDirectory);

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual("NuGet.exe", command, "Invalid Command");
            Assert.AreEqual($"update {solution} -Id {id} -Version {version} -Source \"{sourceDirectory}\" -Pre", arguments, "Invalid Arguments");
            Assert.AreEqual(workingDir, workingDirectory, "Invalid Working Directory");
        }
    }
}
