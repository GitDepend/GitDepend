using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GitDepend.Busi
{
    /// <summary>
    /// A helper class for dealing with nuget.exe
    /// </summary>
    public class Nuget : INuget
    {
        private readonly IConsole _console;
        private const int ErrorEncountered = 1;

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
            return ExecuteNuGetCommand($"update {soluton} -Id {id} -Version {version} -Source \"{sourceDirectory}\" -Pre -verbosity quiet");
        }

        private ReturnCode ExecuteNuGetCommand(string arguments)
        {
            var oldOut = Console.Out;

            StringBuilder sb = new StringBuilder();
            Console.SetOut(new StringWriter(sb));
            var args = Regex.Matches(arguments, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(m => m.Value.Trim('"'))
                .ToArray();

            var code = NuGet.CommandLine.Program.Main(args);
            //var output = sb.ToString();
            //var hasWarnings = output.ToLower().Contains("warning");
            
            Console.SetOut(oldOut);
            
            _console.WriteLine($"nuget {arguments}");
            //if (hasWarnings)
            //{
            //    _console.WriteLine(output);
            //    code = ErrorEncountered;
            //}

            return code != (int)ReturnCode.Success
                ? ReturnCode.FailedToRunNugetCommand
                : ReturnCode.Success;
        }
    }
}