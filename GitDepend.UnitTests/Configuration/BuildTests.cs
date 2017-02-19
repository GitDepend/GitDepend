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
    public class BuildTests
    {
        [Test]
        public void DefaultValuesTest()
        {
            var build = new Build();

            Assert.AreEqual("make.bat", build.Script);
            Assert.AreEqual(null, build.Arguments);
        }

        [Test]
        public void ToStringTest()
        {
            var build = new Build
            {
                Script = "buildall.bat",
                Arguments = "cov"
            };

            var expected = "{\r\n  \"script\": \"buildall.bat\",\r\n  \"arguments\": \"cov\"\r\n}";
            var actual = build.ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}
