using System;
using System.Linq;
using System.Security.Policy;
using System.Security.Principal;
using qorpent.Security;
using qorpent.v2.security.user;
using qorpent.v2.security.user.storage;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.Log.NewLog;

namespace qorpent.v2.security.authorization {
    [ContainerComponent(Lifestyle.Singleton, "roleresolver.service", ServiceType = typeof (IRoleResolverService))]
    public class RoleResolverService : ExtensibleServiceBase<IRoleResolver>, IRoleResolverService {
        private IRoleResolverCache _cache;
        private IRoleExpressionEvaluator _evaluator;

        /// <summary>
        /// </summary>
        [Inject]
        public IRoleResolverCache Cache {
            get { return _cache ?? (_cache = new RoleResolverCache()); }
            set { _cache = value; }
        }

        [Inject]
        public IUserService Users { get; set; }

        public RoleResolverService() {
            
        }

        public RoleResolverService(IUserService users = null, params IRoleResolver[] resolvers) {
            if (null != users) {
                Users = users;
            }
            if (null != resolvers) {
                foreach (var roleResolver in resolvers) {
                    RegisterExtension(roleResolver);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Inject]
        public IRoleExpressionEvaluator Evaluator {
            get { return _evaluator??(_evaluator = new RoleExpressionEvaluator()); }
            set { _evaluator = value; }
        }

        public bool IsInRole(IIdentity identity, string role, bool exact = false) {
            if (string.IsNullOrWhiteSpace(role)) return true;
            role = role.ToUpperInvariant().Trim();
            if (-1 != role.LastIndexOfAny(new[] {' ', '!','+', ',','-','&','|','(',')'})) {
                return Evaluator.Evaluate(this, identity,role);
            }
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
            
            //first rule - GUEST role is allowed for all authenticated users - allow
            if (role == SecurityConst.ROLE_GUEST) {
                return true;
            }
            //second rule - if ADMIN requested and user is not marked as admin - deny
            if (role == SecurityConst.ROLE_ADMIN && !id.IsAdmin) {
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
            if (role == SecurityConst.ROLE_USER) {
                return true;
            }
            ///sexth - if it's not GUEST, ADMIN , DEFAULT we must provide extension-based checking
            /// so it's true if user IsAdmin and role not ADMIN and exact mode
            var key = identity.Name + "::" + role + "::" + exact;
            return Cache.Get(key, _ => InternalIsInRole(identity, role, exact));
        }

        public void Clear() {
            Logg.Trace(new{roles="clear called"}.stringify());
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