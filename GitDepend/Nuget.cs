using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace GitDepend
{
	/// <summary>
	/// A helper class for dealing with nuget.exe
	/// </summary>
	public class Nuget
	{
		private readonly string _configFile;
		private readonly string _workingDir;

		/// <summary>
		/// Creates a new <see cref="Nuget"/>
		/// </summary>
		/// <param name="configFile">The NuGet.config file to use.</param>
		public Nuget(string configFile)
		{
			_configFile = configFile;
			_workingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		}

		/// <summary>
		/// Updates the specified nuget package in all projects within the given solution to the specified version.
		/// </summary>
		/// <param name="soluton">The solution file.</param>
		/// <param name="id">The id of the package.</param>
		/// <param name="version">The version of the package.</param>
		/// <param name="sourceDirectory">The directory containing packages on disk.</param>
		/// <returns></returns>
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