using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [SetUp]
        public void Setup()
        {
            _fileSystem = RegisterMockFileSystem();
            EnsureDirectory(_fileSystem, Lib1Directory);
            EnsureDirectory(_fileSystem, Lib2Directory);

        }


        [Test]
        public void Execute_ShouldSucceed()
        {
            
            var algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
            algorithm.Arrange(x => x.TraverseDependencies(Arg.IsAny<IVisitor>(), Arg.AnyString)).DoInstead(
                (IVisitor visitor, string directory) =>
                {
                }).Returns(ReturnCode.Success);
            
            RemoveSubOptions options = new RemoveSubOptions()
            {
                Directory = Lib2Directory,
                DependencyName = "lib1"
            };

            var command = new RemoveCommand(options);

            var code = command.Execute();

            Assert.AreEqual(ReturnCode.Success, code);
            Assert.IsFalse(_fileSystem.Directory.Exists(Lib1Directory));
            
        }

        [Test]
        public void Execute_ShouldFail_With_MisnamedDependency()
        {
            
        }
    }
}
