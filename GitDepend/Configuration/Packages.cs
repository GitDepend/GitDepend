﻿using Newtonsoft.Json;

namespace GitDepend.Configuration
{
	public class Packages
	{
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