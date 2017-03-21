using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.CommandLine;
using GitDepend.Configuration;
using GitDepend.Visitors;
using NUnit.Framework;

namespace GitDepend.UnitTests.Visitors
{
    [TestFixture]
    public class ManageDependenciesVisitorTests : TestFixtureBase
    {
        [Test]
        public void VisitorShould_FindMatchingName()
        {
            ManageSubOptions options = new ManageSubOptions();
            options.Name = Lib1Config.Name;
            options.SetBranch = "newbranch";
            options.SetDirectory = "mydirectory";
            options.SetUrl = "myurl";

            ManageDependenciesVisitor visitor = new ManageDependenciesVisitor(options);
            visitor.VisitDependency(Lib1Directory, Lib1Dependency);
            
            Assert.IsTrue(!string.IsNullOrEmpty(visitor.NameMatchingDirectory));
        }

        [Test]
        public void ManageShouldFailIfNoDependencyMatch()
        {
            ManageSubOptions options = new ManageSubOptions();
            options.Name = Lib2Config.Name;
            options.SetBranch = "newbranch";
            options.SetDirectory = "mydirectory";
            options.SetUrl = "myurl";

            ManageDependenciesVisitor visitor = new ManageDependenciesVisitor(options);
            visitor.VisitDependency(Lib2Directory, new Dependency()
            {
                Directory = "",
                Url = "",
                Configuration = new GitDependFile(),
                Branch = ""
            });

            Assert.IsTrue(string.IsNullOrEmpty(visitor.NameMatchingDirectory));
        }
    }
}
