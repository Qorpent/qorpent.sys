using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using qorpent.v2.security.user;
using Qorpent;

namespace qorpent.v2.security.messaging {
    public interface IUserMessagingService {
        void SendWelcome(IUser target);
        void SendPasswordReset(IUser target);
        void SendTemplated(IUser target, string template, string from, string subject, IScope scope);
    }
}