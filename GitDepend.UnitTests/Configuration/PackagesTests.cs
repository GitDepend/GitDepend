using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Configuration;
using NUnit.Framework;

namespace GitDepend.UnitTests.Configuration
{
	public class PackagesTests
	{
		[Test]
		public void DefaultValuesTest()
		{
			var packages = new Packages();

			Assert.AreEqual("artifacts/NuGet/Debug", packages.Directory);
		}

		[Test]
		public void ToStringTest()
		{
			var build = new Packages()
			{
				Directory = "NuGet"
			};

			var expected = @"{
  ""dir"": ""NuGet""
}";
			var actual = build.ToString();
			Assert.AreEqual(expected, actual);
		}
	}
}
