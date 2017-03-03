using System;
using System.IO;

namespace GitDepend.Busi
{
    /// <summary>
    /// Abstracts calls the <see cref="Console"/> to improve testablility.
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// Gets or sets the foreground color of the console.
        /// </summary>
        ConsoleColor ForegroundColor { get; set; }

        /// <summary>
        /// Gets the standard output stream.
        /// </summary>
        TextWriter Out { get; }

        /// <summary>
        /// Gets the standard error stream.
        /// </summary>
        TextWriter Error { get; }

        /// <summary>
        /// Writes a line terminator.
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Writes a text representation of an object by calling the ToString method, followed by a new line.
        /// </summary>
        /// <param name="value">The object to write. If the object is null only the line terminator is written.</param>
        void WriteLine(object value);

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="string.Format(string,object)"/>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arguments">An array of objects to insert into the format string.</param>
        void WriteLine(string format, params object[] arguments);

        /// <summary>
        /// Writes out a formatted string, using the same semantics as <see cref="string.Format(string,object)"/>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arguments">An array of objects to insert into the format string.</param>
        void Write(string format, params object[] arguments);

        /// <summary>
        /// Reads the next line of characters from the input stream.
        /// </summary>
        /// <returns>The next line of characters from the input stream.</returns>
        string ReadLine();
    }
}