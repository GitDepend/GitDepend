using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    /// 
    /// </summary>
    /// <seealso cref="GitDepend.Visitors.IVisitor" />
    public class CheckArtifactsVisitor : IVisitor
    {
        private const string NUMBERS = "0123456789";
        private const string ALPHABET = "abcdefghijklmnopqrstuvwxyz";

        private readonly IFileSystem _fileSystem;
        private Dictionary<string, string> _dependencyPackageNamesAndVersions;
        private const int EMPTY = 0;
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckArtifactsVisitor"/> class.
        /// </summary>
        public CheckArtifactsVisitor()
        {
            _fileSystem = DependencyInjection.Resolve<IFileSystem>();
            _dependencyPackageNamesAndVersions = new Dictionary<string, string>();
        }
        
        /// <summary>
        /// The return code
        /// </summary>
        public ReturnCode ReturnCode { get; set; }

        /// <summary>
        /// Visits a project dependency.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="dependency">The <see cref="Dependency" /> to visit.</param>
        /// <returns>
        /// The return code.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public ReturnCode VisitDependency(string directory, Dependency dependency)
        {
            //read in the nuget packages created in the artifacts folder
            var nugetPackageFiles = _fileSystem.Directory.EnumerateFiles(directory + '/' + dependency.Configuration.Packages.Directory);
            //get the versionNumbers, they come in the format of {ProjectName}.{version#}.nupkg
            var directoryLess = nugetPackageFiles.Select(file => Path.GetFileName(file));

            var extensionLess = directoryLess.Select(file => file.Replace(".nupkg", "")).ToList();

            foreach (var file in extensionLess)
            {
                string versionNumber, packageName;
                GetPackageNameAndVersion(file, out versionNumber, out packageName);
                if (!_dependencyPackageNamesAndVersions.ContainsKey(packageName))
                {
                    _dependencyPackageNamesAndVersions.Add(packageName, versionNumber);
                }
            }

            if (_dependencyPackageNamesAndVersions.Count == EMPTY)
            {
                return ReturnCode.DependencyPackagesNotBuilt;
            }

            return ReturnCode.Success;
        }
        
        /// <summary>
        /// Visists a project.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="config">The <see cref="GitDependFile" /> with project configuration information.</param>
        /// <returns>
        /// The return code.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public ReturnCode VisitProject(string directory, GitDependFile config)
        {
            var packagesFiles = _fileSystem.Directory.GetFiles(directory, "packages.config", SearchOption.AllDirectories);

            //build a dictionary of current packages that need to be checked.
            var neededPackages = GetPackagesFromPackagesConfigFiles(packagesFiles);
            //check only the ones that match
            var misMatching = CheckMatches(neededPackages);

            if (misMatching.Count != 0)
            {
                return ReturnCode.DependencyPackagesMisMatch;
            }

            return ReturnCode.Success;
        }
        
        private void GetPackageNameAndVersion(string file, out string versionNumber, out string packageName)
        {

            string afterHyphen = file.IndexOf('-') > 0 ? file.Substring(file.IndexOf('-')) : "";
            string hyphenLess = file.Substring(0, file.IndexOf('-'));
            int indexOfNumber = hyphenLess.IndexOfAny(NUMBERS.ToCharArray());
            string subPackage = hyphenLess.Substring(0, indexOfNumber);
            string subVersion = hyphenLess.Substring(indexOfNumber);

            var alphabet = ALPHABET.ToCharArray();
            var numbers = NUMBERS.ToCharArray();
            //check subversion for characters
            int indexOfChar = subVersion.IndexOfAny(alphabet);
            var hasCharacters = indexOfChar >= 0;
            while (hasCharacters)
            {
                if (hasCharacters)
                {
                    indexOfNumber = subVersion.IndexOfAny(numbers);
                    if (indexOfNumber == 0)
                    {
                        indexOfNumber++;
                    }
                    subPackage = subPackage + subVersion.Substring(0, indexOfNumber);
                    subVersion = subVersion.Substring(indexOfNumber);
                }
                indexOfChar = subVersion.IndexOfAny(alphabet);
                hasCharacters = indexOfChar >= 0;
            }
            packageName = subPackage.Substring(0, subPackage.LastIndexOf('.'));
            versionNumber = subVersion + afterHyphen;

        }

        private Dictionary<string, string> GetPackagesFromPackagesConfigFiles(string[] packagesFiles)
        {
            var packagesDictionary = new Dictionary<string, string>();
            foreach (var packageFile in packagesFiles)
            {
                
                foreach(var reference in GetPackageReferences(packageFile))
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
