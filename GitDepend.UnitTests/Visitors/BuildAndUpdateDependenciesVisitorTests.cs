using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using GitDepend.Busi;
using GitDepend.Configuration;
using GitDepend.Visitors;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests.Visitors
{
    [TestFixture]
    public class BuildAndUpdateDependenciesVisitorTests : TestFixtureBase
    {
        [Test]
        public void VisitDependency_ShouldReturn_GitDependFileNotFound_WhenUnableToLoadConfigFile()
        {
            var factory = Container.Resolve<IGitDependFileFactory>();
            var instance = new BuildAndUpdateDependenciesVisitor(new List<string>(), new List<string>());

            string dir;
            ReturnCode loadCode = ReturnCode.GitRepositoryNotFound;
            factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode))
                .Returns(null as GitDependFile);

            var code = instance.VisitDependency(Lib2Directory, Lib1Dependency);
            Assert.AreEqual(ReturnCode.GitRepositoryNotFound, code, "Invalid ReturnCode");
            Assert.AreEqual(ReturnCode.GitRepositoryNotFound, instance.ReturnCode, "Invalid ReturnCode");
        }

        [Test]
        public void VisitDependency_ShouldRunBuildScript()
        {
            var factory = Container.Resolve<IGitDependFileFactory>();
            var processManager = Container.Resolve<IProcessManager>();
            var fileSystem = RegisterMockFileSystem();
            var instance = new BuildAndUpdateDependenciesVisitor(new List<string> { Lib1Config.Name }, new List<string> { Lib1Config.Name, Lib2Config.Name });

            string dir;
            ReturnCode loadCode = ReturnCode.Success;
            factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode))
                .Returns(Lib1Config);

            EnsureDirectory(fileSystem, Lib1Directory);
            EnsureDirectory(fileSystem, Lib2Directory);
            EnsureFiles(fileSystem, Lib1Directory, new List<string>
            {
                "make.bat"
            });

            bool scriptExecuted = false;
            processManager.Arrange(m => m.Start(Arg.IsAny<ProcessStartInfo>()))
                .Returns((ProcessStartInfo info) =>
                {
                    scriptExecuted = true;
                    EnsureFiles(fileSystem, Lib1PackagesDirectory, Lib1Packages);

                    return new FakeProcess
                    {
                        ExitCode = 0,
                        HasExited = true
                    };
                });
            

            var code = instance.VisitDependency(Lib2Directory, Lib1Dependency);

            var cachedPackages = fileSystem.Directory.GetFiles(instance.GetCacheDirectory(), "*.nupkg");

            Assert.AreEqual(Lib1Packages.Count, cachedPackages.Length, "Invalid number of cached nuget packages");

            foreach (var package in cachedPackages)
            {
                var name = fileSystem.Path.GetFileName(package);
                // Make sure that name shows up in the original list.
                Assert.IsTrue(Lib1Packages.Any(p => fileSystem.Path.GetFileName(p) == name));
            }

            Assert.IsTrue(scriptExecuted, "Build Script was not executed");
            Assert.AreEqual(ReturnCode.Success, code, "Invalid ReturnCode");
            Assert.AreEqual(ReturnCode.Success, instance.ReturnCode, "Invalid ReturnCode");
        }

        [Test]
        public void VisitDependency_ShouldReturn_CouldNotCreateCacheDirectory_IfCacheDirectoryFailsToCreate()
        {
            var factory = Container.Resolve<IGitDependFileFactory>();

            string dir;
            ReturnCode loadCode = ReturnCode.Success;
            factory.Arrange(f => f.LoadFromDirectory(Arg.AnyString, out dir, out loadCode))
                .Returns(Lib1Config);

            var fileSystem = Container.Resolve<IFileSystem>();
            fileSystem.Directory.Arrange(d => d.CreateDirectory(Arg.AnyString))
                .Throws<IOException>("Access Denied");

            var instance = new BuildAndUpdateDependenciesVisitor(new List<string>(), new List<string>());

            var code = instance.VisitDependency(Lib2Directory, Lib1Dependency);
            Assert.AreEqual(ReturnCode.CouldNotCreateCacheDirectory, code, "Invalid ReturnCode");
            Assert.AreEqual(ReturnCode.CouldNotCreateCacheDirectory, instance.ReturnCode, "Invalid ReturnCode");
        }

        [Test]
        public void VisitProject_ShouldReturn_DirectoryDoesNotExist_WhenDirectoryIsNull()
        {
            var instance = new BuildAndUpdateDependenciesVisitor(new List<string> { Lib1Config.Name }, new List<string> { Lib1Config.Name, Lib2Config.Name });
            var code = instance.VisitProject(null, Lib2Config);
            Assert.AreEqual(ReturnCode.DirectoryDoesNotExist, code, "Invalid Return Code");
        }

        [Test]
        public void VisitProject_ShouldReturn_DirectoryDoesNotExist_WhenDirectoryDoesNotExist()
        {
            var instance = new BuildAndUpdateDependenciesVisitor(new List<string> { Lib1Config.Name }, new List<string> { Lib1Config.Name, Lib2Config.Name });
            var code = instance.VisitProject(Lib2Directory, Lib2Config);
            Assert.AreEqual(ReturnCode.DirectoryDoesNotExist, code, "Invalid Return Code");
        }

        [Test]
        public void VisitProject_ShouldReturn_Success_WhenConfigIsNull()
        {
            var instance = new BuildAndUpdateDependenciesVisitor(new List<string>(), new List<string>());
            var code = instance.VisitProject(Lib2Directory, null);
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual(ReturnCode.Success, instance.ReturnCode, "Invalid Return Code");
        }

        [Test]
        public void VisitProject_ShouldCallNugetUpdate_ForEachNugetPackageInDependencyArtifactsFolder()
        {
            var fileSystem = RegisterMockFileSystem();
            var nuget = Container.Resolve<INuget>();
            var instance = new BuildAndUpdateDependenciesVisitor(new List<string> { Lib1Config.Name }, new List<string> { Lib1Config.Name, Lib2Config.Name });

            EnsureFiles(fileSystem, Lib1PackagesDirectory, Lib1Packages);
            EnsureFiles(fileSystem, Lib1PackagesDirectory, new List<string>
            {
                ".nupkg",
                "bad.input.nupkg"
            });

            EnsureFiles(fileSystem, Lib2Directory, Lib2Solutions);
            Dictionary<string, List<string>> updatedPackages = new Dictionary<string, List<string>>();

            nuget.Arrange(n => n.Update(Arg.AnyString, Arg.AnyString, Arg.AnyString, Arg.AnyString))
                .Returns((string solution, string id, string version, string sourceDirectory) =>
                {
                    Assert.AreEqual(Lib2Directory, nuget.WorkingDirectory, "Invalid working directory for Nuget.exe");

                    var key = $"{id}.{version}";
                    if (updatedPackages.ContainsKey(key))
                    {
                        var list = updatedPackages[key];

                        Assert.IsFalse(list.Contains(solution));
                        list.Add(solution);
                    }
                    else
                    {
                        updatedPackages.Add(key, new List<string> { solution });
                    }
                    return ReturnCode.Success;
                });

            fileSystem.Directory.CreateDirectory(Lib2Directory);

            var code = instance.VisitProject(Lib2Directory, Lib2Config);
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual(ReturnCode.Success, instance.ReturnCode, "Invalid Return Code");

            Assert.AreEqual(Lib1Packages.Count, updatedPackages.Keys.Count, "Invalid number of updated packages");

            foreach (var kvp in updatedPackages)
            {
                Assert.IsTrue(Lib1Packages.Any(p => fileSystem.Path.GetFileNameWithoutExtension(p) == kvp.Key));

                foreach (var solution in kvp.Value)
                {
                    Assert.IsTrue(Lib2Solutions.Any(s => s == fileSystem.Path.GetFileName(solution)), $"{solution} was not found in Lib2Solutions");
                }
            }
        }

        [Test]
        public void VisitProject_ShouldReturn_CouldNotCreateCacheDirectory_WhenCacheDirectoryFailsToInitialize()
        {
            var fileSystem = Container.Resolve<IFileSystem>();
            fileSystem.Directory.Arrange(d => d.Exists(Arg.AnyString)).Returns(true);
            fileSystem.Directory.Arrange(d => d.GetFiles(Arg.AnyString, "*.nupkg"))
                .Returns(Lib1Packages.ToArray());
            fileSystem.Directory.Arrange(d => d.GetFiles(Arg.AnyString, "*.sln", Arg.IsAny<SearchOption>()))
                .Returns(Lib2Solutions.Select(s => Lib2Directory + "\\" + s).ToArray());

            fileSystem.Path.Arrange(p => p.GetFileNameWithoutExtension(Arg.AnyString))
                .Returns(Path.GetFileNameWithoutExtension);

            fileSystem.Directory.Arrange(d => d.CreateDirectory(Arg.AnyString))
                .Throws<IOException>("Access Denied");

            var instance = new BuildAndUpdateDependenciesVisitor(new List<string> { Lib1Config.Name }, new List<string> { Lib1Config.Name, Lib2Config.Name });

            var code = instance.VisitProject(Lib2Directory, Lib2Config);
            Assert.AreEqual(ReturnCode.CouldNotCreateCacheDirectory, code, "Invalid Return Code");
            Assert.AreEqual(ReturnCode.CouldNotCreateCacheDirectory, instance.ReturnCode, "Invalid Return Code");
        }

        [Test]
        public void VisitProject_ShouldCallNugetRestoreBeforeCallingNuGetUpdate()
        {
            var fileSystem = RegisterMockFileSystem();
            var nuget = Container.Resolve<INuget>();
            var instance = new BuildAndUpdateDependenciesVisitor(new List<string>(), new List<string>());

            EnsureFiles(fileSystem, Lib1PackagesDirectory, Lib1Packages);
            EnsureFiles(fileSystem, Lib2Directory, Lib2Solutions);

            bool nugetRestoreCalled = false;
            nuget.Arrange(n => n.Restore(Arg.AnyString))
                .Returns((string solution) =>
                {
                    nugetRestoreCalled = true;
                    return ReturnCode.Success;
                });

            nuget.Arrange(n => n.Update(Arg.AnyString, Arg.AnyString, Arg.AnyString, Arg.AnyString))
                .Returns((string solution, string id, string version, string sourceDirectory) =>
                {
                    Assert.IsTrue(nugetRestoreCalled, "NuGet.exe restore should have been called before Update");
                    return ReturnCode.Success;
                });

            fileSystem.Directory.CreateDirectory(Lib2Directory);

            var code = instance.VisitProject(Lib2Directory, Lib2Config);
            Assert.AreEqual(ReturnCode.Success, code, "Invalid Return Code");
            Assert.AreEqual(ReturnCode.Success, instance.ReturnCode, "Invalid Return Code");
        }
    }
}
