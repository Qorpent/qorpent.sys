using Qorpent;

namespace qorpent.v2.security.user.storage {
    public interface IUserService : IExtensibleService<IUserSource>, IUserSource, IWriteableUserSource {
        object Clear();
    }
}