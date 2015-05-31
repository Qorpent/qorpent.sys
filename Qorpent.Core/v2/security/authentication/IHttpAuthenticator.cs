using Qorpent.IO.Http;

namespace qorpent.v2.security.authentication {
    public interface IHttpAuthenticator {
        void Authenticate(IHttpRequestDescriptor request, IHttpResponseDescriptor response);
    }
}