using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Visitors;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests
{
	[TestFixture]
	public class DependencyVisitorAlgorithmTests
	{
		private DependencyVisitorAlgorithm _instance;

		[SetUp]
		public void SetUp()
		{
			_instance = new DependencyVisitorAlgorithm();
		}

		[Test]
		public void TraverseDependencies_ShouldNotThrow_WithNullVisitorAndNullDirectory()
		{
			_instance.TraverseDependencies(null, null);
		}

		[Test]
		public void TraverseDependencies_ShouldNotThrow_WithNullDirectory()
		{
			var visitor = Mock.Create<IVisitor>();
			
			_instance.TraverseDependencies(visitor, null);
		}

		[Test]
		public void TraverseDependencies_ShouldNotThrow_WithNullVisitor()
		{
			_instance.TraverseDependencies(null, @"C:\testdir");
		}
	}
}
