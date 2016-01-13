using Qorpent.IO.Http;

namespace Qorpent.Host {
    public interface IRedirectSource
    {
        string GetRedirectUrl(IHttpRequestDescriptor context);
    }
}