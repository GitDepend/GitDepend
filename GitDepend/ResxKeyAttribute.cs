using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDepend
{
    /// <summary>
    /// Provides a key for strings in the resx files.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ResxKeyAttribute : Attribute
    {
        /// <summary>
        /// The resource key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Creates a new <see cref="ResxKeyAttribute"/> witht he specified key.
        /// </summary>
        /// <param name="key">The key for the resx files.</param>
        public ResxKeyAttribute(string key)
        {
            Key = key;
        }
    }
}
