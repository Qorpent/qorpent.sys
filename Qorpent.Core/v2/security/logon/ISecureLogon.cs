using System.Security.Principal;
using qorpent.v2.security.logon.services;
using Qorpent;

namespace qorpent.v2.security.logon {
    /// <summary>
    ///     Interface for salt - sign -check login
    /// </summary>
    public interface ISecureLogon : ILogon {
        /// <summary>
        ///     Second phase - verify salt hash pair with given username as logon
        /// </summary>
        /// <param name="username"></param>
        /// <param name="salt"></param>
        /// <param name="hash"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        IIdentity Logon(string username, SecureLogonInfo info, IScope context = null);

        /// <summary>
        ///     First phase - user requests salt to verify it with own key
        /// </summary>
        /// <param name="username"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        string GetSalt(string username, IScope context = null);
    }
}