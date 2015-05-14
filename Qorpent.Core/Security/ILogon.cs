using System.Security.Principal;
using Qorpent.Model;

namespace Qorpent.Security {
    public interface ILogon :IWithIndex{
        /// <summary>
        ///     Execute system logon procedure and return true if proceed
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool IsAuth(string username, string password);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        IIdentity Logon(string username, string password);
    }
}