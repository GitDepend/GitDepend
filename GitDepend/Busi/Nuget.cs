using System.Diagnostics;
using System.IO.Abstractions;

namespace GitDepend.Busi
{
    /// <summary>
    /// A helper class for dealing with nuget.exe
    /// </summary>
    public class Nuget : INuget
    {
        private readonly IConsole _console;
        private readonly IProcessManager _processManager;

        /// <summary>
        /// The working directory for nuget.exe
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Creates a new <see cref="Nuget"/>
        /// </summary>
        public Nuget()
        {
            _console = DependencyInjection.Resolve<IConsole>();
            _processManager = DependencyInjection.Resolve<IProcessManager>();
        }

        /// <summary>
        /// Restores nuget packages needed by the provided solution.
        /// </summary>
        /// <param name="solution">The solution file.</param>
        public ReturnCode Restore(string solution)
        {
            return ExecuteNuGetCommand($"restore {solution}");
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
                RedirectStandardOutput = true
            };
            _console.WriteLine($"{info.FileName} {arguments}");

            int code;
            using (var proc = _processManager.Start(info))
            {
                proc.StandardOutput.ReadToEnd();
                proc?.WaitForExit();
                code = proc?.ExitCode ?? (int)ReturnCode.FailedToRunNugetCommand;
            }

            return code != (int)ReturnCode.Success
                ? ReturnCode.FailedToRunNugetCommand
                : ReturnCode.Success;
        }
    }
}