using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace GitDepend.Configuration
{
	public class GitDependFile
	{
		private List<Dependency> _dependencies;
		private Build _build;
		private Packages _packages;

		[JsonProperty("build")]
		public Build Build => _build ?? (_build = new Build());

		[JsonProperty("packages")]
		public Packages Packages => _packages ?? (_packages = new Packages());

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

		public static GitDependFile LoadFromDir(string directory, out string dir, out string error)
		{
			dir = directory;
			var current = directory;
			bool isGitRoot;
			do
			{
				isGitRoot = Directory.GetDirectories(current, ".git").Any();

				if (!isGitRoot)
				{
					current = Directory.GetParent(current)?.FullName;
				}

			} while (!string.IsNullOrEmpty(current) && !isGitRoot);
			

			if (!string.IsNullOrEmpty(current) && isGitRoot)
			{
				var file = Path.Combine(current, "GitDepend.json");

				if (File.Exists(file))
				{
					try
					{
						var json = File.ReadAllText(file);
						var gitDependFile = JsonConvert.DeserializeObject<GitDependFile>(json);
						error = null;
						dir = current;
						return gitDependFile;
					}
					catch (Exception ex)
					{
						error = ex.Message;
						Console.Error.WriteLine(ex.Message);
						return null;
					}
				}
				error = null;
				return new GitDependFile();
			}

			error = "This is not a git repository";
			return null;
		}
	}
}