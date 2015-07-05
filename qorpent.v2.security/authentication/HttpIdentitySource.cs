using System.Security;
using System.Security.Principal;
using qorpent.v2.security.user;
using qorpent.v2.security.user.services;
using qorpent.v2.security.user.storage;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.authentication {
    [ContainerComponent(Lifestyle.Singleton, "sys.sec.http.identitysource", ServiceType = typeof (IHttpIdentitySource))]
    public class HttpIdentitySource : ServiceBase, IHttpIdentitySource {
        [Inject]
        public IHttpTokenService TokenService { get; set; }

        [Inject]
        public IUserService UserService { get; set; }

        [Inject]
        public IUserStateChecker UserStateChecker { get; set; }

        [Inject]
        public IHttpDefaultIdentitySource DefaultIdentitySource { get; set; }

        public IIdentity GetUserIdentity(IHttpRequestDescriptor request) {
            var currentToken = TokenService.Extract(request);
            if (Logg.IsForDebug()) {
                Logg.Debug(new {request = request.Uri.ToString(), action = "extract", token = currentToken}.stringify());
            }
            if (currentToken != null && TokenService.IsValid(request, currentToken)) {
                return AuthenticateWithValidToken(request, currentToken);
            }
            var result = (Identity)DefaultIdentitySource.GetUserIdentity(request);
            result.DisabledToken = currentToken;
            return result;
        }

        private IIdentity AuthenticateWithValidToken(IHttpRequestDescriptor request, Token currentToken) {
            var currentExpire = currentToken.Expire.ToUniversalTime();
            Token token;
            if (IsProlongable(request)) {
                token = TokenService.Prolongate(currentToken);
            }
            else {
                token = currentToken;
            }
            var resultExpire = token.Expire.ToUniversalTime();
            if (Logg.IsForDebug()) {
                Logg.Debug(
                    new {request = request.Uri.ToString(), token = "upgrade", from = currentExpire, to = resultExpire}
                        .stringify());
            }
            var result = BuildIdentity(token);
            return result;
        }

        private bool IsProlongable(IHttpRequestDescriptor request) {
            var path = request.Uri.AbsolutePath;
            if (path == "/isauth") return false;
            if (path == "/logout") return false;
            if (path == "/myinfo") return false;
            return true;
        }

        private Identity BuildIdentity(Token token) {
            var result = new Identity {
                Token = token,
                Name = token.User,
                IsAuthenticated = true,
                AuthenticationType = "form",
                IsAdmin = token.IsAdmin
            };
            var errormessage = "";
            var user = UserService.GetUser(token.User);

            if (null != user) {
                result.User = user;
                result.IsAdmin = user.IsAdmin;
                var userstate = UserStateChecker.GetActivityState(user);
                if (userstate != UserActivityState.Ok) {
                    result.IsError = true;
                    errormessage += userstate.ToStr() + "; ";
                }
            }
            if (!string.IsNullOrWhiteSpace(token.ImUser)) {
                var imtoken = new Token {User = token.ImUser};
                var imidentity = BuildIdentity(imtoken);
                result.ImpersonationSource = imidentity;
                if (!imidentity.IsAuthenticated) {
                    result.IsError = true;
                    errormessage += "not-auth impersonation; ";
                }
                else if (!imidentity.IsAdmin) {
                    result.IsError = true;
                    errormessage += "non-admin impersonation;";
                }
            }
            if (result.IsError) {
                result.IsAuthenticated = false;
                result.Error = new SecurityException(errormessage);
            }
            return result;
        }
    }
}