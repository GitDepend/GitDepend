using System.Collections.Generic;
using GitDepend.CommandLine;
using GitDepend.Commands;
using GitDepend.Visitors;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Commands
{
    [TestFixture]
    public class IncludeCommandTests : TestFixtureBase
    {
        private IDependencyVisitorAlgorithm _algorithm;

        [SetUp]
        public void Setup()
        {
            _algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
        }

        [Test]
        public void IncludeCommandSucceeds()
        {
            _algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString, true)).DoInstead(
                (IncludeVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.Success;
                });

            var options = new IncludeSubOptions();
            var command = new IncludeCommand(options);

            var code = command.Execute();

            Assert.AreEqual(ReturnCode.Success, code);
        }

        [Test]
        public void IncludeCommandFails_WhenOtherReturnCodeReturned()
        {
            _algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString, true)).DoInstead(
                (IncludeVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.InvalidCommand;
                });

            var options = new IncludeSubOptions();
            var command = new IncludeCommand(options);

            var code = command.Execute();

            Assert.AreNotEqual(ReturnCode.Success, code);
        }
    }
}
