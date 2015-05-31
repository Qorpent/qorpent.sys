using System.Security.Principal;
using qorpent.v2.security.user;

namespace qorpent.v2.security.authorization {
    public static class RoleResolverExtensions {
        public static bool IsInRole(this IRoleResolverService service, IPrincipal principal, string role,
            bool exact = false) {
            return service.IsInRole(principal.Identity, role, exact);
        }

        public static bool IsInRole(this IRoleResolverService service, string login, string role,
            bool exact = false) {
            var srv = service as RoleResolverService;
            var us = srv.Users.GetUser(login);
            if (null == us) {
                return false;
            }
            var id = new Identity {
                Name = us.Name,
                IsAuthenticated = true,
                IsAdmin = us.IsAdmin,
                User = us
            };
            return service.IsInRole(id, role, exact);
        }
    }
}