using Qorpent;

namespace qorpent.v2.security.user.storage {
    public interface IUserCache : ICacheService<IUser, IUserCacheLease> {
    }
}