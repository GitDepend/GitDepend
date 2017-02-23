using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Configuration;
using NUnit.Framework;

namespace GitDepend.UnitTests.Configuration
{
    [TestFixture]
    public class DependencyTests : TestFixtureBase
    {
        [Test]
        public void DefaultValuesTest()
        {
            var dependency = new Dependency();

            Assert.AreEqual(null, dependency.Url);
            Assert.AreEqual(null, dependency.Directory);
            Assert.AreEqual(null, dependency.Branch);
        }

        [Test]
        public void ToStringTest()
        {
            var expected = "{\r\n  \"url\": \"git@github.com:kjjuno/Lib1.git\",\r\n  \"dir\": \"..\\\\Lib1\",\r\n  \"branch\": \"develop\"\r\n}";
            var actual = Lib1Dependency.ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}
