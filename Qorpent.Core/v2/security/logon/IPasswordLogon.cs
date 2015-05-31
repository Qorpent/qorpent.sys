using System.Security.Principal;
using Qorpent;

namespace qorpent.v2.security.logon {
    /// <summary>
    ///     Interface for login/password login
    /// </summary>
    public interface IPasswordLogon : ILogon {
        /// <summary>
        ///     Logon with given username and passowrd
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        IIdentity Logon(string username, string password, IScope context = null);
    }
}