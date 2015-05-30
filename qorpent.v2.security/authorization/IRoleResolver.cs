using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Qorpent.Security;

namespace qorpent.v2.security.authorization
{
    public interface IRoleResolver {
        bool IsInRole(IIdentity identity, string role, bool exact =false);
    }
}
