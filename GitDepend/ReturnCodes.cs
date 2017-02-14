namespace GitDepend
{
	/// <summary>
	/// Project Return Codes.
	/// </summary>
	public static class ReturnCodes
	{
		/// <summary>
		/// Indicates the program ran successfully.
		/// </summary>
		public const int Success = 0;

		/// <summary>
		/// Indicates that the program could not find the GitDepend.json file.
		/// </summary>
		public const int GitDependFileNotFound = 1;

		/// <summary>
		/// Indicates that the target directory is not a git repository.
		/// </summary>
		public const int GitRepositoryNotFound = 2;

		/// <summary>
		/// Indicates that there was a failure while running a git command.
		/// </summary>
		public const int FailedToRunGitCommand = 3;

		/// <summary>
		/// Indicates that a dependency failed to run the build script successfully.
		/// </summary>
		public const int FailedToRunBuildScript = 4;
	}
}
	