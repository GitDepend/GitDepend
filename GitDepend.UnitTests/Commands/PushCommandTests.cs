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
    public class PushCommandTests : TestFixtureBase
    {
        private IDependencyVisitorAlgorithm _algorithm;

        [SetUp]
        public void Setup()
        {
            _algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
        }

        [Test]
        public void PushCommandSucceeds()
        {
            _algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString)).DoInstead(
                (PushBranchVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.Success;
                });
            var options = new PushSubOptions();
            var command = new PushCommand(options);

            var code = command.Execute();

            Assert.AreEqual(ReturnCode.Success, code);

        }

        [Test]
        public void PushCommandFails_WhenOtherReturnCodeReturned()
        {
            _algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString)).DoInstead(
                (PushBranchVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.InvalidCommand;
                });

            var options = new PushSubOptions();
            var command = new PushCommand(options);

            var code = command.Execute();

            Assert.AreNotEqual(ReturnCode.Success, code);
        }
    }
}
