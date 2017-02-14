using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
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
		private IFileIo _fileIo;

		[SetUp]
		public void SetUp()
		{
			_fileIo = Mock.Create<IFileIo>();
			_instance = new DependencyVisitorAlgorithm(_fileIo);
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
