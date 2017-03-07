using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Configuration;
using GitDepend.Visitors;
using NUnit.Framework;

namespace GitDepend.UnitTests.Visitors
{
    [TestFixture]
    public class RemoveDependencyVisitorTests : TestFixtureBase
    {
        private RemoveDependencyVisitor _visitor;

        [SetUp]
        public void TestSetup()
        {
            _visitor = new RemoveDependencyVisitor("lib1");
        }

        [Test]
        public void RemoveDependencyShouldFindDependency()
        {
            var result = _visitor.VisitDependency("lib1", new Dependency()
            {
                Branch = "",
                Directory = "../lib1",
                Url = "",
                Configuration = new GitDependFile()
                {
                    Dependencies =
                    {
                        new Dependency
                        {
                            Configuration = new GitDependFile()
                            {
                                Name = "lib1"
                            }
                        }
                    },
                    Name = "lib2",
                }
            });

            Assert.AreEqual(ReturnCode.Success, result);
        }

        [Test]
        public void RemoveDependencyShouldReturnNotFound()
        {

        }
    }
}