using GitDepend.Configuration;

namespace GitDepend.Busi
{
	/// <summary>
	/// A class that can load a <see cref="GitDependFile"/> from disk.
	/// </summary>
	public interface IGitDependFileFactory
	{
		/// <summary>
		/// Finds a GitDepend.json file in the given directory and loads it into memory.
		/// </summary>
		/// <param name="directory">The directory to start in.</param>
		/// <param name="dir">The directory where GitDepend.json was found.</param>
		/// <param name="error">An error string indicating what went wrong in the case that the file could not be loaded.</param>
		/// <returns>A <see cref="GitDependFile"/> or null if none could be loaded.</returns>
		GitDependFile LoadFromDirectory(string directory, out string dir, out string error);
	}
}