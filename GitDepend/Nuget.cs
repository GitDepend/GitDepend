using System.Diagnostics;
using System.IO.Abstractions;
using System.Reflection;
using GitDepend.Busi;

namespace GitDepend
{
	/// <summary>
	/// A helper class for dealing with nuget.exe
	/// </summary>
	public class Nuget : INuget
	{
		private readonly IProcessManager _processManager;

		/// <summary>
		/// The working directory for nuget.exe
		/// </summary>
		public string WorkingDirectory { get; set; }

		/// <summary>
		/// Creates a new <see cref="Nuget"/>
		/// </summary>
		/// <param name="processManager">The <see cref="IProcessManager"/> to use.</param>
		/// <param name="fileSystem">The <see cref="IFileSystem"/> to use.</param>
		public Nuget(IProcessManager processManager, IFileSystem fileSystem)
		{
			_processManager = processManager;
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
			return ExecuteNuGetCommand($"update {soluton} -Id {id} -Version {version} -Source \"{sourceDirectory}\" -Pre");
		}

		private ReturnCode ExecuteNuGetCommand(string arguments)
		{
			var info = new ProcessStartInfo("NuGet.exe", arguments)
			{
				WorkingDirectory = WorkingDirectory,
				UseShellExecute = false,
			};
			var proc = _processManager.Start(info);
			proc?.WaitForExit();

			var code = proc?.ExitCode ?? (int)ReturnCode.FailedToRunNugetCommand;

			return code != (int)ReturnCode.Success
				? ReturnCode.FailedToRunNugetCommand
				: ReturnCode.Success;
		}
	}
}