using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDepend.Busi
{
    /// <summary>
    /// Provides access to strings located in the resource files.
    /// </summary>
    public interface IUiStrings
    {
        /// <summary>
        /// Gets the string associated with the given key.
        /// </summary>
        /// <param name="key">The resource string key.</param>
        /// <returns>The string from the resource files.</returns>
        string GetString(string key);
    }
}
