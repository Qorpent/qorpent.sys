using qorpent.v2.security.user;
using Qorpent;

namespace qorpent.v2.security.messaging {
    public interface IUserMessagingService {
        PostMessage SendWelcome(IUser target);
        PostMessage SendPasswordReset(IUser target);
        PostMessage SendTemplated(IUser target, string template, string from, string subject, IScope scope);
    }
}