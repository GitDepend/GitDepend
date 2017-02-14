using Newtonsoft.Json;

namespace GitDepend.Configuration
{
	/// <summary>
	/// The packages section of GitDepend.json
	/// </summary>
	public class Packages
	{
		/// <summary>
		/// The directory where packages can be found.
		/// </summary>
		[JsonProperty("dir")]
		public string Directory { get; set; } = "artifacts/NuGet/Debug";

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
