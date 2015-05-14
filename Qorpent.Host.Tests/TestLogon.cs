using System.Security.Principal;
using Qorpent.Security;

namespace Qorpent.Host.Tests {
    class TestLogon : ILogon
    {
        public int Idx { get; set; }

        public bool IsAuth(string username, string password)
        {
            return username == password;
        }

        public IIdentity Logon(string username, string password)
        {
            if (username == password) return new GenericIdentity(username);
            return null;
        }
    }
}