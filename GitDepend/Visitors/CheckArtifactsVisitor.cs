using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using GitDepend.Configuration;
using Microsoft.Practices.ObjectBuilder2;
using NuGet;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace GitDepend.Visitors
{
    /// <summary>
    /// Checks to see if artifacts are up to date for all dependencies.
    /// </summary>
    public class CheckArtifactsVisitor : NamedDependenciesVisitor
    {
        private readonly bool _force;
        private static readonly Regex NugetPackageRegex = new Regex(@"^(?<id>.*?)\.(?<version>(?:\d\.){2,3}\d(?:-.*?)?)$", RegexOptions.Compiled);

        private readonly IFileSystem _fileSystem;
        private readonly Dictionary<string, string> _dependencyPackageNamesAndVersions;
        private const int EMPTY = 0;

        /// <summary>
        /// Contains a list of the name of the dependencies that need building
        /// </summary>
        public HashSet<string> DependenciesThatNeedBuilding { get; set; }

        /// <summary>
        /// The projects that need nuget update
        /// </summary>
        public HashSet<string> ProjectsThatNeedNugetUpdate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether dependencies and projects are [up to date].
        /// </summary>
        public bool UpToDate => DependenciesThatNeedBuilding.IsEmpty() && ProjectsThatNeedNugetUpdate.IsEmpty();

        /// <summary>
        /// Creates a new <see cref="DisplayStatusVisitor"/>
        /// </summary>
        /// <param name="whitelist">The projects to visit. If this list is null or empty all projects will be visited.</param>
        /// <param name="force">Should all named dependencies be forced to build?</param>
        public CheckArtifactsVisitor(IList<string> whitelist, bool force) : base(whitelist)
        {
            _force = force;
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
            _dependencyPackageNamesAndVersions = new Dictionary<string, string>();
            DependenciesThatNeedBuilding = new HashSet<string>();
            ProjectsThatNeedNugetUpdate = new HashSet<string>();
        }

        #region Overrides of NamedDependenciesVisitor

        /// <summary>
        /// Provides the custom hook for VisitDependency. This will only be called if the dependency
        /// was specified in the whitelist.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="dependency">The <see cref="Dependency"/> to visit.</param>
        /// <returns></returns>
        protected override ReturnCode OnVisitDependency(string directory, Dependency dependency)
        {
            if (_force)
            {
                DependenciesThatNeedBuilding.Add(dependency.Configuration.Name);
                return ReturnCode.Success;
            }

            string path = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(dependency.Directory, dependency.Configuration.Packages.Directory));

            //read in the nuget packages created in the artifacts folder
            var localArtifacts = new Dictionary<string, string>();
            if (_fileSystem.Directory.Exists(path))
            {
                var nugetPackageFiles = _fileSystem.Directory.EnumerateFiles(path);
                //get the versionNumbers, they come in the format of {ProjectName}.{version#}.nupkg
                var directoryLess = nugetPackageFiles.Select(file => _fileSystem.Path.GetFileName(file));

                var extensionLess = directoryLess.Select(file => file.Replace(".nupkg", "")).ToList();

                foreach (var file in extensionLess)
                {
                    string versionNumber, packageName;

                    GetPackageNameAndVersion(file, out versionNumber, out packageName);
                    if (!_dependencyPackageNamesAndVersions.ContainsKey(packageName))
                    {
                        _dependencyPackageNamesAndVersions.Add(packageName, versionNumber);
                        localArtifacts.Add(packageName, versionNumber);
                    }
                }
            }

            if (localArtifacts.IsEmpty())
            {
                DependenciesThatNeedBuilding.Add(dependency.Configuration.Name);
            }
            else
            {
                if (ProjectsThatNeedNugetUpdate.Contains(dependency.Configuration.Name) ||
                    dependency.Configuration.Dependencies.Any(dep => ProjectsThatNeedNugetUpdate.Contains(dep.Configuration.Name)))
                {
                    DependenciesThatNeedBuilding.Add(dependency.Configuration.Name);
                }
            }

            return ReturnCode.Success;
        }

        /// <summary>
        /// Visits a project.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="config">The <see cref="GitDependFile"/> with project configuration information.</param>
        /// <returns>The return code.</returns>
        public override ReturnCode VisitProject(string directory, GitDependFile config)
        {
            if (_force)
            {
                ProjectsThatNeedNugetUpdate.Add(config.Name);
                return ReturnCode.Success;
            }

            var packagesFiles = _fileSystem.Directory.GetFiles(directory, "packages.config", SearchOption.AllDirectories);

            //build a dictionary of current packages that need to be checked.
            var neededPackages = GetPackagesFromPackagesConfigFiles(packagesFiles);
            //check only the ones that match
            var misMatching = CheckMatches(neededPackages);

            if (misMatching.Any() || DependenciesThatNeedBuilding.Any(d => config.Dependencies.Any(d2 => d2.Configuration.Name == d)))
            {
                ProjectsThatNeedNugetUpdate.Add(config.Name);
                DependenciesThatNeedBuilding.Add(config.Name);
            }

            return ReturnCode.Success;
        }

        #endregion

        private void GetPackageNameAndVersion(string file, out string versionNumber, out string packageName)
        {
            var match = NugetPackageRegex.Match(file);
            packageName = match.Groups["id"].Value;
            versionNumber = match.Groups["version"].Value;
        }

        private Dictionary<string, string> GetPackagesFromPackagesConfigFiles(string[] packagesFiles)
        {
            var packagesDictionary = new Dictionary<string, string>();
            foreach (var packageFile in packagesFiles)
            {

                foreach (var reference in GetPackageReferences(packageFile))
                {
                    if (!packagesDictionary.ContainsKey(reference.Id))
                    {
                        packagesDictionary.Add(reference.Id, reference.Version);
                    }
                }
            }
            return packagesDictionary;
        }

        private Dictionary<string, Tuple<string, string>> CheckMatches(Dictionary<string, string> neededPackages)
        {
            var misMatching = new Dictionary<string, Tuple<string, string>>();
            foreach (var package in _dependencyPackageNamesAndVersions)
            {
                if (neededPackages.ContainsKey(package.Key))
                {
                    if (neededPackages[package.Key] != package.Value)
                    {
                        misMatching.Add(package.Key, new Tuple<string, string>(package.Value, neededPackages[package.Key]));
                    }
                }
            }

            return misMatching;
        }


        IEnumerable<NugetPackageReference> GetPackageReferences(string filePath)
        {
            var references = new List<NugetPackageReference>();
            var document = GetDocument(filePath);

            if (document == null)
            {
                return null;
            }

            foreach (var reference in document.Root.Elements("package"))
            {
                references.Add(new NugetPackageReference
                {
                    Id = reference.GetOptionalAttributeValue("id"),
                    Version = reference.GetOptionalAttributeValue("version"),
                    TargetFramework = reference.GetOptionalAttributeValue("targetFramework")
                });
            }
            return references;
        }

        private XDocument GetDocument(string filePath)
        {
            return XDocument.Parse(_fileSystem.File.ReadAllText(filePath));
        }
    }
}
