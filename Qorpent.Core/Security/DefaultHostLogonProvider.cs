using System.Linq;
using System.Security.Principal;
using Qorpent.IoC;

namespace Qorpent.Security {
    [ContainerComponent(Lifestyle.Singleton,ServiceType = typeof(IHostLogonProvider))]
    public class DefaultHostLogonProvider : ServiceBase, IHostLogonProvider {
        public int Idx { get; set; }

        [Inject]
        public ILoginSourceProvider LoginSourceProvider { get; set; }

        [Inject]
        public IHostLogon[] HostLogons { get; set; }

        public bool IsAuth(string username, string password, IScope scope = null) {
            var logininfo = LoginSourceProvider.Get(username);
            if (null != logininfo) {
                return logininfo.Logon(password) == LogonAuthenticationResult.Ok;
            }
            if (null == HostLogons || 0 == HostLogons.Length) return false;
            return HostLogons.OrderBy(_=>_.Idx).Any(logon => logon.IsAuth(username, password));
        }

        public IIdentity Logon(string username, string password, IScope context = null)
        {
            var logininfo = LoginSourceProvider.Get(username);
            if (null != logininfo)
            {
                if (!logininfo.Logon(password).HasFlag(LogonAuthenticationResult.Ok)) {
                    return null;
                }
                return new GenericIdentity(logininfo.Login);
            }
            if (null == HostLogons || 0 == HostLogons.Length) return null;
            return HostLogons.OrderBy(_ => _.Idx).Select(_ => _.Logon(username, password)).FirstOrDefault(_ => null != _);
        }
    }
}