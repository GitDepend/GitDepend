using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Configuration;

namespace GitDepend.Busi
{
    /// <summary>
    /// An extenstions class that proivies methods for <see cref="Dependency"/> objects.
    /// </summary>
    public static class DependencyExtensions
    {
        /// <summary>
        /// Updates the config file
        /// </summary>
        /// <param name="dependency">The <see cref="Dependency"/> object.</param>
        /// <param name="directory">The projects directory.</param>
        /// <returns></returns>
        public static ReturnCode SyncConfigWithCurrentBranch(this Dependency dependency, string directory)
        {
            var factory = DependencyInjection.Resolve<IGitDependFileFactory>();
            var fileSystem = DependencyInjection.Resolve<IFileSystem>();
            var git = DependencyInjection.Resolve<IGit>();

            string dir;
            ReturnCode code;
            var config = factory.LoadFromDirectory(directory, out dir, out code);

            if (code != ReturnCode.Success)
            {
                return code;
            }

            bool dirty = false;
            var dep = config.Dependencies.FirstOrDefault(d => d.Configuration.Name == dependency.Configuration.Name);

            if (dep != null)
            {
                var path = fileSystem.Path.GetFullPath(fileSystem.Path.Combine(directory, dep.Directory));
                git.WorkingDirectory = path;
                var branch = git.GetCurrentBranch();

                if (dep.Branch != branch)
                {
                    dep.Branch = branch;
                    dirty = true;
                    Console.WriteLine($"using {dep.Branch} for {dep.Configuration.Name}");
                }

                if (dirty)
                {
                    fileSystem.File.WriteAllText(fileSystem.Path.Combine(directory, "GitDepend.json"), config.ToString());
                }
            }

            return ReturnCode.Success;
        }
    }
}
