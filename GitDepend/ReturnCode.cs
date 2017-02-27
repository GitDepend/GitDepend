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
        /// Indicates that the target directory is not a git repository.
        /// </summary>
        GitRepositoryNotFound = 1,

        /// <summary>
        /// Indicates that there was a failure while running a git command.
        /// </summary>
        FailedToRunGitCommand = 2,

        /// <summary>
        /// Indicates that there was a failure while running a nuget command.
        /// </summary>
        FailedToRunNugetCommand = 3,

        /// <summary>
        /// Indicates that a dependency failed to run the build script successfully.
        /// </summary>
        FailedToRunBuildScript = 4,

        /// <summary>
        /// Indicates that the specified directory does not exist.
        /// </summary>
        DirectoryDoesNotExist = 5,

        /// <summary>
        /// Indicates that there was a failre when trying to create the nuget cache directory.
        /// </summary>
        CouldNotCreateCacheDirectory = 6,

        /// <summary>
        /// Indicates that the specified dependency url is invalid. The only supported format is https
        /// </summary>
        InvalidUrlFormat = 7,

        /// <summary>
        /// Specifies that a dependency is missing in the GitDepend.json
        /// </summary>
        MissingDependency = 8,

        /// <summary>
        /// Indicates the supplied arguments were invalid.
        /// </summary>
        InvalidArguments = 9997,

        /// <summary>
        /// Indicates that the command parser did not understand the specified command.
        /// </summary>
        InvalidCommand = 9998,

        /// <summary>
        /// Indicates that something went wrong, but the cause could not be determined.
        /// </summary>
        UnknownError = 9999,
    }
}
