using System;
using System.Threading;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.messaging;
using qorpent.v2.security.user;
using qorpent.v2.security.user.services;
using qorpent.v2.security.user.storage;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Log;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.handlers.logon {
    [ContainerComponent(Lifestyle.Singleton, "resetpwdreq.handler", ServiceType = typeof (IRequireResetPasswordHandler))]
    [UserOp("resetpwdreq",Secure = true,SuccessLevel = LogLevel.Info)]
    public class RequireResetPasswordHandler : UserOperation,IRequireResetPasswordHandler {
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

        HandlerResult GetError(string error, string login, string email) {
            return new HandlerResult { Result = new {error}, State = 500 , Data = new{error,login,email}};
        }

        protected override HandlerResult GetResult(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var p = RequestParameters.Create(context);
            var login = p.Get("login");
            
            var email = p.Get("email");
           
            if (string.IsNullOrWhiteSpace(login))
            {
                return GetError("no login", login, email);
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                return GetError("no email", login, email);
            }
            var user = Users.GetUser(login);
            if (null == user)
            {
                return GetError("not existed user", login, email);
            }
            if (!CheckState.IsLogable(user))
            {
                return GetError("not for logon user", login, email);
            }
            var state = CheckState.GetActivityState(user);
            if (state != UserActivityState.Ok)
            {
                return GetError("invalid state " + state.ToStr(), login, email);
            }
            if (user.Email != email)
            {
                return GetError("invalid email", login, email);
            }
            PasswordManager.MakeRequest(user, 10, email);
            Users.Store(user);
            var message = UserMessagingService.SendPasswordReset(user);
            bool sent = false;
            string senderror = "";
            //try force
            try
            {
                if (null != Sender)
                {
                    var savedmessage = Queue.GetMessage(message.Id);
                    if (!savedmessage.WasSent)
                    {
                        Sender.Send(savedmessage);
                        Queue.MarkSent(savedmessage.Id);
                    }
                    sent = true;
                }
                else
                {
                    throw new Exception("no sender found");
                }
            }
            catch (Exception e)
            {
                senderror = e.Message;
            }
            var result = new {messageid = message.Id, minutes = 10, sent, senderror};
            return new HandlerResult {Result = result, Data = new{resetpwdreq=true,login,email,data=result}};
        }


        

 

    }
}