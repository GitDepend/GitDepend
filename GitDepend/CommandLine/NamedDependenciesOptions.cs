using System.Collections.Generic;
using CommandLine;

namespace GitDepend.CommandLine
{
    /// <summary>
    /// Provides command line options for verbs that specify that they only operate on the specified dependencies.
    /// </summary>
    public abstract class NamedDependenciesOptions : CommonSubOptions
    {
        /// <summary>
        /// Specifies which dependencies will be affected.
        /// </summary>
        [ValueList(typeof(List<string>))]
        public IList<string> Dependencies { get; set; }
    }
}