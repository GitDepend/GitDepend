using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
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
    public class GitTests : TestFixtureBase
    {
        [Test]
        public void DefaultValuesTest()
        {
            var instance = new Git();

            Assert.IsNull(instance.WorkingDirectory);
        }

        [Test]
        public void CheckoutTest()
        {
            string command = null;
            string arguments = null;
            var processManager = Container.Resolve<IProcessManager>();
            processManager.Arrange(p => p.Start(Arg.IsAny<ProcessStartInfo>()))
                .Returns((ProcessStartInfo info) =>
                {
                    command = info.FileName;
                    arguments = info.Arguments;

                    return new FakeProcess();
                });

            var instance = new Git();
            var code = instance.Checkout("develop");

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual("git", command);
            Assert.AreEqual("checkout develop", arguments);
        }

        [Test]
        public void CloneTest()
        {
            string command = null;
            string arguments = null;
            var processManager = Container.Resolve<IProcessManager>();
            processManager.Arrange(p => p.Start(Arg.IsAny<ProcessStartInfo>()))
                .Returns((ProcessStartInfo info) =>
                {
                    command = info.FileName;
                    arguments = info.Arguments;

                    return new FakeProcess();
                });

            var instance = new Git();
            var url = "ssh://git@github.com:kjjuno/Lib1.git";
            var directory = @"C:\projects\Lib1";
            var branch = "master";
            var code = instance.Clone(url, directory, branch);

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual("git", command);
            Assert.AreEqual($"clone {url} \"{directory}\" -b {branch}", arguments);
        }

        [Test]
        public void AddTest()
        {
            string command = null;
            string arguments = null;
            var processManager = Container.Resolve<IProcessManager>();
            processManager.Arrange(p => p.Start(Arg.IsAny<ProcessStartInfo>()))
                .Returns((ProcessStartInfo info) =>
                {
                    command = info.FileName;
                    arguments = info.Arguments;

                    return new FakeProcess();
                });

            var instance = new Git();
            var code = instance.Add("packages.config");

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual("git", command);
            Assert.AreEqual("add \"packages.config\"", arguments);
        }

        [Test]
        public void StatusTest()
        {
            string command = null;
            string arguments = null;
            var processManager = Container.Resolve<IProcessManager>();
            processManager.Arrange(p => p.Start(Arg.IsAny<ProcessStartInfo>()))
                .Returns((ProcessStartInfo info) =>
                {
                    command = info.FileName;
                    arguments = info.Arguments;

                    return new FakeProcess();
                });

            var instance = new Git();
            var code = instance.Status();

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual("git", command);
            Assert.AreEqual("status", arguments);
        }

        [Test]
        public void CommitTest()
        {
            var fileSystem = Container.Resolve<IFileSystem>();

            var tempFile = @"C:\temp\somefile.dat";
            fileSystem.Path.Arrange(p => p.GetTempFileName())
                .Returns(tempFile);

            string command = null;
            string arguments = null;
            var processManager = Container.Resolve<IProcessManager>();
            processManager.Arrange(p => p.Start(Arg.IsAny<ProcessStartInfo>()))
                .Returns((ProcessStartInfo info) =>
                {
                    command = info.FileName;
                    arguments = info.Arguments;

                    return new FakeProcess();
                });

            var instance = new Git();
            var message = "This is a test message" + Environment.NewLine +
                          "With multiple lines";

            var code = instance.Commit(message);

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual("git", command);
            Assert.AreEqual($"commit --file=\"{tempFile}\"", arguments);
        }

        [Test]
        public void WorkingDirectoryTest()
        {
            string workingDir = null;
            var processManager = Container.Resolve<IProcessManager>();
            processManager.Arrange(p => p.Start(Arg.IsAny<ProcessStartInfo>()))
                .Returns((ProcessStartInfo info) =>
                {
                    workingDir = info.WorkingDirectory;

                    return new FakeProcess();
                });

            var instance = new Git() { WorkingDirectory = Lib1Directory };
            var message = "This is a test message";
            var code = instance.Commit(message);

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual(Lib1Directory, workingDir);
        }

        [Test]
        public void CreateBranchTest()
        {
            const string BRANCH = "feature/test_branch";

            string command = null;
            string arguments = null;
            var processManager = Container.Resolve<IProcessManager>();
            processManager.Arrange(p => p.Start(Arg.IsAny<ProcessStartInfo>()))
                .Returns((ProcessStartInfo info) =>
                {
                    command = info.FileName;
                    arguments = info.Arguments;

                    return new FakeProcess();
                });

            var instance = new Git();
            var code = instance.CreateBranch(BRANCH);

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual("git", command);
            Assert.AreEqual($"branch {BRANCH}", arguments);
        }

        [Test]
        public void DeleteBranchTest()
        {
            const string BRANCH = "feature/test_branch";

            string command = null;
            string arguments = null;
            var processManager = Container.Resolve<IProcessManager>();
            processManager.Arrange(p => p.Start(Arg.IsAny<ProcessStartInfo>()))
                .Returns((ProcessStartInfo info) =>
                {
                    command = info.FileName;
                    arguments = info.Arguments;

                    return new FakeProcess();
                });

            var instance = new Git();
            var code = instance.DeleteBranch(BRANCH, false);

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual("git", command);
            Assert.AreEqual($"branch -d {BRANCH}", arguments);
        }

        [Test]
        public void ForceDeleteBranchTest()
        {
            const string BRANCH = "feature/test_branch";

            string command = null;
            string arguments = null;
            var processManager = Container.Resolve<IProcessManager>();
            processManager.Arrange(p => p.Start(Arg.IsAny<ProcessStartInfo>()))
                .Returns((ProcessStartInfo info) =>
                {
                    command = info.FileName;
                    arguments = info.Arguments;

                    return new FakeProcess();
                });

            var instance = new Git();
            var code = instance.DeleteBranch(BRANCH, true);

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual("git", command);
            Assert.AreEqual($"branch -D {BRANCH}", arguments);
        }

        [Test]
        public void ListMergedBranchesTest()
        {
            string command = null;
            string arguments = null;
            var processManager = Container.Resolve<IProcessManager>();
            processManager.Arrange(p => p.Start(Arg.IsAny<ProcessStartInfo>()))
                .Returns((ProcessStartInfo info) =>
                {
                    command = info.FileName;
                    arguments = info.Arguments;

                    return new FakeProcess();
                });

            var instance = new Git();
            var code = instance.ListMergedBranches();

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual("git", command);
            Assert.AreEqual($"branch --merged", arguments);
        }

        [Test]
        public void ListAllBranchesTest()
        {
            string command = null;
            string arguments = null;
            var processManager = Container.Resolve<IProcessManager>();
            processManager.Arrange(p => p.Start(Arg.IsAny<ProcessStartInfo>()))
                .Returns((ProcessStartInfo info) =>
                {
                    command = info.FileName;
                    arguments = info.Arguments;

                    return new FakeProcess();
                });

            var instance = new Git();
            var code = instance.ListAllBranches();

            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual("git", command);
            Assert.AreEqual($"branch", arguments);
        }
    }
}
