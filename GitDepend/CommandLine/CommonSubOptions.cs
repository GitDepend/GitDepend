using CommandLine;

namespace GitDepend.CommandLine
{
	public abstract class CommonSubOptions
	{
		[Option('d', "dir", Required = false, HelpText = "The directory to process. The current working directory will be used if this option is ignored.")]
		public string Directory { get; set; }
	}
}
