using System.Security.Principal;
using Qorpent.IO.Http;

namespace qorpent.v2.security.authorization {
    public interface INotAuthProcessProvider {
        AuthorizationReaction GetReaction( IHttpRequestDescriptor request);
    }
}