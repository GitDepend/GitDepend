using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using GitDepend.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GitDepend.UnitTests
{
    [TestFixture]
    public abstract class TestFixtureBase
    {
        protected UnityContainer Container;

        protected readonly GitDependFile Lib1Config;
        protected readonly Dependency Lib1Dependency;
        protected readonly string Lib1Directory = @"C:\projects\Lib1";
        protected readonly string Lib1PackagesDirectory = @"C:\projects\Lib1\artifacts\NuGet\Debug";
        protected readonly List<string> Lib1Packages;

        protected readonly GitDependFile Lib2Config;
        protected readonly string Lib2Directory = @"C:\projects\Lib2";
        protected readonly string Lib2PackagesDirectory = @"C:\projects\Lib2\artifacts\NuGet\Debug";
        protected readonly List<string> Lib2Solutions;


        protected TestFixtureBase()
        {
            Lib1Config = new GitDependFile
            {
                Name = "Lib1",
                Build = { Script = "make.bat" },
                Packages = { Directory = "artifacts/NuGet/Debug" }
            };

            Lib1Dependency = new Dependency
            {
                Directory = "..\\Lib1",
                Url = "https://github.com/kjjuno/Lib1.git",
                Branch = "develop",
                Configuration = Lib1Config
            };

            Lib1Packages = new List<string>
            {
                "Lib1.Core.0.1.0-alpha0003.nupkg",
                "Lib1.Busi.0.1.0-alpha0003.nupkg",
                "Lib1.Data.0.1.0-alpha0003.nupkg"
            };

            Lib2Config = new GitDependFile
            {
                Name = "Lib2",
                Build = { Script = "make.bat" },
                Packages = { Directory = "artifacts/NuGet/Debug" },
                Dependencies = { Lib1Dependency }
            };

            Lib2Solutions = new List<string>
            {
                "Lib2.sln",
                "Lib2.UnitTests.sln"
            };
        }

        [SetUp]
        public void SetUp()
        {
            Container = new UnityContainer();
            Container.EnableMocking();
            DependencyInjection.Container = Container;
        }

        protected IFileSystem RegisterMockFileSystem()
        {
            var fileSystem = new MockFileSystem();
            Container.RegisterType<IFileSystem, MockFileSystem>(new InjectionFactory(c => fileSystem));
            return fileSystem;
        }

        protected void EnsureDirectory(IFileSystem fileSystem, string directory)
        {
            if (!fileSystem.Directory.Exists(directory))
            {
                fileSystem.Directory.CreateDirectory(directory);
            }
        }

        protected void EnsureFiles(IFileSystem fileSystem, string baseDirectory, List<string> files)
        {
            EnsureDirectory(fileSystem, baseDirectory);

            foreach (var file in files)
            {
                var path = fileSystem.Path.Combine(baseDirectory, file);

                var dir = fileSystem.Path.GetDirectoryName(path);

                EnsureDirectory(fileSystem, dir);

                fileSystem.File.WriteAllBytes(path, new byte[] { 0x01, 0x02, 0x03 });
            }
        }
    }
}
