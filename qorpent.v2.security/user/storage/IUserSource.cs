using Qorpent.Model;

namespace qorpent.v2.security.user.storage {
    public interface IUserSource : IWithIndex {
        IUser GetUser(string login);
    }
}