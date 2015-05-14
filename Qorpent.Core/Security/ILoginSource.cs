using System.Collections.Generic;
using Qorpent.Events;

namespace Qorpent.Security {
    public interface ILoginSource :IResetable{
        IEnumerable<LoginInfo> Query(LoginInfo match = null);
        LoginInfo Get(string login);
    }
}