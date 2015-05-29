using Qorpent;
using Qorpent.IoC;

namespace qorpent.v2.security.user.storage {
    [ContainerComponent(Lifestyle.Singleton,"user.cache",ServiceType=typeof(IUserCache))]
    [ContainerComponent(Lifestyle.Singleton,"user.cache-service",ServiceType=typeof(ICacheService<IUser,IUserCacheLease>))]
    public class UserCache : CacheServiceBase<IUser, IUserCacheLease>, IUserCache {
        
    }
}