using Newtonsoft.Json;

namespace GitDepend.Configuration
{
	/// <summary>
	/// The build section of GitDepend.json
	/// </summary>
	public class Build
	{
		/// <summary>
		/// The script to run when building this repository.
		/// </summary>
		[JsonProperty("script")]
		public string Script { get; set; } = "make.bat";

		/// <summary>
		/// Any script arguments.
		/// </summary>
		[JsonProperty("arguments")]
		public string Arguments { get; set; }

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