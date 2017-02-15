using System.Diagnostics;

namespace GitDepend.Busi
{
	/// <summary>
	/// Implementation of <see cref="IProcessManager"/> that proxies calls to the <see cref="Process"/> clas.
	/// </summary>
	public class ProcessManager : IProcessManager
	{
		#region Implementation of IProcessManager

		/// <summary>
		/// Starts the process resource that is specified by the parameter containing process
		/// start information (for example, the file name of the process to start) and associates
		/// the resource with a new <see cref="Process"/> component.
		/// </summary>
		/// <param name="startInfo">
		/// The <see cref="ProcessStartInfo"/> that contains the information that is
		/// used to start the process, including the file name and any command-line arguments.
		/// </param>
		/// <returns>
		/// A new <see cref="Process"/> that is associated with the process resource,
		/// or null if no process resource is started. Note that a new process that’s started
		/// alongside already running instances of the same process will be independent from
		/// the others. In addition, Start may return a non-null Process with its <see cref="Process.HasExited"/>
		/// property already set to true. In this case, the started process may have activated
		/// an existing instance of itself and then exited.
		/// </returns>
		public IProcess Start(ProcessStartInfo startInfo)
		{
			var proc = Process.Start(startInfo);
			return new ProcessWrapper(proc);
		}

		#endregion
	}
}