using Qorpent;
using Qorpent.IoC;

namespace qorpent.v2.security.authorization {
    [ContainerComponent(Lifestyle.Singleton, "roleresolver.cache", ServiceType = typeof (IRoleResolverCache))]
    public class RoleResolverCache : CacheService<bool, IRoleResolverCacheLease>, IRoleResolverCache {
    }
}