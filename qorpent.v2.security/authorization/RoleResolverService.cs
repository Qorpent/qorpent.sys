using System;
using System.Linq;
using System.Security.Principal;
using qorpent.v2.security.user;
using qorpent.v2.security.user.storage;
using Qorpent;
using Qorpent.IoC;

namespace qorpent.v2.security.authorization {
    [ContainerComponent(Lifestyle.Singleton, "roleresolver.service", ServiceType = typeof (IRoleResolverService))]
    public class RoleResolverService : ExtensibleServiceBase<IRoleResolver>, IRoleResolverService {
        private IRoleResolverCache _cache;

        /// <summary>
        /// </summary>
        [Inject]
        public IRoleResolverCache Cache {
            get { return _cache ?? (_cache = new RoleResolverCache()); }
            set { _cache = value; }
        }

        [Inject]
        public IUserService Users { get; set; }

        public bool IsInRole(IIdentity identity, string role, bool exact = false) {
            if (!identity.IsAuthenticated) {
                return false;
            }
            if (string.IsNullOrWhiteSpace(role)) {
                throw new ArgumentNullException("role");
            }
            var id = identity as Identity;
            if (null == id) {
               id = new Identity(identity);
            }
            role = role.ToUpperInvariant().Trim();
            //first rule - GUEST role is allowed for all authenticated users - allow
            if (role == "GUEST") {
                return true;
            }
            //second rule - if ADMIN requested and user is not marked as admin - deny
            if (role == "ADMIN" && !id.IsAdmin) {
                return false;
            }
            // third rule - if user IsAdmin - it's any role without checking (except exact mode)
            if (id.IsAdmin && !exact) {
                return true;
            }
            // forth - while role is not GUEST - guest is not allowed for any role if it's IsAdmin (in exact mode)
            if (id.IsGuest) {
                return false;
            }
            // fith - if user is not GUEST any other authenticated users are matched with DEFAULT role
            if (role == "DEFAULT") {
                return true;
            }
            ///sexth - if it's not GUEST, ADMIN , DEFAULT we must provide extension-based checking
            /// so it's true if user IsAdmin and role not ADMIN and exact mode
            var key = identity.Name + "::" + role + "::" + exact;
            return Cache.Get(key, _ => InternalIsInRole(identity, role, exact));
        }

        public void Clear() {
            _cache.Clear();
        }

        public override void OnContainerCreateInstanceFinished() {
            base.OnContainerCreateInstanceFinished();
            Initialize();
        }

        public void Initialize() {
            var us = Users as UserService;
            var c = Cache as RoleResolverCache;
            if (null != us && null != c) {
                var leases = c.GetExtensions().ToList();
                foreach (var extension in us.GetExtensions()) {
                    var rl = extension as IRoleResolverCacheLease;
                    if (null != rl) {
                        if (!leases.Contains(rl)) {
                            c.RegisterExtension(rl);
                        }
                    }
                }
            }
        }

        private bool InternalIsInRole(IIdentity identity, string role, bool exact) {
            return Extensions.Any(_ => _.IsInRole(identity, role, exact));
        }
    }
}