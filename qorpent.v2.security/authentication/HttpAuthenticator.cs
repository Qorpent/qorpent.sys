using System.Security.Principal;
using qorpent.v2.security.user;
using Qorpent;
using Qorpent.IoC;
using Qorpent.IO.Http;

namespace qorpent.v2.security.authentication {
    [ContainerComponent(Lifestyle.Singleton, "sys.sec.http.authenticator", ServiceType = typeof (IHttpAuthenticator))]
    public class HttpAuthenticator : ServiceBase, IHttpAuthenticator {
        private IHttpTokenService _tokenService;
        private IHttpIdentitySource _identitySource;

        /// <summary>
        /// 
        /// </summary>
        [Inject]
        public IHttpTokenService TokenService {
            get { return _tokenService ?? (_tokenService  =new HttpTokenService()); }
            set { _tokenService = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Inject]
        public IHttpIdentitySource IdentitySource {
            get { return _identitySource ?? (_identitySource = new HttpDefaultIdentitySource()); }
            set { _identitySource = value; }
        }

        public void Authenticate(IHttpRequestDescriptor request, IHttpResponseDescriptor response) {
            var identity = (Identity) IdentitySource.GetUserIdentity(request);
            request.User = new GenericPrincipal(identity, null);
            var token = identity.IsAuthenticated ? identity.Token : null;
            TokenService.Store(response, request.Uri, token);
        }
    }
}