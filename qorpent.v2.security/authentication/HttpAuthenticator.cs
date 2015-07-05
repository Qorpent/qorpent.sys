using System.Security.Principal;
using qorpent.v2.security.user;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Log.NewLog;

namespace qorpent.v2.security.authentication {
    [ContainerComponent(Lifestyle.Singleton, "sys.sec.http.authenticator", ServiceType = typeof (IHttpAuthenticator))]
    public class HttpAuthenticator : ServiceBase, IHttpAuthenticator {
        private IHttpTokenService _tokenService;
        private IHttpIdentitySource _identitySource;
        private ILoggy _oplog;

        /// <summary>
        /// 
        /// </summary>
        [Inject]
        public IHttpTokenService TokenService {
            get { return _tokenService ?? (_tokenService  =new HttpTokenService()); }
            set { _tokenService = value; }
        }

        ILoggy OpLog {
            get { return _oplog ?? (_oplog = LoggyManager.Get("user.op.secure.auth")); }
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
            if (identity.IsAuthenticated) {
                if (OpLog.IsForDebug()) {
                    OpLog.Debug(new { isauth = true, login = identity.Name, url = request.Uri.ToString() }.stringify());
                }
            }
            else {
                if (null == identity.DisabledToken) {
                    if (OpLog.IsForDebug()) {
                        OpLog.Debug(new {isauth = false, login = identity.Name, url = request.Uri.ToString()}.stringify());
                    }
                }
                else {
                    if (OpLog.IsForInfo()) {
                        OpLog.Info(new { isauth = false, login = identity.Name, url = request.Uri.ToString(), disabledtoken = identity.DisabledToken }.stringify());
                    }
                }
            }
            TokenService.Store(response, request.Uri, token);
        }
    }
}