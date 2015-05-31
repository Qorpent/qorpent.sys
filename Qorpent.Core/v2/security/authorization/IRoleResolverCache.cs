using Qorpent;

namespace qorpent.v2.security.authorization {
    public interface IRoleResolverCache : ICacheService<bool, IRoleResolverCacheLease> {
    }
}