using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;
using NUnit.Framework;

namespace GitDepend.UnitTests.Busi
{
	[TestFixture]
	public class GitDependFileFactoryTests : TestFixtureBase
	{
		[Test]
		public void LoadFromDirectory_ShoulReturn_DirectoryDoesNotExist_WhenSpecifiedDirectoryDoesNotExist()
		{
			var instance = new GitDependFileFactory();

			string directory = @"C:\projects\DoesNotExit";
			string dir;
			ReturnCode code;
			var config = instance.LoadFromDirectory(directory, out dir, out code);

			Assert.IsNull(config, "Config file should be null");
			Assert.IsNull(dir, "The found directory should be null");
			Assert.AreEqual(ReturnCode.DirectoryDoesNotExist, code, "Invalid Return Code");
		}

		[Test]
		public void LoadFromDirectory_ShoulReturn_GitRepositoryNotFound_NoGitDirectoryExists()
		{
			var fileSystem = RegisterMockFileSystem();
			var instance = new GitDependFileFactory();

			string directory = @"C:\projects\GitDepend";
			fileSystem.Directory.CreateDirectory(directory);

			string dir;
			ReturnCode code;
			var config = instance.LoadFromDirectory(directory, out dir, out code);

			Assert.IsNull(config, "Config file should be null");
			Assert.IsNull(dir, "The found directory should be null");
			Assert.AreEqual(ReturnCode.GitRepositoryNotFound, code, "Invalid Return Code");
		}
	}
}
