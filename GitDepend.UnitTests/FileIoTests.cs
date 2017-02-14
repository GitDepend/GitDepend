using System;
using System.IO;
using System.Linq;
using System.Threading;
using GitDepend.Busi;
using NUnit.Framework;

namespace GitDepend.UnitTests
{
	[TestFixture]
	// ReSharper disable once InconsistentNaming
	public class FileIOTests : TestFixtureBase
	{
		private FileIo _fileIo;

		[SetUp]
		public void SetUp()
		{
			_fileIo = new FileIo();
		}

		[Test]
		public void OpenFileStreamReadTest()
		{
			const string FILENAME = ".readtest";

			EnsureFileDoesNotExist(FILENAME);

			using (var fs = _fileIo.OpenFileStreamRead(FILENAME))
			{
				Assert.IsFalse(File.Exists(FILENAME));
				Assert.IsNull(fs);
			}

			Touch(FILENAME);

			HoldFile(FILENAME);

			using (var fs = _fileIo.OpenFileStreamRead(FILENAME))
			{
				Assert.IsTrue(File.Exists(FILENAME));
				Assert.IsNotNull(fs);
				Assert.AreEqual(0, fs.Length);
			}

			EnsureFileDoesNotExist(FILENAME);
		}

		private void Touch(string filename)
		{
			using (var fs = _fileIo.OpenFileStreamWrite(filename, FileMode.OpenOrCreate))
			{
				fs.Close();
			}
		}

		[Test]
		public void OpenFileStreamWriteTest()
		{
			const string FILENAME = ".writetest";

			EnsureFileDoesNotExist(FILENAME);

			using (var fs = _fileIo.OpenFileStreamWrite(FILENAME))
			{
				Assert.IsTrue(File.Exists(FILENAME));
				Assert.IsNotNull(fs);
				Assert.AreEqual(0, fs.Length);
			}

			HoldFile(FILENAME);

			using (var fs = _fileIo.OpenFileStreamWrite(FILENAME))
			{
				Assert.IsTrue(File.Exists(FILENAME));
				Assert.IsNotNull(fs);
				Assert.AreEqual(0, fs.Length);
			}

			EnsureFileDoesNotExist(FILENAME);
		}

		[Test]
		public void GetFileLengthTest()
		{
			const string FILENAME = ".filelength";

			EnsureFileDoesNotExist(FILENAME);

			var bytes = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };
			File.WriteAllBytes(FILENAME, bytes);

			var length = _fileIo.GetFileLength(FILENAME);
			Assert.AreEqual(bytes.Length, length);

			HoldFile(FILENAME);
			length = _fileIo.GetFileLength(FILENAME);
			Assert.AreEqual(bytes.Length, length);

			Thread.Sleep(200);
			EnsureFileDoesNotExist(FILENAME);

			length = _fileIo.GetFileLength(FILENAME);
			Assert.AreEqual(0, length);
		}

		[Test]
		public void DeleteDirectoryTest()
		{
			const string DIR = "unit_test_dir";

			CreateTestDirectoryStructure(DIR);

			Assert.IsTrue(Directory.Exists(DIR));
			Assert.AreEqual(3, Directory.GetDirectories(DIR).Length);
			Assert.AreEqual(4, Directory.GetFiles(DIR).Length);

			_fileIo.DeleteDirectory(DIR, true);

			Assert.IsFalse(Directory.Exists(DIR));

			CreateTestDirectoryStructure(DIR);

			HoldFile(Path.Combine(DIR, "file1"));

			_fileIo.DeleteDirectory(DIR, true);

			Assert.IsFalse(Directory.Exists(DIR));
		}

		[Test]
		public void GetLastWriteTimeUtcTest()
		{
			const string FILENAME = ".lastwrite";

			EnsureFileDoesNotExist(FILENAME);

			var time = _fileIo.GetLastWriteTimeUtc(FILENAME);
			var expected = new DateTime(1601, 1, 1);
			Assert.AreEqual(expected, time, "Incorrect date when the file doesn't exist");

			File.WriteAllText(FILENAME, "Last Write Test");

			time = _fileIo.GetLastWriteTimeUtc(FILENAME);

			var now = DateTime.UtcNow;
			Assert.IsTrue(now - time < TimeSpan.FromMilliseconds(100));

			HoldFile(FILENAME);

			time = _fileIo.GetLastWriteTimeUtc(FILENAME);
			Assert.IsTrue(now - time < TimeSpan.FromMilliseconds(100));

			Thread.Sleep(200);
			EnsureFileDoesNotExist(FILENAME);

			var paths = Enumerable.Repeat("SubDir", 1000).ToArray();

			var tooLong = Path.Combine(@"\\unit_tests\buckets\some_bucket", Path.Combine(paths));

			time = _fileIo.GetLastWriteTimeUtc(tooLong);

			Assert.AreEqual(new DateTime(), time, "The time should have failed because the path is too long");
		}

