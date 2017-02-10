namespace GitDepend
{
	public static class ReturnCodes
	{
		public const int Success = 0;
		public const int GitDependFileNotFound = 1;
		public const int GitRepositoryNotFound = 2;
		public const int FailedToRunGitCommand = 3;
		public const int FailedToRunBuildScript = 4;
	}
}
	