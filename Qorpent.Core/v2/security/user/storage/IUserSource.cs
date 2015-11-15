using System.Collections;
using System.Collections.Generic;
using Qorpent.Model;

namespace qorpent.v2.security.user.storage {
    public interface IUserSource : IWithIndex {
        IUser GetUser(string login);
        IEnumerable<IUser> SearchUsers(UserSearchQuery query);
    }
}