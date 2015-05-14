using System.Linq;
using System.Security.Principal;
using Qorpent.IoC;

namespace Qorpent.Security {
    [ContainerComponent(Lifestyle.Transient,ServiceType = typeof(ILogonProvider))]
    public class DefaultLogonProvider : ServiceBase, ILogonProvider {
        public int Idx { get; set; }

        [Inject]
        public ILogon[] Logons { get; set; }

        public bool IsAuth(string username, string password) {
            if (null == Logons || 0 == Logons.Length) return false;
            return Logons.OrderBy(_=>_.Idx).Any(logon => logon.IsAuth(username, password));
        }

        public IIdentity Logon(string username, string password)
        {
            if (null == Logons || 0 == Logons.Length) return null;
            return Logons.OrderBy(_ => _.Idx).Select(_ => _.Logon(username, password)).FirstOrDefault(_ => null != _);
        }
    }
}