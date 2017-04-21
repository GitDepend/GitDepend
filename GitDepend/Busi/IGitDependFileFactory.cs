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
        /// <param name="code">The return code indicating if the load was successful, or which error occurred.</param>
        /// <returns>A <see cref="GitDependFile"/> or null if none could be loaded.</returns>
        GitDependFile LoadFromDirectory(string directory, out string dir, out ReturnCode code);

        /// <summary>
        /// Finds a GitDepend.json file in the given directory and loads it into memory.
        /// </summary>
        /// <param name="directory">The directory to start in.</param>
        /// <returns>Whether the key file exists</returns>
        bool CheckForDependencyInclude(string directory);

    }
}