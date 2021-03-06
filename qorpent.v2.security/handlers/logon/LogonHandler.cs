﻿using System;
using System.Security.Principal;
using System.Threading;
using Qorpent.Log.NewLog;
using qorpent.v2.security.authentication;
using qorpent.v2.security.logon;
using qorpent.v2.security.user;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Log;

namespace qorpent.v2.security.handlers.logon {
	[ContainerComponent(Lifestyle.Singleton, "handler.logon", ServiceType = typeof (ILogonHandler))]
    [UserOp("logon",Secure =true,SuccessLevel = LogLevel.Info,ErrorLevel = LogLevel.Warn,TreatFalseAsError = true)]
    public class LogonHandler : UserOperation,ILogonHandler {
        
       
        [Inject]
        public ILogonService LogonService { get; set; }

        [Inject]
        public IHttpTokenService TokenService { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="context"></param>
        /// <param name="callbackEndPoint"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        protected override HandlerResult GetResult(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var ctx = RequestParameters.Create(context);
            var login = ctx.Get("login");
            var password = ctx.Get("pass");
            var identity = (Identity)LogonService.Logon(login, password);
            context.User = new GenericPrincipal(identity, null);
            var logondata = new LogonInfo {
                Identity = identity,
                RemoteEndPoint = context.Request.RemoteEndPoint,
                LocalEndPoint = context.Request.LocalEndPoint,
                UserAgent = context.Request.UserAgent
            };
            var strRemoteIp = logondata.RemoteEndPoint.Address.ToString();
            if (identity.IsAuthenticated && !identity.IsGuest)
            {
                var token = TokenService.Create(context.Request);
                identity.User = identity.User ?? new User {Login = identity.Name};
	            var resolvedUsername = identity.User.Login;
				if (!string.IsNullOrWhiteSpace(identity.User.Domain)) {
					resolvedUsername = resolvedUsername + "@" + identity.User.Domain;
				}
                TokenService.Store(context.Response, context.Request.Uri, token);
				Loggy.Info("Login: " + resolvedUsername + ", " + logondata.UserAgent + " from " + strRemoteIp);
                return new HandlerResult {Result = new{auth=true,state=identity.State,stateinfo=identity.StateInfo}, Data = logondata};
            }
            if (!identity.IsAuthenticated && !identity.IsGuest) {
                Loggy.Warn("Login failed: " + context.User.Identity.Name + ", User Agent: [" + logondata.UserAgent + "] from ip: " + strRemoteIp);
            }
            TokenService.Store(context.Response, context.Request.Uri, null);
            return new HandlerResult {Result = new { auth = false, state = identity.State, stateinfo = identity.StateInfo }, Data = logondata};
        }

        public override string GetUserOperationLog(bool iserror, LogLevel level, HandlerResult result,WebContext context) {
            var info = result.Data as LogonInfo;
            if (iserror) {
                return new {
                    auth = false,
                    login = info.Identity.Name,
                    error = info.Identity.Error,
                    addr = info.RemoteEndPoint.Address.ToString(),
                    local = info.LocalEndPoint.Address.ToString(),
                    port = info.LocalEndPoint.Port
                }.stringify();
            }
            return new {
                auth = true,
                type = info.Identity.AuthenticationType,
                isadmin = info.Identity.IsAdmin,
                login = info.Identity.Name,
                error = info.Identity.Error,
                addr = info.RemoteEndPoint.Address.ToString(),
                local = info.LocalEndPoint.Address.ToString(),
                port = info.LocalEndPoint.Port,
                agent = info.UserAgent
            }.stringify();
        }

    }
}