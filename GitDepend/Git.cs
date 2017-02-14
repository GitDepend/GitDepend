using System.Diagnostics;

namespace GitDepend
{
	/// <summary>
	/// A helper class for dealing with git.exe
	/// </summary>
	public class Git
	{
		private readonly string _workingDir;

		/// <summary>
		/// Creates a new <see cref="Git"/>
		/// </summary>
		public Git()
		{
		}

		/// <summary>
		/// Creates a new <see cref="Git"/> that operates on the given working directory.
		/// </summary>
		/// <param name="workingDir">The working directory for all git operations.</param>
		public Git(string workingDir)
		{
			_workingDir = workingDir;
		}

		/// <summary>
		/// Checks out the given branch.
		/// </summary>
		/// <param name="branch">The branch to check out.</param>
		/// <returns></returns>
		public int Checkout(string branch)
		{
			return ExecuteGitCommand($"checkout {branch}");
		}

		/// <summary>
		/// Clones a repository.
		/// </summary>
		/// <param name="url">The repository url.</param>
		/// <param name="directory">The directory where the repository should be cloned.</param>
		/// <param name="branch">The branch that should be checked out.</param>
		/// <returns></returns>
		public int Clone(string url, string directory, string branch)
		{
			return ExecuteGitCommand($"clone {url} {directory} -b {branch}");
		}

		/// <summary>
		/// Adds files into the staging area to prepare them for a commit.
		/// </summary>
		/// <param name="files">The files to add to the staging area.</param>
		/// <returns></returns>
		public int Add(params string[] files)
		{
			foreach (string file in files)
			{
				ExecuteGitCommand($"add {file}");
			}

			return ReturnCodes.Success;
		}

		/// <summary>
		/// Shows the status of the repository.
		/// </summary>
		/// <returns></returns>
		public int Status()
		{
			return ExecuteGitCommand("status");
		}

		/// <summary>
		/// Makes a commit with the given message.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public int Commit(string message)
		{
			return ExecuteGitCommand($"commit -m \"{message}\"");
		}

		private int ExecuteGitCommand(string arguments)
		{
			var info = new ProcessStartInfo("git", arguments)
			{
				WorkingDirectory = _workingDir,
				UseShellExecute = false,
			};
			var proc = Process.Start(info);
			proc?.WaitForExit();

			return proc?.ExitCode ?? ReturnCodes.FailedToRunGitCommand;
		}
	}
}
