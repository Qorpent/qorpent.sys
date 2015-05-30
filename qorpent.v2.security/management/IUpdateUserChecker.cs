using System.Security.Principal;
using qorpent.v2.security.user;

namespace qorpent.v2.security.management
{
    public interface IUpdateUserChecker {
         UpdateResult ValidateUpdate(IIdentity actor, UserUpdateInfo update, IUser target);
    }
}
