using System.Collections.Generic;
using Newtonsoft.Json;

namespace GitDepend.Configuration
{
    /// <summary>
    /// Represents GitDepend.json in memory.
    /// </summary>
    public class GitDependFile
    {
        private List<Dependency> _dependencies;
        private Build _build;
        private Packages _packages;

        /// <summary>
        /// The name of this repository
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The build section.
        /// </summary>
        [JsonProperty("build")]
        public Build Build => _build ?? (_build = new Build());

        /// <summary>
        /// The packages section
        /// </summary>
        [JsonProperty("packages")]
        public Packages Packages => _packages ?? (_packages = new Packages());

        /// <summary>
        /// A list of <see cref="Dependency"/> objects for this repository.
        /// </summary>
        [JsonProperty("dependencies")]
        public List<Dependency> Dependencies => _dependencies ?? (_dependencies = new List<Dependency>());

        #region Overrides of Object

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        #endregion
    }
}