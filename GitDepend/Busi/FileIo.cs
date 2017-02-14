using System;
using System.IO;
using System.Threading;

namespace GitDepend.Busi
{
	/// <summary>
	/// A static class for helping with File IO.
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public class FileIo : IFileIo
	{
		#region Fields

		private const int TRIES = 10;
		private const int SLEEP = 1000;

		#endregion

		/// <summary>
		/// Gets a FileStream with <see cref="FileAccess.Read"/> and <see cref="FileShare.Read"/>.
		/// If the file cannot be accessed initially this call will block until we can either successfully
		/// get a handle to the file, or we give up.
		/// </summary>
		/// <param name="fileName">The path to the file.</param>
		/// <param name="mode">The <see cref="FileMode"/> to use.</param>
		/// <param name="share">The <see cref="FileShare"/> sharing mode to use.</param>
		/// <returns>A FileStream with <see cref="FileAccess.Read"/> and <see cref="FileShare.Read"/>, or null if the call failed.</returns>
		public Stream OpenFileStreamRead(string fileName, FileMode mode = FileMode.Open, FileShare share = FileShare.Read)
		{
			int tries = TRIES;
			FileStream fs = null;

			while (tries-- > 0)
			{
				try
				{
					fs = File.Open(fileName, mode, FileAccess.Read, share);
					break;
				}
				// ReSharper disable EmptyGeneralCatchClause
				catch (Exception)
				// ReSharper restore EmptyGeneralCatchClause
				{
					if (!File.Exists(fileName))
					{
						break;
					}
				}
				Thread.Sleep(SLEEP);
			}

			return fs;
		}

		/// <summary>
		/// Gets a FileStream with <see cref="FileAccess.Write"/> and <see cref="FileShare.None"/>.
		/// If the file cannot be accessed initially this call will block until we can either successfully
		/// get a handle to the file, or we give up.
		/// </summary>
		/// <param name="fileName">The path to the file.</param>
		/// <param name="mode">The <see cref="FileMode"/> to use.</param>
		/// <param name="share">The <see cref="FileShare"/> sharing mode to use.</param>
		/// <returns>A FileStream with <see cref="FileAccess.Write"/> and <see cref="FileShare.None"/>, or null if the call failed.</returns>
		public Stream OpenFileStreamWrite(string fileName, FileMode mode = FileMode.Create, FileShare share = FileShare.None)
		{
			int tries = TRIES;
			FileStream fs = null;

			while (tries-- > 0)
			{
				try
				{
					fs = File.Open(fileName, mode, FileAccess.Write, share);
					break;
				}
				// ReSharper disable EmptyGeneralCatchClause
				catch (Exception) { }
				// ReSharper restore EmptyGeneralCatchClause
				Thread.Sleep(SLEEP);
			}

			return fs;
		}

		/// <summary>
		/// Gets the length of a file.
		/// If the file cannot be accessed initially this call will block until we can either successfully
		/// get a handle to the file, or we give up.
		/// </summary>
		/// <param name="fileName">The path to the file.</param>
		/// <returns>The length of the file, or 0 if the call was unsuccessful.</returns>
		public long GetFileLength(string fileName)
		{
			long length = 0;

			try
			{
				length = new FileInfo(fileName).Length;
			}
			// ReSharper disable EmptyGeneralCatchClause
			catch (Exception) { }
			// ReSharper restore EmptyGeneralCatchClause

			return length;
		}

		/// <summary>
		/// Deletes a directory.
		/// If the directory cannot be accessed initially this call will block until we can either successfully
		/// delete the directory, or we give up.
		/// </summary>
		/// <param name="dir">The path to the directory</param>
		/// <param name="recursive">Is the delete recursive or not?</param>
		public void DeleteDirectory(string dir, bool recursive)
		{
			int tries = TRIES;

			while (tries-- > 0)
			{
				try
				{
					if (DirectoryExists(dir))
					{
						Directory.Delete(dir, recursive);
					}
					break;
				}
				// ReSharper disable EmptyGeneralCatchClause
				catch (Exception) { }
				// ReSharper restore EmptyGeneralCatchClause

				Thread.Sleep(SLEEP);
			}
		}

		/// <summary>
		/// Creates all directories and subdirectories in the specified path.
		/// </summary>
		/// <param name="dir">The directory path to create.</param>
		public void CreateDirectory(string dir)
		{
			Directory.CreateDirectory(dir);
		}

		/// <summary>
		/// Gets the LastWriteTimeUtc for a file.
		/// If the directory cannot be accessed initially this call will block until we can either successfully
		/// get the LastWriteTimeUtc, or we give up.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public DateTime GetLastWriteTimeUtc(string fileName)
		{
			DateTime lastWrite = new DateTime();

			try
			{
				lastWrite = File.GetLastWriteTimeUtc(fileName);
			}
			// ReSharper disable EmptyGeneralCatchClause
			catch (Exception) { }
			// ReSharper restore EmptyGeneralCatchClause

			return lastWrite;
		}

		/// <summary>
		/// Determines whether the given path refers to an existing directory on disk.
		/// </summary>
		/// <param name="dir">The path to test.</param>
		/// <returns>True if the directory exists, and false if it does not.</returns>
		public bool DirectoryExists(string dir)
		{
			return Directory.Exists(dir);
		}

		/// <summary>
		/// Determines whether the specified file exists.
		/// </summary>
		/// <param name="path">The file to check.</param>
		/// <returns>True if the file exists, and false if it does not.</returns>
		public bool FileExists(string path)
		{
			return File.Exists(path);
		}

		/// <summary>
		/// Moves a specified file to a new location, providing the option to specify a new file name.
		/// </summary>
		/// <param name="sourceFileName">The name of the file to move.</param>
		/// <param name="destFileName">The new path for the file.</param>
		public void MoveFile(string sourceFileName, string destFileName)
		{
			if (FileExists(destFileName))
			{
				DeleteFile(destFileName);
			}

			File.Move(sourceFileName, destFileName);
		}

		/// <summary>
		/// Deletes the specified file.
		/// </summary>
		/// <param name="path">The name of the file to be deleted. Wildcard characters are not supported.</param>
		public void DeleteFile(string path)
		{
			File.Delete(path);
		}

		/// <summary>
		/// Returns the names of files (including their paths) that match the specified search pattern in the specified directory.
		/// </summary>
		/// <param name="path">The directory to search.</param>
		/// <param name="searchPattern">
		/// The search string to match against the names of files in <paramref name="path"/>.
		/// The parameter cannot end in two periods ("..") or contain two periods ("..") followed by
		/// <see cref="Path.DirectorySeparatorChar"/> or <see cref="Path.AltDirectorySeparatorChar"/>, nor
		/// can it contain any of the characters in Path.InvalidPathChars/>.
		/// </param>
		/// <param name="searchOption">
		/// One of the enumeration values that specifies whether the search operation should include all subdirectories or only the current directory.
		/// </param>
		/// <returns>The names of files (including their paths) that match the specified search pattern in the specified directory.</returns>
		public string[] GetFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			return Directory.GetFiles(path, searchPattern, searchOption);
		}

