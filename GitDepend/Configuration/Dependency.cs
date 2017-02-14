using Newtonsoft.Json;

namespace GitDepend.Configuration
{
	/// <summary>
	/// The dependency section of GitDepend.json
	/// </summary>
	public class Dependency
	{
		/// <summary>
		/// The dependency name.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// The url where the dependency can be cloned with git.
		/// </summary>
		[JsonProperty("url")]
		public string Url { get; set; }

		/// <summary>
		/// The directory where the dependency exists, or to which it will be cloned.
		/// </summary>
		[JsonProperty("dir")]
		public string Directory { get; set; }

		/// <summary>
		/// The branch that should be checked out.
		/// </summary>
		[JsonProperty("branch")]
		public string Branch { get; set; }

		/// <summary>
		/// The <see cref="GitDependFile"/> that configures the dependency.
		/// </summary>
		[JsonIgnore]
		public GitDependFile Configuration { get; set; }

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
