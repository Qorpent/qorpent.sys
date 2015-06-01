using System;
using System.Threading;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.messaging;
using qorpent.v2.security.user;
using qorpent.v2.security.user.services;
using qorpent.v2.security.user.storage;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.handlers.logon {
    [ContainerComponent(Lifestyle.Singleton, "resetpwdreq.handler", ServiceType = typeof (IRequireResetPasswordHandler))
    ]
    public class RequireResetPasswordHandler : IRequireResetPasswordHandler {
        [Inject]
        public IUserService Users { get; set; }

        [Inject]
        public IUserStateChecker CheckState { get; set; }

        [Inject]
        public IUserMessagingService UserMessagingService { get; set; }

        [Inject]
        public IMessageQueue Queue { get; set; }

        [Inject]
        public IMessageSender Sender { get; set; }

        [Inject]
        public IPasswordManager PasswordManager { get; set; }

        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var p = RequestParameters.Create(context);
            var login = p.Get("login");
            if (EmptyLogin(context, login)) {
                return;
            }
            var email = p.Get("email");
            if (EmptyEmail(context, email)) {
                return;
            }
            var user = Users.GetUser(login);
            if (null == user) {
                Error(context, "not existed user");
                return;
            }
            if (!CheckState.IsLogable(user)) {
                Error(context, "not for logon user");
                return;
            }
            var state = CheckState.GetActivityState(user);
            if (state != UserActivityState.Ok) {
                Error(context, "invalid state " + state.ToStr());
                return;
            }
            if (user.Email != email) {
                Error(context, "invalid email");
                return;
            }
            PasswordManager.MakeRequest(user, 10, email);
            Users.Store(user);
            var message = UserMessagingService.SendPasswordReset(user);
            //try force
            try {
                if (null != Sender) {
                    var savedmessage = Queue.GetMessage(message.Id);
                    if (!savedmessage.WasSent) {
                        Sender.Send(savedmessage);
                        Queue.MarkSent(savedmessage.Id);
                    }
                }
            }
            catch (Exception e) {
            }
            context.Finish(new {messageid = message.Id, minutes = 10}.stringify());
        }

        private static void Error(WebContext context, string message) {
            context.Finish(GetError(message), status: 500);
        }

        private static string GetError(string m) {
            return "{\"error\":\"" + m + "\"}";
        }

        private static bool EmptyLogin(WebContext context, string login) {
            if (string.IsNullOrWhiteSpace(login)) {
                Error(context, "no login");
                return true;
            }
            return false;
        }

        private static bool EmptyEmail(WebContext context, string email) {
            if (string.IsNullOrWhiteSpace(email)) {
                Error(context, "no email");
                return true;
            }
            return false;
        }
    }
}