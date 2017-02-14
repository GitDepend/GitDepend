using System;
using System.IO;

namespace GitDepend.Busi
{
	/// <summary>
	/// A class for helping with File IO.
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public interface IFileIo
	{
		/// <summary>
		/// Gets a FileStream with <see cref="FileAccess.Read"/> and <see cref="FileShare.Read"/>.
		/// If the file cannot be accessed initially this call will block until we can either successfully
		/// get a handle to the file, or we give up.
		/// </summary>
		/// <param name="fileName">The path to the file.</param>
		/// <param name="mode">The <see cref="FileMode"/> to use.</param>
		/// <param name="share">The <see cref="FileShare"/> sharing mode to use.</param>
		/// <returns>A FileStream with <see cref="FileAccess.Read"/> and <see cref="FileShare.Read"/>, or null if the call failed.</returns>
		Stream OpenFileStreamRead(string fileName, FileMode mode = FileMode.Open, FileShare share = FileShare.Read);

		/// <summary>
		/// Gets a FileStream with <see cref="FileAccess.Write"/> and <see cref="FileShare.None"/>.
		/// If the file cannot be accessed initially this call will block until we can either successfully
		/// get a handle to the file, or we give up.
		/// </summary>
		/// <param name="fileName">The path to the file.</param>
		/// <param name="mode">The <see cref="FileMode"/> to use.</param>
		/// <param name="share">The <see cref="FileShare"/> sharing mode to use.</param>
		/// <returns>A FileStream with <see cref="FileAccess.Write"/> and <see cref="FileShare.None"/>, or null if the call failed.</returns>
		Stream OpenFileStreamWrite(string fileName, FileMode mode = FileMode.Create, FileShare share = FileShare.None);

		/// <summary>
		/// Gets the length of a file.
		/// If the file cannot be accessed initially this call will block until we can either successfully
		/// get a handle to the file, or we give up.
		/// </summary>
		/// <param name="fileName">The path to the file.</param>
		/// <returns>The length of the file, or 0 if the call was unsuccessful.</returns>
		long GetFileLength(string fileName);

		/// <summary>
		/// Deletes a directory.
		/// If the directory cannot be accessed initially this call will block until we can either successfully
		/// delete the directory, or we give up.
		/// </summary>
		/// <param name="dir">The path to the directory</param>
		/// <param name="recursive">Is the delete recursive or not?</param>
		void DeleteDirectory(string dir, bool recursive);

		/// <summary>
		/// Creates all directories and subdirectories in the specified path.
		/// </summary>
		/// <param name="dir">The directory path to create.</param>
		void CreateDirectory(string dir);

		/// <summary>
		/// Gets the LastWriteTimeUtc for a file.
		/// If the directory cannot be accessed initially this call will block until we can either successfully
		/// get the LastWriteTimeUtc, or we give up.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		DateTime GetLastWriteTimeUtc(string fileName);

		/// <summary>
		/// Determines whether the given path refers to an existing directory on disk.
		/// </summary>
		/// <param name="dir">The path to test.</param>
		/// <returns>True if the directory exists, and false if it does not.</returns>
		bool DirectoryExists(string dir);

		/// <summary>
		/// Determines whether the specified file exists.
		/// </summary>
		/// <param name="path">The file to check.</param>
		/// <returns>True if the file exists, and false if it does not.</returns>
		bool FileExists(string path);

		/// <summary>
		/// Moves a specified file to a new location, providing the option to specify a new file name.
		/// </summary>
		/// <param name="sourceFileName">The name of the file to move.</param>
		/// <param name="destFileName">The new path for the file.</param>
		void MoveFile(string sourceFileName, string destFileName);

		/// <summary>
		/// Deletes the specified file.
		/// </summary>
		/// <param name="path">The name of the file to be deleted. Wildcard characters are not supported.</param>
		void DeleteFile(string path);

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
		string[] GetFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly);

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
		string[] GetDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly);

		/// <summary>
		/// Retrives the parent directory of the specified path, including both absolute and relative paths.
		/// </summary>
		/// <param name="path">The path for which to retrieve the parent directory.</param>
		/// <returns>The parent directory as a <see cref="DirectoryInfo"/> or null if none exists.</returns>
		DirectoryInfo GetParentDirectory(string path);

		/// <summary>
		/// Copies an existing file to a new file. Overwriting a file of the same name is allowed.
		/// </summary>
		/// <param name="sourceFileName">The file to copy.</param>
		/// <param name="destFileName">The name of the destination file. This cannot be a directory.</param>
		/// <param name="overwrite">true if the destination file can be overwrittin; otherwise false.</param>
		void Copy(string sourceFileName, string destFileName, bool overwrite);
	}
}
