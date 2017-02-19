using System.Diagnostics;

namespace GitDepend.Busi
{
    /// <summary>
    /// An implementation of <see cref="IProcess"/> that delegates all method
    /// calls and properties to the associated <see cref="Process"/>
    /// </summary>
    public class ProcessWrapper : IProcess
    {
        private readonly Process _proc;

        /// <summary>
        /// Creates a new <see cref="ProcessWrapper"/> around the specified <see cref="Process"/>
        /// </summary>
        /// <param name="proc">The <see cref="Process"/> to wrap.</param>
        public ProcessWrapper(Process proc)
        {
            _proc = proc;
        }

        #region Implementation of IProcess

        /// <summary>
        /// Gets the value that the associated process specified when it terminated.
        /// </summary>
        public int ExitCode => _proc?.ExitCode ?? 0;

        /// <summary>
        /// Indicates if the associated process has exited or not.
        /// </summary>
        public bool HasExited => _proc?.HasExited ?? true;

        /// <summary>
        /// Instructs the <see cref="IProcess"/> to wait indefinitely for the associated process to exit.
        /// </summary>
        public void WaitForExit()
        {
            _proc?.WaitForExit();
        }

        #endregion
    }
}