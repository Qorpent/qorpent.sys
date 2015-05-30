using System;
using System.Linq;
using System.Security.Principal;
using qorpent.v2.security.user;
using qorpent.v2.security.user.storage;
using Qorpent;
using Qorpent.IoC;

namespace qorpent.v2.security.authorization {
    [ContainerComponent(Lifestyle.Singleton,"userbased.roleresolver",ServiceType=typeof(IRoleResolver))]
    public class RoleResolver : ServiceBase, IRoleResolver {

        [Inject]
        public IUserService Users { get; set; }

        public bool IsInRole(IIdentity identity, string role, bool exact) {
            var id = identity as Identity;
            if (null == id) {
                throw new Exception("only qorpent identities are allowed");
            }
            var user = id.User ?? (id.User = Users.GetUser(id.Name));
            if (HasRole(user, role)) {
                return true;
            }
            if (exact) return false;
            if (!string.IsNullOrWhiteSpace(user.Domain)) {
                var master = Users.GetUser(user.Domain + "@groups");
                if (HasRole(master, role)) {
                    return true;
                }
            }
            foreach (var grp in user.Groups) {
                var g = Users.GetUser(grp + "@groups");
                if (HasRole(g, role)) {
                    return true;
                }
            }
            return false;
        }

        private static bool HasRole(IUser user, string role) {
            if (null == user) return false;
            return user.Roles.Any(_ => _.Equals(role, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}