		[Test]
		public void CreateDirectoryTest()
		{
			const string DIR = "unit_tests";

			if (Directory.Exists(DIR))
			{
				Directory.Delete(DIR);
			}

			Assert.IsFalse(Directory.Exists(DIR), "The directory should not exist");

			_fileIo.CreateDirectory(DIR);

			Assert.IsTrue(Directory.Exists(DIR), "The directory should exist");

			Directory.Delete(DIR);
		}

		[Test]
		public void DirectoryExistsTest()
		{
			const string DIR = "unit_tests";

			if (Directory.Exists(DIR))
			{
				Directory.Delete(DIR);
			}

			Assert.IsFalse(_fileIo.DirectoryExists(DIR), "The directory should not exist");

			Directory.CreateDirectory(DIR);

			Assert.IsTrue(_fileIo.DirectoryExists(DIR), "The directory should exist");

			Directory.Delete(DIR);
		}

		[Test]
		public void MoveFileTest()
		{
			const string SOURCE = ".source";
			const string DESTINATION = ".destination";

			EnsureFileDoesNotExist(SOURCE);
			EnsureFileDoesNotExist(DESTINATION);

			const string TEXT = "Peter Piper picked a peck of pickled peppers";
			File.WriteAllText(SOURCE, TEXT);

			Assert.IsTrue(File.Exists(SOURCE), "Source file should exist");
			Assert.IsFalse(File.Exists(DESTINATION), "The destination file should not exist");

			_fileIo.MoveFile(SOURCE, DESTINATION);

			Assert.IsFalse(File.Exists(SOURCE), "Source file should not exist");
			Assert.IsTrue(File.Exists(DESTINATION), "The destination file should exist");

			var contents = File.ReadAllText(DESTINATION);

			Assert.AreEqual(TEXT, contents, "File contents mismatch");
		}

		[Test]
		public void DeleteFileTest()
		{
			const string FILENAME = ".delete_me";

			EnsureFileDoesNotExist(FILENAME);

			File.WriteAllText(FILENAME, "This file should get deleted");

			Assert.IsTrue(File.Exists(FILENAME), "The file should exist");

			_fileIo.DeleteFile(FILENAME);

			Assert.IsFalse(File.Exists(FILENAME), "The file should not exist");
		}

		[Test]
		public void GetFilesTest()
		{
			const string DIR = "get_files_test";

			if (!Directory.Exists(DIR))
			{
				Directory.CreateDirectory(DIR);
			}

			string[] files = { "img1.png", "img2.png", "report.pdf", "main.c" };

			foreach (var file in files)
			{
				File.WriteAllText(Path.Combine(DIR, file), file);
			}

			var expected = files.Select(f => Path.Combine(DIR, f)).OrderBy(f => f).ToArray();
			var actual = _fileIo.GetFiles(DIR, "*").OrderBy(f => f).ToArray();

			AssertArraysAreEqual(expected, actual);

			expected = files.Where(f => Path.GetExtension(f) == ".png").Select(f => Path.Combine(DIR, f)).OrderBy(f => f).ToArray();
			actual = _fileIo.GetFiles(DIR, "*.png").OrderBy(f => f).ToArray();

			AssertArraysAreEqual(expected, actual);

			expected = files.Where(f => Path.GetExtension(f) == ".pdf").Select(f => Path.Combine(DIR, f)).OrderBy(f => f).ToArray();
			actual = _fileIo.GetFiles(DIR, "*.pdf").OrderBy(f => f).ToArray();

			AssertArraysAreEqual(expected, actual);

			expected = files.Where(f => Path.GetExtension(f) == ".c").Select(f => Path.Combine(DIR, f)).OrderBy(f => f).ToArray();
			actual = _fileIo.GetFiles(DIR, "*.c").OrderBy(f => f).ToArray();

			AssertArraysAreEqual(expected, actual);

			Directory.Delete(DIR, true);
		}

		private static void HoldFile(string filename)
		{
			var holder = new FileStream(filename, FileMode.Open, FileAccess.Write, FileShare.None);

			ThreadPool.QueueUserWorkItem(obj =>
			{
				Thread.Sleep(100);
				holder.Close();
				holder.Dispose();
			});
		}

		private static void CreateTestDirectoryStructure(string dir)
		{
			Directory.CreateDirectory(dir);

			File.WriteAllBytes(Path.Combine(dir, "file1"), new byte[] { 0x01, 0x02 });
			File.WriteAllBytes(Path.Combine(dir, "file2"), new byte[] { 0x01, 0x02, 0x03 });
			File.WriteAllBytes(Path.Combine(dir, "file3"), new byte[] { 0x01, 0x02, 0x03, 0x04 });
			File.WriteAllBytes(Path.Combine(dir, "file4"), new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 });

			Directory.CreateDirectory(Path.Combine(dir, "subdir1"));
			Directory.CreateDirectory(Path.Combine(dir, "subdir2"));
			Directory.CreateDirectory(Path.Combine(dir, "subdir3"));
		}

		private static void EnsureFileDoesNotExist(string filename)
		{
			if (File.Exists(filename))
			{
				File.Delete(filename);
			}
		}
	}
}
