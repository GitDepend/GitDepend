using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using GitDepend.Configuration;
using GitDepend.Visitors;
using NUnit.Framework;
using Telerik.JustMock;

namespace GitDepend.UnitTests.Visitors
{
	[TestFixture]
	public class BuildAndUpdateDependenciesVisitorTests : TestFixtureBase
	{
		private IGitDependFileFactory _factory;
		private IGit _git;
		private INuget _nuget;
		private IProcessManager _processManager;
		private MockFileSystem _fileSystem;
		private BuildAndUpdateDependenciesVisitor _instance;

		[SetUp]
		public void SetUp()
		{
			_factory = Mock.Create<IGitDependFileFactory>();
			_git = Mock.Create<IGit>();
			_nuget = Mock.Create<INuget>();
			_processManager = Mock.Create<IProcessManager>();
			_fileSystem = new MockFileSystem();
			_instance = new BuildAndUpdateDependenciesVisitor(_factory, _git, _nuget, _processManager, _fileSystem);
		}

		[Test]
		public void VisitDependency_()
		{
			_instance.VisitDependency(Lib1Dependency);
		}
	}
}
