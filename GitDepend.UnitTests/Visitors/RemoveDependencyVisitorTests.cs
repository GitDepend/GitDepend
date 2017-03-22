using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Commands;
using GitDepend.Configuration;
using GitDepend.Visitors;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Visitors
{
    [TestFixture]
    public class RemoveDependencyVisitorTests : TestFixtureBase
    {
        private IGitDependFileFactory _factory;
        private IFileSystem _fileSystem;

        [SetUp]
        public void TestSetup()
        {
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
        }

        [Test]
        public void RemoveDependencyShouldFindDependency()
        {
            string dir;
            ReturnCode returnCode;
            _factory.Arrange(x => x.LoadFromDirectory(Arg.AnyString, out dir, out returnCode)).Returns(Lib1Config);
            _fileSystem.Arrange(x => x.Path.Combine(Arg.AnyString, Arg.AnyString)).Returns("C:\\projects\\Lib1");
            _fileSystem.Arrange(x => x.Path.GetFullPath(Arg.AnyString)).Returns("C:\\projects\\Lib1");

            var libToRemove = Lib1Config.Name;
            var libsToRemove = new List<string>()
            {
                libToRemove
            };
            RemoveDependencyVisitor visitor = new RemoveDependencyVisitor(libsToRemove);

            returnCode = visitor.VisitDependency(Lib1Directory, Lib1Dependency);

            Assert.AreEqual(ReturnCode.Success, returnCode);

            returnCode = visitor.VisitProject(Lib2Directory, Lib2Config);

            Assert.AreEqual(ReturnCode.Success, returnCode);
            Assert.AreEqual(1, visitor.FoundDependencyDirectories.Count);
        }

        [Test]
        public void RemoveDependencyShouldReturnSuccess_EvenWhenNotFound()
        {
            string dir;
            ReturnCode returnCode;
            _factory.Arrange(x => x.LoadFromDirectory(Lib1Directory, out dir, out returnCode)).Returns(Lib1Config);
            List<string> libsToRemove = new List<string> { "lib3" };
            RemoveDependencyVisitor visitor = new RemoveDependencyVisitor(libsToRemove);

            returnCode = visitor.VisitDependency(Lib1Directory, Lib1Dependency);
            Assert.AreEqual(ReturnCode.Success, returnCode);

            returnCode = visitor.VisitProject(Lib2Directory, Lib2Config);

            Assert.AreEqual(ReturnCode.Success, returnCode);
        }
    }
}