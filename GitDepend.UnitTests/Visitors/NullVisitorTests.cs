using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Visitors;
using NUnit.Framework;

namespace GitDepend.UnitTests.Visitors
{
    [TestFixture]
    public class NullVisitorTests : TestFixtureBase
    {
        [Test]
        public void VisitDependency_ShouldReturn_Success()
        {
            var instance = new NullVisitor();
            var code = instance.VisitDependency(null, Lib1Dependency);
            Assert.AreEqual(ReturnCode.Success, code, "Invalid ReturnCode");
        }

        [Test]
        public void VisitProject_ShouldReturn_Success()
        {
            var instance = new NullVisitor();
            var code = instance.VisitProject(null, Lib1Config);
            Assert.AreEqual(ReturnCode.Success, code, "Invalid ReturnCode");
        }
    }
}
