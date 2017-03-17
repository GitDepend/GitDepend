using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GitDepend.CommandLine;
using LibGit2Sharp;
using NUnit.Framework;

namespace GitDepend.IntegrationTests
{
    public class TestFixtureBase
    {
        protected const string Lib1Name = "Lib1";
        protected const string Lib1Url = "https://github.com/GitDepend/Lib1.git";

        protected const string Lib2Name = "Lib2";
        protected const string Lib2Url = "https://github.com/GitDepend/Lib2.git";

        /// <summary>
        /// Executes GitDepend with the given arguments
        /// </summary>
        /// <param name="arguments">The arguments to pass to GitDepend.</param>
        /// <returns>The <see cref="GitDependExecutionInfo"/> with execution results.</returns>
        protected GitDependExecutionInfo GitDepend(string arguments)
        {
            return GitDepend(arguments, ".");
        }

        /// <summary>
        /// Executes GitDepend with the given arguments, and input strings.
        /// </summary>
        /// <param name="arguments">The arguments to pass to GitDepend.</param>
        /// <param name="input">An array of user inputs to feed to standard in.</param>
        /// <returns>The <see cref="GitDependExecutionInfo"/> with execution results.</returns>
        protected GitDependExecutionInfo GitDepend(string arguments, string[] input)
        {
            return GitDepend(arguments, ".", input);
        }

        /// <summary>
        /// Executes GitDepend with the given arguments, and runs in the given working directory.
        /// </summary>
        /// <param name="arguments">The arguments to pass to GitDepend.</param>
        /// <param name="workingDirectory">The working directory for GitDepend.</param>
        /// <returns>The <see cref="GitDependExecutionInfo"/> with execution results.</returns>
        protected GitDependExecutionInfo GitDepend(string arguments, string workingDirectory)
        {
            return GitDepend(arguments, workingDirectory, new string[0]);
        }

        /// <summary>
        /// Executes GitDepend with the given arguments and input strings running in the given working directory.
        /// </summary>
        /// <param name="arguments">The arguments to pass to GitDepend.</param>
        /// <param name="workingDirectory">The working directory for GitDepend.</param>
        /// <param name="input">An array of user inputs to feed to standard in.</param>
        /// <returns>The <see cref="GitDependExecutionInfo"/> with execution results.</returns>
        protected GitDependExecutionInfo GitDepend(string arguments, string workingDirectory, string[] input)
        {
            var info = new ProcessStartInfo("GitDepend.exe", arguments)
            {
                UseShellExecute = false,
                WorkingDirectory = workingDirectory,

                CreateNoWindow = true,

                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var executionInfo = new GitDependExecutionInfo();

            using (var proc = Process.Start(info))
            {
                using (var writer = proc.StandardInput)
                {
                    foreach (var line in input ?? new string[0])
                    {
                        writer.WriteLine(line);
                    }
                }

                using (var reader = proc.StandardOutput)
                {
                    executionInfo.StandardOut = reader.ReadToEnd();
                }

                using (var reader = proc.StandardError)
                {
                    executionInfo.StandardError = reader.ReadToEnd();
                }
                proc.WaitForExit();
                executionInfo.ReturnCode = (ReturnCode)proc.ExitCode;
            }

            return executionInfo;
        }

        /// <summary>
        /// Clones a git repository
        /// </summary>
        /// <param name="sourceUrl">The url to clone.</param>
        /// <param name="workdirPath">The directory where the repository should be cloned.</param>
        protected void Clone(string sourceUrl, string workdirPath)
        {
            Repository.Clone(sourceUrl, workdirPath);
        }

        /// <summary>
        /// Clones a git repository
        /// </summary>
        /// <param name="sourceUrl">The url to clone.</param>
        /// <param name="workdirPath">The directory where the repository should be cloned.</param>
        /// <param name="options">The <see cref="CloneOptions"/> that control the clone operation.</param>
        protected void Clone(string sourceUrl, string workdirPath, CloneOptions options)
        {
            Repository.Clone(sourceUrl, workdirPath, options);
        }

        /// <summary>
        /// Deletes a path without throwing an exception.
        /// </summary>
        /// <param name="path">The directory to delete.</param>
        protected void SafeDeleteDirectory(string path)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Writes the text in ReplaceFile into SourceFile
        /// </summary>
        /// <param name="workdirPath"></param>
        /// <param name="sourcefile"></param>
        /// <param name="replaceFile"></param>
        /// <returns>Update File was successful</returns>
        protected bool UpdateFile(string workdirPath, string sourcefile, string replaceFile)
        {
            if (!Directory.Exists(workdirPath))
                return false;

            if (!File.Exists(Path.Combine(workdirPath, sourcefile)))
                return false;

            var path = Path.Combine(workdirPath, sourcefile);
            File.WriteAllText(path, File.ReadAllText(replaceFile));

            return true;
        }
    }
}
