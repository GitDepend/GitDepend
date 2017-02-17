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
            var expected = @"{
  ""build"": {
    ""script"": ""make.bat"",
    ""arguments"": null
  },
  ""packages"": {
    ""dir"": ""artifacts/NuGet/Debug""
  },
  ""dependencies"": [
    {
      ""name"": ""Lib1"",
      ""url"": ""git@github.com:kjjuno/Lib1.git"",
      ""dir"": ""..\\Lib1"",
      ""branch"": ""develop""
    }
  ]
}";
            var actual = Lib2Config.ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}
