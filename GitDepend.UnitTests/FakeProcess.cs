using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Busi;

namespace GitDepend.UnitTests
{
	public class FakeProcess : IProcess
	{
		#region Implementation of IProcess

		/// <summary>
		/// Gets the value that the associated process specified when it terminated.
		/// </summary>
		public int ExitCode { get; set; }

		/// <summary>
		/// Indicates if the associated process has exited or not.
		/// </summary>
		public bool HasExited { get; set; } = true;

		/// <summary>
		/// Instructs the <see cref="IProcess"/> to wait indefinitely for the associated process to exit.
		/// </summary>
		public void WaitForExit()
		{
			
		}

		#endregion
	}
}
