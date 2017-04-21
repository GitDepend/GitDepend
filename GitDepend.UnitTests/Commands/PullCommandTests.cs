using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.CommandLine;
using GitDepend.Commands;
using GitDepend.Visitors;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Commands
{
    [TestFixture]
    public class PullCommandTests : TestFixtureBase
    {
        private IDependencyVisitorAlgorithm _algorithm;

        [SetUp]
        public void Setup()
        {
            _algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
        }

        [Test]
        public void PullCommandSucceeds()
        {
            _algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString, false)).DoInstead(
                (PullBranchVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.Success;
                });

            var options = new PullSubOptions();
            var command = new PullCommand(options);

            var code = command.Execute();

            Assert.AreEqual(ReturnCode.Success, code);
        }

        [Test]
        public void PullCommandFails_WhenOtherReturnCodeReturned()
        {
            _algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString, false)).DoInstead(
                (PullBranchVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.InvalidCommand;
                });

            var options = new PullSubOptions();
            var command = new PullCommand(options);

            var code = command.Execute();

            Assert.AreNotEqual(ReturnCode.Success, code);
        }
    }
}
