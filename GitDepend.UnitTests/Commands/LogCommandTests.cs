using GitDepend.CommandLine;
using GitDepend.Commands;
using GitDepend.Visitors;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Commands
{
    [TestFixture]
    public class LogCommandTests : TestFixtureBase
    {
        private IDependencyVisitorAlgorithm _algorithm;

        [SetUp]
        public void Setup()
        {
            _algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
        }

        [Test]
        public void LogCommandSucceeds()
        {
            _algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString)).DoInstead(
                (LogVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.Success;
                });

            var options = new LogSubOptions();
            var command = new LogCommand(options);

            var code = command.Execute();

            Assert.AreEqual(ReturnCode.Success, code);
        }

        [Test]
        public void LogCommandFails_WhenOtherReturnCodeReturned()
        {
            _algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString)).DoInstead(
                (LogVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.InvalidCommand;
                });

            var options = new LogSubOptions();
            var command = new LogCommand(options);

            var code = command.Execute();

            Assert.AreNotEqual(ReturnCode.Success, code);
        }
    }
}
