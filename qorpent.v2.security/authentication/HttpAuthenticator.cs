using System.Security.Principal;
using qorpent.v2.security.user;
using Qorpent;
using Qorpent.IoC;
using Qorpent.IO.Http;

namespace qorpent.v2.security.authentication {
    [ContainerComponent(Lifestyle.Singleton,"http.authenticator",ServiceType=typeof(IHttpAuthenticator))]
    public class HttpAuthenticator:ServiceBase,IHttpAuthenticator {


        [Inject]
        public IHttpTokenService TokenService { get; set; }
   
        [Inject]
        public IHttpIdentitySource IdentitySource { get; set; }


        public void Authenticate(IHttpRequestDescriptor request, IHttpResponseDescriptor response) {
            var identity = (Identity)IdentitySource.GetUserIdentity(request);
            request.User = new GenericPrincipal(identity,null);
            var token = identity.IsAuthenticated ? identity.Token : null;
            TokenService.Store(response, request.Uri, token);
        }


        

        
    }
}