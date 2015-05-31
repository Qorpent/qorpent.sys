using System;
using Qorpent.IO.Http;

namespace qorpent.v2.security.authentication {
    public interface IHttpTokenService {
        Token Extract(IHttpRequestDescriptor request);
        bool IsValid(IHttpRequestDescriptor request, Token token);
        void Store(IHttpResponseDescriptor response, Uri requestUri, Token token);
        Token Create(IHttpRequestDescriptor request);
        Token Prolongate(Token existedToken);
    }
}