using Qorpent;
using Qorpent.IoC;
using Qorpent.Security;

namespace qorpent.v2.security.logon {
    [ContainerComponent(Lifestyle.Singleton, "hostlogon.v2", ServiceType = typeof (IHostLogonProvider))]
    public partial class LogonService : IHostLogonProvider {
        /// <summary>
        ///     //TODO: deprecated method, logon should return only identities
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        bool IHostLogon.IsAuth(string username, string password, IScope context) {
            return Logon(username, password, context).IsAuthenticated;
        }
    }
}