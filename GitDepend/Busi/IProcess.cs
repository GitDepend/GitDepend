using System.Diagnostics;

namespace GitDepend.Busi
{
    /// <summary>
    /// An abstraction of <see cref="Process"/>
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// Gets the value that the associated process specified when it terminated.
        /// </summary>
        int ExitCode { get; }

        /// <summary>
        /// Indicates if the associated process has exited or not.
        /// </summary>
        bool HasExited { get; }

        /// <summary>
        /// Instructs the <see cref="IProcess"/> to wait indefinitely for the associated process to exit.
        /// </summary>
        void WaitForExit();
    }
}