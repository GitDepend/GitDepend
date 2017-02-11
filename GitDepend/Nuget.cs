using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace GitDepend
{
	public class Nuget
	{
		private readonly string _configFile;
		private readonly string _workingDir;

		public Nuget(string configFile)
		{
			_configFile = configFile;
			_workingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		}

		public int Update(string soluton, string id, string version, string sourceDirectory)
		{
			return ExecuteNuGetCommand($"update {soluton} -Id {id} -Version {version} {ConfigFileParam()} -Source \"{sourceDirectory}\" -Pre");
		}

		private string ConfigFileParam()
		{
			return string.IsNullOrEmpty(_configFile)
				? string.Empty
				: $"-ConfigFile {_configFile}";
		}

		private int ExecuteNuGetCommand(string arguments)
		{
			var info = new ProcessStartInfo("NuGet.exe", arguments)
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