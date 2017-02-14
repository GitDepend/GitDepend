using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace GitDepend
{
	/// <summary>
	/// A helper class for dealing with nuget.exe
	/// </summary>
	public class Nuget : INuget
	{
		private readonly string _workingDir;

		/// <summary>
		/// The configuration file to use.
		/// </summary>
		public string ConfigFile { get; set; }

		/// <summary>
		/// Creates a new <see cref="Nuget"/>
		/// </summary>
		public Nuget()
		{
			_workingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		}

		/// <summary>
		/// Updates the specified nuget package in all projects within the given solution to the specified version.
		/// </summary>
		/// <param name="soluton">The solution file.</param>
		/// <param name="id">The id of the package.</param>
		/// <param name="version">The version of the package.</param>
		/// <param name="sourceDirectory">The directory containing packages on disk.</param>
		/// <returns>The nuget return code.</returns>
		public ReturnCode Update(string soluton, string id, string version, string sourceDirectory)
		{
			return ExecuteNuGetCommand($"update {soluton} -Id {id} -Version {version} {ConfigFileParam()} -Source \"{sourceDirectory}\" -Pre");
		}

		private string ConfigFileParam()
		{
			return string.IsNullOrEmpty(ConfigFile)
				? string.Empty
				: $"-ConfigFile {ConfigFile}";
		}

		private ReturnCode ExecuteNuGetCommand(string arguments)
		{
			var info = new ProcessStartInfo("NuGet.exe", arguments)
			{
				WorkingDirectory = _workingDir,
				UseShellExecute = false,
			};
			var proc = Process.Start(info);
			proc?.WaitForExit();

			var code = proc?.ExitCode ?? (int)ReturnCode.FailedToRunNugetCommand;

			return code != (int)ReturnCode.Success
				? ReturnCode.FailedToRunNugetCommand
				: ReturnCode.Success;
		}
	}
}