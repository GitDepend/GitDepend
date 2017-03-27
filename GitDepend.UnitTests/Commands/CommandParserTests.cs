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
        private const string InvalidCommand = "Invalid Command";

        [Test]
        public void GetCommand_ShouldReturn_InitCommand_WhenInitVerbIsSpecified()
        {
            string[] args = { "init" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is InitCommand, InvalidCommand);
        }

        [Test]
        public void GetCommand_ShouldReturn_ShowConfigCommand_WhenCloneVerbIsSpecified()
        {
            string[] args = { "config" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is ConfigCommand, InvalidCommand);
        }

        [Test]
        public void GetCommand_ShouldReturn_CleanCommand_WhenCleanVerbIsSpecified()
        {
            string[] args = {"clean"};
            var instance = new CommandParser();
            var command = instance.GetCommand(args);

            Assert.IsTrue(command is CleanCommand, InvalidCommand);
        }

        [Test]
        public void GetCommand_ShouldReturn_CloneCommand_WhenCloneVerbIsSpecified()
        {
            string[] args = { "clone" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is CloneCommand, InvalidCommand);
        }

        [Test]
        public void GetCommand_ShouldReturn_UpdateCommand_WhenUpdateVerbIsSpecified()
        {
            string[] args = { "update" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is UpdateCommand, InvalidCommand);
        }

        [Test]
        public void GetCommand_ShouldReturn_StatusCommand_WhenStatusVerbIsSpecified()
        {
            string[] args = { "status", "lib1" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is StatusCommand, InvalidCommand);
        }

        [Test]
        public void GetCommand_ShouldReturn_ListCommand_WhenListVerbIsSpecified()
        {
            string[] args = { "list" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is ListCommand, InvalidCommand);
        }

        [Test]
        public void GetCommand_ShouldReturn_BranchCommand_WhenBranchVerbIsSpecified()
        {
            string[] args = { "branch" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is BranchCommand, InvalidCommand);
        }

        [Test]
        public void GetCommand_ShouldReturn_CheckOutCommand_WhenCheckOutVerbIsSpecified()
        {
            string[] args = { "checkout", "my_branch" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is CheckOutCommand, InvalidCommand);
        }

        [Test]
        public void GetCommand_ShouldReturn_SyncCommand_WhenCheckOutVerbIsSpecified()
        {
            string[] args = { "sync" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is SyncCommand, InvalidCommand);
        }

        [Test]
        public void GetCommand_ShouldReturn_AddCommand_WhenAddVerbIsSpecified()
        {
            string[] args = {"add", "--url", "myurl",  "--directory", "mydir", "--branch", "mybranch"};
            var instance = new CommandParser();
            var command = instance.GetCommand(args);

            Assert.IsTrue(command is AddCommand, InvalidCommand);
        }

        [Test]
        public void GetCommand_ShouldReturn_RemoveCommand_WhenRemoveVerbIsSpecified()
        {
            string[] args = { "remove" };
            var instance = new CommandParser();

            var command = instance.GetCommand(args);
            
            Assert.IsTrue(command is RemoveCommand, InvalidCommand);
        }

        [Test]
        public void GetCommand_ShouldReturn_ManageCommand_WhenManageVerbIsSpecified()
        {
            string[] args = { "manage"};
            var instance = new CommandParser();

            var command = instance.GetCommand(args);

            Assert.IsTrue(command is ManageCommand, InvalidCommand);
        }

        [Test]
        public void GetCommand_ShouldReturn_PullCommand_WhenPullVerbIsSpecified()
        {
            string[] args = {"pull"};
            var instance = new CommandParser();
            var command = instance.GetCommand(args);

            Assert.IsTrue(command is PullCommand, InvalidCommand);
        }
    }
}
