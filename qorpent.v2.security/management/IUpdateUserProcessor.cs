using System.Security.Principal;
using qorpent.v2.security.user;

namespace qorpent.v2.security.management {
    public interface IUpdateUserProcessor {
        UpdateResult[] DefineUser(IIdentity actor, UserUpdateInfo[] updateinfo, IUser target, bool store);
    }
}