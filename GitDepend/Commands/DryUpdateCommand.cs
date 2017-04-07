using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Resources;
using GitDepend.Visitors;
using NuGet;

namespace GitDepend.Commands
{
	/// <summary>
	/// An implementation of <see cref="ICommand"/> that recursively does a dry run update for all dependencies.
	/// </summary>
	public class DryUpdateCommand : ICommand
	{
		private readonly UpdateSubOptions _options;
		private readonly IConsole _console;
		private readonly IDependencyVisitorAlgorithm _algorithm;

		/// <summary>
		/// The verb name
		/// </summary>
		public const string Name = "dryupdate";

		/// <summary>
		/// Creates a new <see cref="DryUpdateCommand"/>
		/// </summary>
		/// <param name="options">The <see cref="UpdateSubOptions"/> that configure the command.</param>
		public DryUpdateCommand(UpdateSubOptions options)
		{
			_options = options;
			_console = DependencyInjection.Resolve<IConsole>();
			_algorithm = DependencyInjection.Resolve<IDependencyVisitorAlgorithm>();
		}

		#region Implementation of ICommand

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <returns>The return code.</returns>
		public ReturnCode Execute()
		{
			var verifyVisitor = new VerifyCorrectBranchDryVisitor(_options.Dependencies);
			_algorithm.TraverseDependencies(verifyVisitor, _options.Directory);

			if (verifyVisitor.ReturnCode != ReturnCode.Success)
			{
				_console.WriteLine(strings.NOT_ALL_DEPS_CORRECT_BRANCH);
				return verifyVisitor.ReturnCode;
			}

			//checkArtifactsVisitor this will check to see if we are up to date with the artifacts.
			_algorithm.Reset();

			List<string> dependeciesToBuild = new List<string>();
			List<string> projectsToUpdate = new List<string>();

			var checkArtifactsVisitor = new CheckArtifactsVisitor(_options.Dependencies, _options.Force);
			_algorithm.TraverseDependencies(checkArtifactsVisitor, _options.Directory);

			//if (checkArtifactsVisitor.ReturnCode == ReturnCode.Success && checkArtifactsVisitor.UpToDate)
			//{
			//	_console.WriteLine(strings.PACKAGES_UP_TO_DATE);
			//	return ReturnCode.Success;
			//}

			if (checkArtifactsVisitor.ReturnCode != ReturnCode.Success)
			{
				return checkArtifactsVisitor.ReturnCode;
			}

			dependeciesToBuild.AddRange(checkArtifactsVisitor.DependenciesThatNeedBuilding);
			projectsToUpdate.AddRange(checkArtifactsVisitor.ProjectsThatNeedNugetUpdate);

			_console.WriteLine();
			_console.WriteLine(strings.BRANCH_CHANGES);
			if (verifyVisitor.Changes.Count > 0)
			{
				foreach (string entry in verifyVisitor.Changes)
				{
					_console.WriteLine($"\t{entry}");
				}
			}
			else
			{
				_console.WriteLine($"\t{strings.DEPS_CORRECT_BRANCH}");
			}

			_console.WriteLine(strings.DEPENDENCIES_TO_BUILD);
			if (dependeciesToBuild.Count > 0)
			{
				foreach (string entry in dependeciesToBuild)
				{
					_console.WriteLine($"\t{entry}");
				}
			}
			else
			{
				_console.WriteLine($"\t{strings.PACKAGES_UP_TO_DATE}");
			}

			_console.WriteLine(strings.PROJECTS_TO_UPDATE);
			if (projectsToUpdate.Count > 0)
			{
				foreach (string entry in projectsToUpdate)
				{
					_console.WriteLine($"\t{entry}");
				}
			}
			else
			{
				_console.WriteLine($"\t{strings.PROJECTS_UP_TO_DATE}");
			}

			return ReturnCode.Success;
		}

		#endregion
	}
}
