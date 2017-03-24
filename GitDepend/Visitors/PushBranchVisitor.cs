using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDepend.Configuration;

namespace GitDepend.Visitors
{
    /// <summary>
    /// 
    /// </summary>
    public class PushBranchVisitor : NamedDependenciesVisitor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="whitelist"></param>
        public PushBranchVisitor(IList<string> whitelist) : base(whitelist)
        {
        }

        /// <summary>
        /// Provides the custom hook for VisitDependency. This will only be called if the dependency
        /// was specified in the whitelist.
        /// </summary>
        /// <param name="directory">The directory of the project.</param>
        /// <param name="dependency">The <see cref="Dependency"/> to visit.</param>
        /// <returns></returns>
        protected override ReturnCode OnVisitDependency(string directory, Dependency dependency)
        {
            throw new NotImplementedException();
        }
    }
}
