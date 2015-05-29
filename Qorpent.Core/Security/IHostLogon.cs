using System;
using System.Security.Policy;
using System.Security.Principal;
using Qorpent.Model;

namespace Qorpent.Security {
    public interface IHostLogon :IWithIndex{
        /// <summary>
        ///     Execute system logon procedure and return true if proceed
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Obsolete("v2 migration")]
        bool IsAuth(string username, string password, IScope context = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        IIdentity Logon(string username, string password, IScope context =null);
    }
}