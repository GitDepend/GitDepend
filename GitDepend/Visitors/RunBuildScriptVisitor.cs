using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
	public class RunBuildScriptVisitor : IVisitor
	{
		#region Implementation of IVisitor

		public int ReturnCode { get; set; }
		public int VisitDependency(Dependency dependency)
		{
			string dir;
			string error;
			var config = GitDependFile.LoadFromDir(dependency.Directory, out dir, out error);

			if (config == null)
			{
				return ReturnCodes.GitRepositoryNotFound;
			}

			var info = new ProcessStartInfo(Path.Combine(dependency.Directory, config.Build.Script), config.Build.Arguments)
			{
				WorkingDirectory = dependency.Directory,
				UseShellExecute = false
			};
			var proc = Process.Start(info);
			proc?.WaitForExit();
			return proc?.ExitCode ?? ReturnCodes.FailedToRunBuildScript;
		}

		#endregion
	}
}
