using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDepend.Busi
{
	/// <summary>
	/// Abstracts the Static methods on the <see cref="Process"/> class.
	/// </summary>
	public interface IProcessManager
	{
		/// <summary>
		/// Starts the process resource that is specified by the parameter containing process
		/// start information (for example, the file name of the process to start) and associates
		/// the resource with a new <see cref="IProcess"/> component.
		/// </summary>
		/// <param name="startInfo">
		/// The <see cref="ProcessStartInfo"/> that contains the information that is
		/// used to start the process, including the file name and any command-line arguments.
		/// </param>
		/// <returns>
		/// A new <see cref="IProcess"/> that is associated with the process resource,
		/// or null if no process resource is started. Note that a new process that’s started
		/// alongside already running instances of the same process will be independent from
		/// the others. In addition, Start may return a non-null Process with its <see cref="IProcess.HasExited"/>
		/// property already set to true. In this case, the started process may have activated
		/// an existing instance of itself and then exited.
		/// </returns>
		IProcess Start(ProcessStartInfo startInfo);
	}
}
