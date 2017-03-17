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

        [SetUp]
        public void TestSetup()
        {
            _factory = DependencyInjection.Resolve<IGitDependFileFactory>();
        }

        [Test]
        public void RemoveDependencyShouldFindDependency()
        {
            string dir;
            ReturnCode returnCode;
            _factory.Arrange(x => x.LoadFromDirectory(Lib1Directory, out dir, out returnCode)).Returns(Lib1Config);
            //lib2 depends on lib1
            //remove lib1 reference
            var libToRemove = Lib1Config.Name;
            RemoveDependencyVisitor visitor = new RemoveDependencyVisitor(libToRemove);
            
            returnCode = visitor.VisitDependency(Lib1Directory, Lib1Dependency);

            Assert.AreEqual(ReturnCode.Success, returnCode);

            returnCode = visitor.VisitProject(Lib2Directory, Lib2Config);

            Assert.AreEqual(ReturnCode.Success, returnCode);
            Assert.AreEqual(0, Lib2Config.Dependencies.Count);
        }

        [Test]
        public void RemoveDependencyShouldReturnNotFound()
        {

        }
    }
}