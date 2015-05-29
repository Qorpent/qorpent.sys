using System.Security.Principal;
using Qorpent.Security;

namespace Qorpent.Host.Tests {
    class TestHostLogon : IHostLogon
    {
        public int Idx { get; set; }

        public bool IsAuth(string username, string password, IScope context =null)
        {
            return username == password;
        }

        public IIdentity Logon(string username, string password, IScope context = null)
        {
            if (username == password) return new GenericIdentity(username);
            return null;
        }
    }
}