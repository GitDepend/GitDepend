using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GitDepend.Busi
{
    /// <summary>
    /// Abstracts Console.Write* calls to remove output statments from Unit Tests.
    /// </summary>
    public class ConsoleWrapper : IConsole
    {
        #region Implementation of IConsole

        /// <summary>
        /// Gets or sets the foreground color of the console.
        /// </summary>
        public ConsoleColor ForegroundColor
        {
            get { return Console.ForegroundColor; }
            set { Console.ForegroundColor = value; }
        }

        /// <summary>
        /// Gets the standard output stream.
        /// </summary>
        public TextWriter Out => Console.Out;

        /// <summary>
        /// Gets the standard error stream.
        /// </summary>
        public TextWriter Error => Console.Error;

        /// <summary>
        /// Writes a line terminator.
        /// </summary>
        public void WriteLine()
        {
            Console.WriteLine();
        }

        /// <summary>
        /// Writes a text representation of an object by calling the ToString method, followed by a new line.
        /// </summary>
        /// <param name="value">The object to write. If the object is null only the line terminator is written.</param>
        public void WriteLine(object value)
        {
            Console.WriteLine(value);
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="string.Format(string,object)"/>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arguments">An array of objects to insert into the format string.</param>
        public void WriteLine(string format, params object[] arguments)
        {
            Console.WriteLine(format, arguments);
        }

        /// <summary>
        /// Writes out a formatted string, using the same semantics as <see cref="string.Format(string,object)"/>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arguments">An array of objects to insert into the format string.</param>
        public void Write(string format, params object[] arguments)
        {
            Console.Write(format, arguments);
        }

        /// <summary>
        /// Reads the next line of characters from the input stream.
        /// </summary>
        /// <returns>The next line of characters from the input stream.</returns>
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        #endregion
    }
}
