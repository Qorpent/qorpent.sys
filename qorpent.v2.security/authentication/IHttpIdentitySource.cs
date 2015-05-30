using System.Security.Principal;
using Qorpent.IO.Http;

namespace qorpent.v2.security.authentication {
    public interface IHttpIdentitySource {
        IIdentity GetUserIdentity(IHttpRequestDescriptor request);
    }
}