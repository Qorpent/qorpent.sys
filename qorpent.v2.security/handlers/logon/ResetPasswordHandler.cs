using System.Threading;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.user;
using qorpent.v2.security.user.services;
using qorpent.v2.security.user.storage;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Log;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.handlers.logon {
    [ContainerComponent(Lifestyle.Singleton, "resetpwd.handler", ServiceType = typeof (IResetPasswordHandler))]
    [UserOp("resetpwd",Secure = true,SuccessLevel = LogLevel.Warn,ExceptionLevel = LogLevel.Error)]
    public class ResetPasswordHandler :UserOperation, IResetPasswordHandler {
        [Inject]
        public IUserService Users { get; set; }

        [Inject]
        public IUserStateChecker CheckState { get; set; }
        [Inject]
        public IPasswordManager PasswordManager { get; set; }

        HandlerResult GetError(string error, string login, string pass, string key)
        {
            return new HandlerResult { Result = new { error }, State = 500, Data = new { error, login, pass = pass.GetMd5(),key } };
        }

        protected override HandlerResult GetResult(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var parameters = RequestParameters.Create(context);
            var login = parameters.Get("login");
            var key = parameters.Get("key");
            var pass = parameters.Get("pass");
            if (string.IsNullOrWhiteSpace(login))
            {
                return GetError("no login", login, pass, key);
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                return GetError("no key", login, pass, key);
               
            }
            if (string.IsNullOrWhiteSpace(pass))
            {
                return GetError("no pass", login, pass, key);
            }
            var user = Users.GetUser(login);
            if (null == user)
            {
                return GetError("no user", login, pass, key);
          
            }
            if (!CheckState.IsLogable(user))
            {
                return GetError("not logable user", login, pass, key);
          
            }
            var state = CheckState.GetActivityState(user);
            if (state != UserActivityState.Ok)
            {
                return GetError("invalid state " + state, login, pass, key);
            }
            
            PasswordManager.ResetPassword(user, pass, key);

            
            Users.Store(user);

            return new HandlerResult {
                Result = new {passchanged = true},
                Data = new {passchanged = true, login, key, pass = pass.GetMd5()}
            };
        }

    }
}