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
    /// An implementation of <see cref="ICommand"/> that recursively updates all dependencies.
    /// </summary>
    public class UpdateCommand : ICommand
    {
        private readonly UpdateSubOptions _options;
        private readonly IConsole _console;
        private readonly IDependencyVisitorAlgorithm _algorithm;

        /// <summary>
        /// The verb name
        /// </summary>
        public const string Name = "update";

        /// <summary>
        /// Creates a new <see cref="UpdateCommand"/>
        /// </summary>
        /// <param name="options">The <see cref="UpdateSubOptions"/> that configure the command.</param>
        public UpdateCommand(UpdateSubOptions options)
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
            var verifyVisitor = new VerifyCorrectBranchVisitor(_options.Dependencies);
            _algorithm.TraverseDependencies(verifyVisitor, _options.Directory);

            if (verifyVisitor.ReturnCode != ReturnCode.Success)
            {
                _console.WriteLine(strings.NOT_ALL_DEPS_CORRECT_BRANCH);
                return verifyVisitor.ReturnCode;
            }

            //checkArtifactsVisitor this will check to see if we are up to date with the artifacts.
            _algorithm.Reset();
            var checkArtifactsVisitor = new CheckArtifactsVisitor();
            _algorithm.TraverseDependencies(checkArtifactsVisitor, _options.Directory);

            if (checkArtifactsVisitor.ReturnCode == ReturnCode.Success && checkArtifactsVisitor.UpToDate)
            {
                Console.WriteLine("All packages are up to date");
                return ReturnCode.Success;
            }
            if(checkArtifactsVisitor.ReturnCode != ReturnCode.Success)
            {
                return checkArtifactsVisitor.ReturnCode;
            }

            var needToBuild = new HashSet<string>();
            needToBuild.AddRange(checkArtifactsVisitor.DependenciesThatNeedBuilding);
            needToBuild.AddRange(checkArtifactsVisitor.ProjectsThatNeedNugetUpdate);

            _console.WriteLine();
            _algorithm.Reset();
            var buildAndUpdateVisitor = new BuildAndUpdateDependenciesVisitor(needToBuild.ToList());
            _algorithm.TraverseDependencies(buildAndUpdateVisitor, _options.Directory);

            if (buildAndUpdateVisitor.ReturnCode == ReturnCode.Success)
            {
                if (buildAndUpdateVisitor.UpdatedPackages.Any())
                {
                    _console.WriteLine(strings.UPDATED_PACKAGES);
                    foreach (var package in buildAndUpdateVisitor.UpdatedPackages)
                    {
                        _console.WriteLine($"    {package}");
                    }
                    _console.WriteLine(strings.UPDATE_COMPLETE);
                }
                else
                {
                    _console.WriteLine(strings.NOTHING_UPDATED);
                }
            }

            return buildAndUpdateVisitor.ReturnCode;
        }

        #endregion
    }
}
