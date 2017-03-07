using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var algorithm = Container.Resolve<IDependencyVisitorAlgorithm>();
            algorithm.Arrange(x => x.TraverseDependencies(new RemoveDependencyVisitor("Lib1"), Lib2Directory));
        }

        [Test]
        public void Execute_ShouldFail_With_MisnamedDependency()
        {
            
        }
    }
}
