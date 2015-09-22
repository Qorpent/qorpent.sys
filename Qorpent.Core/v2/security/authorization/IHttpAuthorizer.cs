using Qorpent.IO.Http;

namespace qorpent.v2.security.authorization
{

    public interface IHttpAuthorizer {
        AuthorizationReaction Authorize(IHttpRequestDescriptor  request);
    }

}
