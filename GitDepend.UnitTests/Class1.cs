using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GitDepend.UnitTests
{
	[TestFixture]
	public class Class1
	{
		[SetUp]
		public void SetUp()
		{
			
		}

		[Test]
		public void SampleTest()
		{
			// Intentionally fail for now until appveyor has been set up to run tests.
			Assert.IsTrue(false);
		}
	}
}
