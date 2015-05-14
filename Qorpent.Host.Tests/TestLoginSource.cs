using System.Collections.Generic;
using Qorpent.Events;
using Qorpent.Security;

namespace Qorpent.Host.Tests {
    class TestLoginSource : ILoginSource
    {



        public object Reset(ResetEventData data)
        {
            if (a.IsAdmin)
            {
                a.IsAdmin = false;
                return null;
            }
            if (a.IsActive)
            {
                a.RawIsActive = false;
            }

            return null;
        }

        public object GetPreResetInfo()
        {
            return null;
        }

        private LoginInfo a = new LoginInfo
        {
            Login = "adm",
            Name = "AA",
            IsAdmin = true,
            RawIsActive = true,
            Password = "adm",
        };

        private LoginInfo u = new LoginInfo
        {
            Login = "usr",
            Name = "UU",
            RolesList = "viewver x.all",
            GroupList = "y",
            Password = "usr"

        };

        private LoginInfo yg = new LoginInfo
        {
            Login = "y@groups",
            Name = "YG",
            IsGroup = true,
            RolesList = "reporter y",
        };

        public IEnumerable<LoginInfo> Query(LoginInfo match = null)
        {
            return new[] { a, u, yg };
        }

        public LoginInfo Get(string login)
        {
            if (login == "adm") return a;
            if (login == "usr") return u;
            if (login == "y@groups") return yg;
            return null;
        }
    }
}