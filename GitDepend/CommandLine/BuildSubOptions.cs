using CommandLine;

namespace GitDepend.CommandLine
{
	/// <summary>
	/// The options for the build verb.
	/// </summary>
	public class BuildSubOptions : CommonSubOptions
	{
		/// <summary>
		/// The name of the dependency we are going to build.
		/// </summary>
		[ValueOption(0)]
		public string DependencyName { get; set; }

		/// <summary>
		/// Should all dependencies above be built.
		/// </summary>
		[Option('a', "all", DefaultValue = false, HelpText = "Indicates that all of the dependencies should be built.")]
		public bool All { get; set; }
	}
}