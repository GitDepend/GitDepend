using System.Diagnostics;
using System.IO.Abstractions;

namespace GitDepend.Busi
{
    /// <summary>
    /// A helper class for dealing with git.exe
    /// </summary>
    public class Git : IGit
    {
        private readonly IProcessManager _processManager;
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// The working directory for all git operations.
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Creates a new <see cref="Git"/>
        /// </summary>
        public Git()
        {
            _processManager = DependencyInjection.Resolve<IProcessManager>();
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
        }

        /// <summary>
        /// Checks out the given branch.
        /// </summary>
        /// <param name="branch">The branch to check out.</param>
        /// <returns>The git return code.</returns>
        public ReturnCode Checkout(string branch)
        {
            return ExecuteGitCommand($"checkout {branch}");
        }

        /// <summary>
        /// Clones a repository.
        /// </summary>
        /// <param name="url">The repository url.</param>
        /// <param name="directory">The directory where the repository should be cloned.</param>
        /// <param name="branch">The branch that should be checked out.</param>
        /// <returns>The git return code.</returns>
        public ReturnCode Clone(string url, string directory, string branch)
        {
            return ExecuteGitCommand($"clone {url} {directory} -b {branch}");
        }

        /// <summary>
        /// Adds files into the staging area to prepare them for a commit.
        /// </summary>
        /// <param name="files">The files to add to the staging area.</param>
        /// <returns>The git return code.</returns>
        public ReturnCode Add(params string[] files)
        {
            foreach (string file in files)
            {
                ExecuteGitCommand($"add {file}");
            }

            return ReturnCode.Success;
        }

        /// <summary>
        /// Shows the status of the repository.
        /// </summary>
        /// <returns>The git return code.</returns>
        public ReturnCode Status()
        {
            return ExecuteGitCommand("status");
        }

        /// <summary>
        /// Makes a commit with the given message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>The git return code.</returns>
        public ReturnCode Commit(string message)
        {
            var file = _fileSystem.Path.GetTempFileName();
            _fileSystem.File.WriteAllText(file, message);

            var code = ExecuteGitCommand($"commit --file=\"{file}\"");

            _fileSystem.File.Delete(file);
            return code;
        }

        private ReturnCode ExecuteGitCommand(string arguments)
        {
            var info = new ProcessStartInfo("git", arguments)
            {
                WorkingDirectory = WorkingDirectory,
                UseShellExecute = false,
            };
            var proc = _processManager.Start(info);
            proc?.WaitForExit();

            var code = proc?.ExitCode ?? (int)ReturnCode.FailedToRunGitCommand;

            return code != (int)ReturnCode.Success
                ? ReturnCode.FailedToRunGitCommand
                : ReturnCode.Success;
        }
    }
}
