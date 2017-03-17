using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Commands;
using GitDepend.Visitors;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Commands
{
    [TestFixture]
    public class RemoveCommandTests : TestFixtureBase
    {
        private IFileSystem _fileSystem;
        private IDependencyVisitorAlgorithm _algorithm;
        private IConsole _console;

        [SetUp]
        public void Setup()
        {
            _fileSystem = RegisterMockFileSystem();
            EnsureDirectory(_fileSystem, Lib1Directory);
            EnsureDirectory(_fileSystem, Lib2Directory);
            _algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
            _console = DependencyInjection.Resolve<IConsole>();
        }


        [Test]
        public void Execute_ShouldSucceed()
        {
            _algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString)).DoInstead(
                (IVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.Success;
                }).MustBeCalled();
            
            RemoveSubOptions options = new RemoveSubOptions()
            {
                Directory = Lib2Directory,
                DependencyName = "lib1"
            };

            var command = new RemoveCommand(options);

            var code = command.Execute();

            Assert.AreEqual(ReturnCode.Success, code);
            _algorithm.Assert("TraverseDependencies should have been called");
        }

        [Test]
        public void Execute_ShouldFail_With_MisnamedDependency()
        {
            _algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString)).DoInstead(
                (IVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.NameDidNotMatchRequestedDependency;
                });
            _console.Arrange(x => x.WriteLine(Arg.AnyString)).MustBeCalled();

            var removeOptions = new RemoveSubOptions();
            var command = new RemoveCommand(removeOptions);

            var code = command.Execute();

            Assert.AreEqual(ReturnCode.NameDidNotMatchRequestedDependency, code);
            _console.Assert("WriteLine should have been called");
        }
    }
}
