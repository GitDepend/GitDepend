using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public int ExitCode { get; set; } = 0;

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

        /// <summary>
        /// Gets a handle to the standard output stream.
        /// </summary>
        public StreamReader StandardOutput => new StreamReader(new MemoryStream());

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            
        }

        #endregion
    }
}
