using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDepend.Busi
{
    internal interface IVersionUpdateChecker
    {
        string CheckVersion(string packageName, string owner);
    }
}
