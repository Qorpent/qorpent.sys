using System;
using System.Threading;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.user;
using qorpent.v2.security.user.services;
using qorpent.v2.security.user.storage;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.handlers.logon {
    [ContainerComponent(Lifestyle.Singleton, "resetpwd.handler", ServiceType = typeof (IResetPasswordHandler))]
    public class ResetPasswordHandler : IResetPasswordHandler {
        [Inject]
        public IUserService Users { get; set; }

        [Inject]
        public IUserStateChecker CheckState { get; set; }

        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var parameters = RequestParameters.Create(context);
            var login = parameters.Get("login");
            var key = parameters.Get("key");
            var pass = parameters.Get("pass");
            if (string.IsNullOrWhiteSpace(login)) {
                Error(context, "no login");
                return;
            }
            if (string.IsNullOrWhiteSpace(key)) {
                Error(context, "no key");
                return;
            }
            if (string.IsNullOrWhiteSpace(pass)) {
                Error(context, "no pass");
                return;
            }
            var user = Users.GetUser(login);
            if (null == user) {
                Error(context, "no user");
                return;
            }
            if (!CheckState.IsLogable(user)) {
                Error(context, "not logable user");
                return;
            }
            var state = CheckState.GetActivityState(user);
            if (state != UserActivityState.Ok) {
                Error(context, "invalid state " + state.ToStr());
                return;
            }

            var pwdmanager = new PasswordManager();
            try {
                pwdmanager.ResetPassword(user, pass, key);
            }
            catch (Exception e) {
                Error(context, "pwd error " + e.Message);
                return;
            }

            Users.Store(user);

            context.Finish(new {passchanged = true}.stringify());
        }

        private static void Error(WebContext context, string message) {
            context.Finish(GetError(message), status: 500);
        }

        private static string GetError(string m) {
            return "{\"error\":\"" + m + "\"}";
        }
    }
}