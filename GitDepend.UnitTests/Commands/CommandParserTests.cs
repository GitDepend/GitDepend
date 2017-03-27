using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Commands;
using NUnit.Framework;

namespace GitDepend.UnitTests.Commands
{
    [TestFixture]
    public class CommandParserTests : TestFixtureBase
    {
        [Test]
        public void GetCommand_ShouldReturn_InitCommand_WhenInitVerbIsSpecified()
        {
            string[] args = { "init" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is InitCommand, "Invalid Command");
        }

        [Test]
        public void GetCommand_ShouldReturn_ShowConfigCommand_WhenCloneVerbIsSpecified()
        {
            string[] args = { "config" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is ConfigCommand, "Invalid Command");
        }

        [Test]
        public void GetCommand_ShouldReturn_CleanCommand_WhenCleanVerbIsSpecified()
        {
            string[] args = {"clean"};
            var instance = new CommandParser();
            var command = instance.GetCommand(args);

            Assert.IsTrue(command is CleanCommand, "Invalid Command");
        }

        [Test]
        public void GetCommand_ShouldReturn_CloneCommand_WhenCloneVerbIsSpecified()
        {
            string[] args = { "clone" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is CloneCommand, "Invalid Command");
        }

        [Test]
        public void GetCommand_ShouldReturn_UpdateCommand_WhenUpdateVerbIsSpecified()
        {
            string[] args = { "update" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is UpdateCommand, "Invalid Command");
        }

        [Test]
        public void GetCommand_ShouldReturn_StatusCommand_WhenStatusVerbIsSpecified()
        {
            string[] args = { "status", "lib1" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is StatusCommand, "Invalid Command");
        }

        [Test]
        public void GetCommand_ShouldReturn_ListCommand_WhenListVerbIsSpecified()
        {
            string[] args = { "list" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is ListCommand, "Invalid Command");
        }

        [Test]
        public void GetCommand_ShouldReturn_BranchCommand_WhenBranchVerbIsSpecified()
        {
            string[] args = { "branch" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is BranchCommand, "Invalid Command");
        }

        [Test]
        public void GetCommand_ShouldReturn_CheckOutCommand_WhenCheckOutVerbIsSpecified()
        {
            string[] args = { "checkout", "my_branch" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is CheckOutCommand, "Invalid Command");
        }

        [Test]
        public void GetCommand_ShouldReturn_SyncCommand_WhenCheckOutVerbIsSpecified()
        {
            string[] args = { "sync" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is SyncCommand, "Invalid Command");
        }

        [Test]
        public void GetCommand_ShouldReturn_AddCommand_WhenAddVerbIsSpecified()
        {
            string[] args = {"add", "--url", "myurl",  "--directory", "mydir", "--branch", "mybranch"};
            var instance = new CommandParser();
            var command = instance.GetCommand(args);

            Assert.IsTrue(command is AddCommand, "InvalidCommand");
        }

        [Test]
        public void GetCommand_ShouldReturn_RemoveCommand_WhenRemoveVerbIsSpecified()
        {
            string[] args = { "remove" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);
            
            Assert.IsTrue(command is RemoveCommand, "Invalid Command");
        }

        [Test]
        public void GetCommand_ShouldReturn_ManageCommand_WhenManageVerbIsSpecified()
        {
            string[] args = { "manage"};
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is ManageCommand, "Invalid Command");
        }

        [Test]
        public void GetCommandShouldReturn_PushCommand_WhenPushVerbIsSpecified()
        {
            string[] args = {"push"};
            var instance = new CommandParser();

            var command = instance.GetCommand(args);
            
            Assert.IsTrue(command is PushCommand, "Invalid Command");
        }

        [Test]
        public void GetCommandShouldReturn_PushCommand_WhenPushVerbIsSpecified_AndAnyArgs()
        {
            //string[] args = { "push", "-a", "origin", "--set-upstream-to=origin/develop"};
            //var instance = new CommandParser();

            //var command = instance.GetCommand(args);

            //Assert.IsTrue(command is PushCommand, "Invalid Command");
        }
    }
}
