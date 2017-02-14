using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitDepend.Configuration;
using NUnit.Framework;

namespace GitDepend.UnitTests
{
	[TestFixture]
	public abstract class TestFixtureBase
	{
		protected readonly GitDependFile Lib1Config;
		protected readonly Dependency Lib1Dependency;
		protected readonly GitDependFile Lib2Config;

		protected TestFixtureBase()
		{
			Lib1Config = new GitDependFile
			{
				Build = { Script = "make.bat" },
				Packages = { Directory = "artifacts/NuGet/Debug" }
			};

			Lib1Dependency = new Dependency
			{
				Name = "Lib1",
				Directory = "..\\Lib1",
				Url = "git@github.com:kjjuno/Lib1.git",
				Branch = "develop",
				Configuration = Lib1Config
			};

			Lib2Config = new GitDependFile
			{
				Build = { Script = "make.bat" },
				Packages = { Directory = "artifacts/NuGet/Debug" },
				Dependencies = { Lib1Dependency }
			};
		}

		protected static void AssertThrows<T>(Action del, string exceptionMessage) where T : Exception
		{
			try
			{
				del();
				Assert.Fail("Did not throw the expected exception");
			}
			catch (Exception ex)
			{
				if (!(ex is T && ex.Message == exceptionMessage))
				{
					Assert.Fail("Expected: " + exceptionMessage + Environment.NewLine + "Actual: " + ex.Message);
				}
			}
		}

		protected static async Task AssertThrows<T>(Func<Task> del, string exceptionMessage) where T : Exception
		{
			try
			{
				await del();
				Assert.Fail("Did not throw the expected exception");
			}
			catch (Exception ex)
			{
				if (!(ex is T && ex.Message == exceptionMessage))
				{
					Assert.Fail("Expected: " + exceptionMessage + Environment.NewLine + "Actual: " + ex.Message);
				}
			}
		}

		protected static void AssertBytesAreEqual(byte[] expected, byte[] actual)
		{
			Assert.IsNotNull(actual, "The data should not be null");
			Assert.AreEqual(expected.Length, actual.Length, "The data length does not match the expected length");
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i], "Data mismatch at position " + i);
			}
		}

		protected static void AssertArraysAreEqual<T>(T[] expected, T[] actual)
		{
			Assert.AreEqual(expected.Length, actual.Length, "Incorrect length");
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i], "Data mismatch at position " + i);
			}
		}
	}
}
