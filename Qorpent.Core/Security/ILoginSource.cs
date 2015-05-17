using System.Collections.Generic;
using Qorpent.Events;

namespace Qorpent.Security {
    public interface ILoginSource :IResetable{
        IEnumerable<LoginInfo> Query(object _query = null);
        LoginInfo Get(string login);
        void Save(LoginInfo login, bool forced=false);
    }
}