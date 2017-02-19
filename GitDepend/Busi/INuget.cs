namespace GitDepend.Busi
{
    /// <summary>
    /// A helper class for dealing with nuget.exe
    /// </summary>
    public interface INuget
    {
        /// <summary>
        /// The working directory for nuget.exe
        /// </summary>
        string WorkingDirectory { get; set; }

        /// <summary>
        /// Updates the specified nuget package in all projects within the given solution to the specified version.
        /// </summary>
        /// <param name="soluton">The solution file.</param>
        /// <param name="id">The id of the package.</param>
        /// <param name="version">The version of the package.</param>
        /// <param name="sourceDirectory">The directory containing packages on disk.</param>
        /// <returns>The nuget return code.</returns>
        ReturnCode Update(string soluton, string id, string version, string sourceDirectory);
    }
}