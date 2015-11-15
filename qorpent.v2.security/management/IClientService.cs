using System;
using System.Runtime.Remoting;
using System.Security.Principal;

namespace qorpent.v2.security.management
{
    /// <summary>
    /// Service for standard client registration
    /// </summary>
    public interface IClientService {
        /// <summary>
        /// Initializes group and main user for group
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        ClientResult Init(IIdentity caller, InitClientRecord record);
        /// <summary>
        /// Move client from demo to working group
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="clientSysName"></param>
        /// <returns></returns>
        ClientResult ToWork(IIdentity caller, string clientSysName);
        /// <summary>
        /// Move client to demo group (+ reset expiration)
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="clientSysName"></param>
        /// <returns></returns>
        ClientResult ToDemo(IIdentity caller, string clientSysName);
        /// <summary>
        /// Set new expire for client
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="clientSysName"></param>
        /// <param name="newExpire"></param>
        /// <returns></returns>
        ClientResult SetExpire(IIdentity caller, string clientSysName, DateTime newExpire);
    }
}
