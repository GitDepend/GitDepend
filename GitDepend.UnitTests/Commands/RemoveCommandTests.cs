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
        private IGitDependFileFactory _factory;

        [SetUp]
        public void Setup()
        {
            _fileSystem = RegisterMockFileSystem();
            EnsureDirectory(_fileSystem, Lib1Directory);
            EnsureDirectory(_fileSystem, Lib2Directory);
            _algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
            _console = DependencyInjection.Resolve<IConsole>();
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
        }


        [Test]
        public void Execute_ShouldSucceed()
        {
            string dir;
            ReturnCode returnCode;
            _algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString)).DoInstead(
                (RemoveDependencyVisitor visitor, string directory) =>
                {
                    visitor.FoundDependencyDirectory = "C:\\projects\\Lib1";
                    visitor.ReturnCode = ReturnCode.Success;
                }).MustBeCalled();
            _factory.Arrange(x => x.LoadFromDirectory(Arg.AnyString, out dir, out returnCode)).Returns(Lib2Config);

            RemoveSubOptions options = new RemoveSubOptions()
            {
                Directory = Lib2Directory,
                Dependencies = new List<string> (){
                    "lib1"},
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
                (RemoveDependencyVisitor visitor, string directory) =>
                {
                    visitor.ReturnCode = ReturnCode.NameDidNotMatchRequestedDependency;
                });
            _console.Arrange(x => x.WriteLine(Arg.AnyString)).MustBeCalled();

            var removeOptions = new RemoveSubOptions()
            {
                Dependencies = new List<string>()
            };
            var command = new RemoveCommand(removeOptions);

            var code = command.Execute();

            Assert.AreEqual(ReturnCode.NameDidNotMatchRequestedDependency, code);
        }
    }
}
