using System.Diagnostics;
using GitDepend.Busi;

namespace GitDepend
{
	/// <summary>
	/// A helper class for dealing with git.exe
	/// </summary>
	public class Git : IGit
	{
		private readonly IProcessManager _processManager;

		/// <summary>
		/// The working directory for all git operations.
		/// </summary>
		public string WorkingDir { get; set; }

		/// <summary>
		/// Creates a new <see cref="Git"/>
		/// </summary>
		/// <param name="processManager">The <see cref="IProcessManager"/> to use.</param>
		public Git(IProcessManager processManager)
		{
			_processManager = processManager;
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
			return ExecuteGitCommand($"commit -m \"{message}\"");
		}

		private ReturnCode ExecuteGitCommand(string arguments)
		{
			var info = new ProcessStartInfo("git", arguments)
			{
				WorkingDirectory = WorkingDir,
				UseShellExecute = false,
			};
			var proc = _processManager.Start(info);
			proc?.WaitForExit();

			var code = proc?.ExitCode ?? (int) ReturnCode.FailedToRunGitCommand;

			return code != (int) ReturnCode.Success
				? ReturnCode.FailedToRunGitCommand
				: ReturnCode.Success;
		}
	}
}
