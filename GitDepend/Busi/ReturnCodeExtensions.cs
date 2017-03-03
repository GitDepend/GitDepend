using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GitDepend.Busi
{
    /// <summary>
    /// Provides extensions for the <see cref="ReturnCode"/> enumeration.
    /// </summary>
    public static class ReturnCodeExtensions
    {
        /// <summary>
        /// Gets the resx key defined in the <see cref="ResxKeyAttribute"/> on the provided <see cref="ReturnCode"/>
        /// </summary>
        /// <param name="code">The <see cref="ReturnCode"/></param>
        /// <returns>The resx key associated with this <see cref="ReturnCode"/></returns>
        public static string GetResxKey(this ReturnCode code)
        {
            string key = null;
            Type t = code.GetType();
            MemberInfo[] members = t.GetMember(code.ToString());
            if (members.Length == 1)
            {
                object[] attrs = members[0].GetCustomAttributes(typeof(ResxKeyAttribute), false);
                if (attrs.Length == 1)
                {
                    key = ((ResxKeyAttribute)attrs[0]).Key;
                }
            }
            return key;
        }
    }
}
