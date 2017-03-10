using System.IO.Abstractions;
using GitDepend.Visitors;
using NUnit.Framework;

namespace GitDepend.UnitTests.Visitors
{
    [TestFixture]
    public class CheckArtifactsVisitorTests : TestFixtureBase
    {

        private IFileSystem _fileSystem;
        [SetUp]
        public void Setup()
        {
            _fileSystem = RegisterMockFileSystem();
            EnsureDirectory(_fileSystem, Lib1Directory);
            EnsureDirectory(_fileSystem, Lib2Directory);
            EnsureDirectory(_fileSystem, Lib1PackagesDirectory);
            EnsureDirectory(_fileSystem, Lib2PackagesDirectory);
        }

        [Test]
        public void ArtifactsUpToDate_ReturnsSuccess()
        {
            EnsureFiles(_fileSystem, Lib1PackagesDirectory, Lib1Packages);

            var visitor = new CheckArtifactsVisitor();
            var code = visitor.VisitDependency(Lib1Directory, Lib1Dependency);

            Assert.AreEqual(ReturnCode.Success, code);
        }

        [Test]
        public void ArtifactsDontExist_FalseUpToDate()
        {
            var visitor = new CheckArtifactsVisitor();
            var code = visitor.VisitDependency(Lib1Directory, Lib1Dependency);

            Assert.AreEqual(ReturnCode.Success, code);
            Assert.AreEqual(false, visitor.UpToDate);
        }

        [Test]
        public void ArtifactsOutOfDate_LowerVersion_FalseUpToDate()
        {
        
            const string LOWER_VERSION = "0002";
            EnsureFiles(_fileSystem, Lib1PackagesDirectory, Lib1Packages);
            var visitor = new CheckArtifactsVisitor();
            visitor.VisitDependency(Lib1Directory, Lib1Dependency);
            
            _fileSystem.File.WriteAllText(Lib2Directory +  "\\packages.config", CreateNugetFile(LOWER_VERSION));

            var code = visitor.VisitProject(Lib2Directory, Lib2Config);

            Assert.AreEqual(ReturnCode.Success, code);
            Assert.AreEqual(false, visitor.UpToDate);
        }

        [Test]
        public void ArtifactsOutOfDate_HigherVersion_FalseUpToDate()
        {
            const string HIGHER_VERSION = "0481";
            EnsureFiles(_fileSystem, Lib1PackagesDirectory, Lib1Packages);
            var visitor = new CheckArtifactsVisitor();
            visitor.VisitDependency(Lib1Directory, Lib1Dependency);

            _fileSystem.File.WriteAllText(Lib2Directory + "\\packages.config", CreateNugetFile(HIGHER_VERSION));

            var code = visitor.VisitProject(Lib2Directory, Lib2Config);

            Assert.AreEqual(ReturnCode.Success, code);
            Assert.AreEqual(false, visitor.UpToDate);
        }

        [Test]
        public void ArtifactsFolderMissing_DoesntThrowException_AndMarksOutOfDate_NeedsToBuild()
        {
            _fileSystem.Directory.Delete(Lib1PackagesDirectory + @"..\..\..\..\", true);
            var visitor = new CheckArtifactsVisitor();
            var code = visitor.VisitDependency(Lib1Directory, Lib1Dependency);

            Assert.AreEqual(ReturnCode.Success, code);
            Assert.AreEqual(visitor.UpToDate, false);
            Assert.Greater(visitor.DependenciesThatNeedBuilding.Count, 0);
        }

        private string CreateNugetFile(string alphaVersion)
        {
            string nugetFile = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<packages>
    <package id=""Lib1.Core"" version=""0.1.0-alpha{alphaVersion}.nupkg"" targetFramework=""net461"" />
    <package id=""Lib1.Busi"" version=""0.1.0-alpha{alphaVersion}.nupkg"" targetFramework=""net461"" />
    <package id=""Lib1.Data"" version=""0.1.0-alpha{alphaVersion}.nupkg"" targetFramework=""net461"" />
</packages>";

            return nugetFile;
        }
    }
}