		/// <summary>
		/// Returns the name of subdirectories (including their paths) that match the specified search pattern in the specified directory.
		/// </summary>
		/// <param name="path">The relative or absolute path to the directory to search. The string is not case-sensitive.</param>
		/// <param name="searchPattern">
		/// The search string to match against the names of subdirectories in <paramref name="path"/>.
		/// This parameter can contain a combination of literal and wildcard characters, but doesn't support regular expressions.
		/// </param>
		/// <param name="searchOption">
		/// One of the enumeration values that specifies whether the search operation should include all subdirectories or only the current directory.
		/// </param>
		/// <returns>The name of subdirectories (including their paths) that match the specified search pattern in the specified directory.</returns>
		public string[] GetDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			return Directory.GetDirectories(path, searchPattern, searchOption);
		}

		/// <summary>
		/// Retrives the parent directory of the specified path, including both absolute and relative paths.
		/// </summary>
		/// <param name="path">The path for which to retrieve the parent directory.</param>
		/// <returns>The parent directory as a <see cref="DirectoryInfo"/> or null if none exists.</returns>
		public DirectoryInfo GetParentDirectory(string path)
		{
			return Directory.GetParent(path);
		}

		/// <summary>
		/// Copies an existing file to a new file. Overwriting a file of the same name is allowed.
		/// </summary>
		/// <param name="sourceFileName">The file to copy.</param>
		/// <param name="destFileName">The name of the destination file. This cannot be a directory.</param>
		/// <param name="overwrite">true if the destination file can be overwrittin; otherwise false.</param>
		public void Copy(string sourceFileName, string destFileName, bool overwrite)
		{
			File.Copy(sourceFileName, destFileName, overwrite);
		}
	}
}