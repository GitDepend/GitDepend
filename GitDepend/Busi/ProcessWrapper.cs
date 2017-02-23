using System.Diagnostics;
using System.IO;

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

        /// <summary>
        /// Gets a handle to the standard output stream.
        /// </summary>
        public StreamReader StandardOutput => _proc?.StandardOutput;

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _proc?.Dispose();
        }

        #endregion
    }
}