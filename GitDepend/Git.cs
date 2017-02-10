using System.Diagnostics;

namespace GitDepend
{
	public class Git
	{
		private readonly string _workingDir;

		public Git()
		{
		}

		public Git(string workingDir)
		{
			_workingDir = workingDir;
		}

		public int Checkout(string branch)
		{
			return ExecuteGitCommand($"checkout {branch}");
		}

		public int Clone(string url, string directory, string branch)
		{
			return ExecuteGitCommand($"clone {url} {directory} -b {branch}");
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
