namespace GitDepend
{
	/// <summary>
	/// Project Return Codes.
	/// </summary>
	public enum ReturnCode
	{
		/// <summary>
		/// Indicates the program ran successfully.
		/// </summary>
		Success = 0,

		/// <summary>
		/// Indicates that the program could not find the GitDepend.json file.
		/// </summary>
		GitDependFileNotFound = 1,

		/// <summary>
		/// Indicates that the target directory is not a git repository.
		/// </summary>
		GitRepositoryNotFound = 2,

		/// <summary>
		/// Indicates that there was a failure while running a git command.
		/// </summary>
		FailedToRunGitCommand = 3,

		/// <summary>
		/// Indicates that there was a failure while running a nuget command.
		/// </summary>
		FailedToRunNugetCommand = 4,

		/// <summary>
		/// Indicates that a dependency failed to run the build script successfully.
		/// </summary>
		FailedToRunBuildScript = 5
	}
}
	