using System.Security.Principal;

namespace qorpent.v2.security.authorization {
    public interface IRoleResolver {
        bool IsInRole(IIdentity identity, string role, bool exact = false);
    }
}