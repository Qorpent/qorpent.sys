using System.Security;
using System.Security.Principal;
using qorpent.v2.security.user;
using qorpent.v2.security.user.services;
using qorpent.v2.security.user.storage;
using Qorpent;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.authentication {
    [ContainerComponent(Lifestyle.Singleton, "http.identitysource", ServiceType = typeof(IHttpIdentitySource))]
    public class HttpIdentitySource:ServiceBase,IHttpIdentitySource {
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
            if (currentToken != null && TokenService.IsValid(request, currentToken))
            {
                return AuthenticateWithValidToken(request, currentToken);
            }
            return DefaultIdentitySource.GetUserIdentity(request);
        }

        private IIdentity AuthenticateWithValidToken(IHttpRequestDescriptor request, Token currentToken)
        {
            var token = TokenService.Prolongate(currentToken);
            var result = BuildIdentity(token);
            return result;
        }

        private Identity BuildIdentity(Token token)
        {
            var result = new Identity
            {
                Token = token,
                Name = token.User,
                IsAuthenticated = true,
                AuthenticationType = "form"
            };
            var errormessage = "";
            var user = UserService.GetUser(token.User);
            if (null != user)
            {
                result.User = user;
                result.IsAdmin = user.IsAdmin;
                var userstate = UserStateChecker.GetActivityState(user);
                if (userstate != UserActivityState.Ok)
                {
                    result.IsError = true;
                    errormessage += userstate.ToStr() + "; ";
                }
            }
            if (!string.IsNullOrWhiteSpace(token.ImUser))
            {
                var imtoken = new Token { User = token.ImUser };
                var imidentity = BuildIdentity(imtoken);
                result.ImpersonationSource = imidentity;
                if (!imidentity.IsAuthenticated)
                {
                    result.IsError = true;
                    errormessage += "not-auth impersonation; ";
                }
                else if (!imidentity.IsAdmin)
                {
                    result.IsError = true;
                    errormessage += "non-admin impersonation;";
                }

            }
            if (result.IsError)
            {
                result.IsAuthenticated = false;
                result.Error = new SecurityException(errormessage);
            }
            return result;
        }

         
    }
}