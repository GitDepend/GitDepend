using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Configuration;
using NUnit.Framework;

namespace GitDepend.UnitTests.Configuration
{
    public class GitDependFileTests : TestFixtureBase
    {
        [Test]
        public void DefaultValuesTest()
        {
            var file = new GitDependFile();
            var build = new Build();
            var packages = new Packages();

            Assert.IsNotNull(file.Build);
            Assert.AreEqual(build.ToString(), file.Build.ToString());

            Assert.IsNotNull(file.Packages);
            Assert.AreEqual(packages.ToString(), file.Packages.ToString());

            Assert.IsNotNull(file.Dependencies);
            Assert.IsEmpty(file.Dependencies);
        }

        [Test]
        public void ToStringTest()
        {
            var expected = "{\r\n" +
                           "  \"build\": {\r\n" +
                           "    \"script\": \"make.bat\",\r\n" +
                           "    \"arguments\": null\r\n" +
                           "  },\r\n" +
                           "  \"packages\": {\r\n" +
                           "    \"dir\": \"artifacts/NuGet/Debug\"\r\n" +
                           "  },\r\n" +
                           "  \"dependencies\": [\r\n" +
                           "    {\r\n" +
                           "      \"name\": \"Lib1\",\r\n" +
                           "      \"url\": \"git@github.com:kjjuno/Lib1.git\",\r\n" +
                           "      \"dir\": \"..\\\\Lib1\",\r\n" +
                           "      \"branch\": \"develop\"\r\n" +
                           "    }\r\n" +
                           "  ]\r\n" +
                           "}";
  
            var actual = Lib2Config.ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}
