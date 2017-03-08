using System;
using System.IO.Abstractions;
using System.Linq;
using GitDepend.Busi;
using GitDepend.CommandLine;
using GitDepend.Visitors;

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
                _console.WriteLine("Not all dependencies are on the correct branch.");
                return verifyVisitor.ReturnCode;
            }

            //checkArtifactsVisitor this will check to see if we are up to date with the artifacts.
            _algorithm.Reset();
            var checkArtifactsVisitor = new CheckArtifactsVisitor();
            _algorithm.TraverseDependencies(checkArtifactsVisitor, _options.Directory);

            if (checkArtifactsVisitor.ReturnCode == ReturnCode.Success)
            {
                Console.WriteLine("All packages are up to date");
                return ReturnCode.Success;
            }
            else if(checkArtifactsVisitor.ReturnCode == ReturnCode.DependencyPackagesMisMatch)
            {
                
            }
            _console.WriteLine();
            _algorithm.Reset();
            var buildAndUpdateVisitor = new BuildAndUpdateDependenciesVisitor(_options.Dependencies);
            _algorithm.TraverseDependencies(buildAndUpdateVisitor, _options.Directory);

            if (buildAndUpdateVisitor.ReturnCode == ReturnCode.Success)
            {
                if (buildAndUpdateVisitor.UpdatedPackages.Any())
                {
                    _console.WriteLine("Updated packages: ");
                    foreach (var package in buildAndUpdateVisitor.UpdatedPackages)
                    {
                        _console.WriteLine($"    {package}");
                    }
                    _console.WriteLine("Update complete!");
                }
                else
                {
                    _console.WriteLine("nothing was updated");
                }
            }

            return buildAndUpdateVisitor.ReturnCode;
        }

        #endregion
    }
}
