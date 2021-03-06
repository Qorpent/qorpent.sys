using System;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Threading;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.user;
using Qorpent;
using Qorpent.Events;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.Log.NewLog;
using Qorpent.Model;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.logon {
    /// <summary>
    ///     Wraps both IPasswordLogon and ISecureLogon over unified facade,
    ///     works on set of ILogonService, support logging to Loggy
    /// </summary>
    [ContainerComponent(Lifestyle.Singleton, "loginservice.v2", ServiceType = typeof (ILogonService))]
    public partial class LogonService : ExtensibleServiceBase<ILogonProvider>, ILogonService {
        private const string PWDLOGONOPID = "lgn_pl_";
        private const string SECLOGONOPID = "lgn_sl_";
        private const string GETSALTOPID = "lgn_gs_";
        private int logonid;

        public IIdentity Logon(string username, string password, IScope context = null) {
            lock (this) {
                if (string.IsNullOrWhiteSpace(username)) {
                    throw new ArgumentException("username");
                }
                if (string.IsNullOrWhiteSpace(password)) {
                    throw new ArgumentException("password");
                }
                var opid = PWDLOGONOPID + Interlocked.Increment(ref logonid);
                LogStart(username, password, context, opid);
                IIdentity result = null;
                try {
                    result = ResolveByExtensions(username, password,opid, context)
                             ?? GetDefaultLogon(username);
                }
                catch (Exception ex) {
                    LogError(username, password, context, opid, ex);
                    result = GetErrorLogon(username, ex);
                    ((Identity)result).State = UserActivityState.Error;
                }
                Identity ires = (Identity) result;
                if (ires.State == UserActivityState.None) {
                    ires.State = ires.IsAuthenticated ? UserActivityState.Ok : UserActivityState.InvalidLogonInfo;
                }                
                
                LogResult(result, opid);
                return result;
            }
        }

        /// <summary>
        ///     True if SecureLogon is configured
        /// </summary>
        public bool SupportSecureLogon {
            get { return Extensions.Any(_ => _ is ISecureLogon); }
        }

        int IWithIndex.Idx { get; set; }

        public IIdentity Logon(string username, SecureLogonInfo info, IScope context = null) {
            if (string.IsNullOrWhiteSpace(username)) {
                throw new ArgumentException("username");
            }
            if (null == info) {
                throw new ArgumentException("info");
            }
            if (string.IsNullOrWhiteSpace(info.Salt)) {
                throw new ArgumentException("info.salt");
            }
            if (string.IsNullOrWhiteSpace(info.Sign)) {
                throw new ArgumentException("info.sign");
            }
            var opid = SECLOGONOPID + Interlocked.Increment(ref logonid);
            LogStart(username, info, context, opid);
            var securelogon = Extensions.OfType<ISecureLogon>().FirstOrDefault();
            if (null == securelogon) {
                if (Logg.IsForError()) {
                    Logg.Error(new {opid, message = "not secure login confugured"});
                }
                return GetErrorLogon(username,
                    new SecurityException("secure logon not supported in current configuration"));
            }
            IIdentity result = null;
            try {
                result = securelogon.Logon(username, info, context) ?? GetDefaultLogon(username);
            }
            catch (Exception ex) {
                LogError(username, info, context, opid, ex);
                result = GetErrorLogon(username, ex);
                ((Identity)result).State = UserActivityState.Error;
            }
            Identity ires = (Identity)result;
            if (ires.State == UserActivityState.None)
            {
                ires.State = ires.IsAuthenticated ? UserActivityState.Ok : UserActivityState.InvalidLogonInfo;
            }
            LogResult(result, opid);
            return result;
        }

        public string GetSalt(string username, IScope context = null) {
            if (string.IsNullOrWhiteSpace(username)) {
                throw new ArgumentException("username");
            }
            var opid = GETSALTOPID + Interlocked.Increment(ref logonid);
            if (Logg.IsForDebug()) {
                Logg.Debug(new {opid, username, context});
            }
            string result = null;
            var securelogon = Extensions.OfType<ISecureLogon>().FirstOrDefault();
            if (null == securelogon) {
                if (Logg.IsForError()) {
                    Logg.Error(new {opid, message = "not secure login confugured"}.stringify());
                }
            }
            else {
                result = securelogon.GetSalt(username);
            }
            if (Logg.IsForDebug()) {
                Logg.Debug(new {opid, username, salt = result}.stringify());
            }
            return result;
        }

        public object Reset(ResetEventData data) {
            Logg.Info(new {logon = "reset was called"});
            foreach (var logonProvider in Extensions) {
                logonProvider.Reset(data);
            }
            return null;
        }

        protected override string GetLoggerNameSuffix() {
            return "logon_service";
        }

        protected override void OnChangeExtensions() {
            base.OnChangeExtensions();
            if (Logg.IsForTrace()) {
                Logg.Trace(new {logonproviders = Extensions.Select(_ => _.ToString()).ToArray()}.stringify());
            }
        }

        private void LogResult(IIdentity result, string opid) {
            if (!result.IsAuthenticated) {
                if (Logg.IsForWarn()) {
                    Logg.Warn(new {logonid = opid, auth = false, result }.stringify());
                }
            }
            else {
                if (Logg.IsForInfo()) {
                    Logg.Info(new {logonid = opid, auth = true, result}.stringify());
                }
            }
        }

        private void LogError(string username, string password, IScope context, string opid, Exception ex) {
            if (Logg.IsForError()) {
                Logg.Error(
                    new {
                        opid,
                        username,
                        password = password.GetMd5(),
                        context,
                        logonerror = ex.Message
                    }.stringify());
            }
        }

        private void LogError(string username, SecureLogonInfo info, IScope context, string opid, Exception ex) {
            if (Logg.IsForError()) {
                Logg.Error(
                    new {
                        opid,
                        username,
                        salt = info.Salt,
                        sign = info.Sign,
                        context,
                        logonerror = ex.Message
                    }.stringify());
            }
        }

        private IIdentity ResolveByExtensions(string username, string password, string opid, IScope context) {
           var extensions = Extensions.OfType<IPasswordLogon>().ToArray();
            IIdentity bestresult = null;
            foreach (var passwordLogon in extensions) {
                if (Logg.IsForDebug()) {
                    Logg.Debug(new {opid,ext=passwordLogon.GetType().Name,message="enter"}.stringify());
                }
                var subresult = passwordLogon.Logon(username, password, context);
                if (Logg.IsForDebug())
                {
                    Logg.Debug(new { opid, ext = passwordLogon.GetType().Name, message = null != subresult && subresult.IsAuthenticated }.stringify());
                }
                if (null != subresult && UserActivityState.None != ((Identity) subresult).State) {
                    bestresult = subresult;
                }
                
                if (null != subresult && subresult.IsAuthenticated) {
                    return subresult;
                }
            }
            return bestresult;
        }

        private void LogStart(string username, string password, IScope context, string opid) {
            if (Logg.IsForDebug()) {
                Logg.Debug(new {opid, username, pass = password.GetMd5(), context}.stringify());
            }
        }

        private void LogStart(string username, SecureLogonInfo info, IScope context, string opid) {
            if (Logg.IsForDebug()) {
                Logg.Debug(new {opid, username, salt = info.Salt, sign = info.Sign, context}.stringify());
            }
        }

        private IIdentity GetErrorLogon(string username, Exception exception) {
            return new Identity {
                Name = username.ToLowerInvariant(),
                AuthenticationType = "error",
                IsAuthenticated = false,
                IsError = true,
                Error = exception
            };
        }

        private IIdentity GetDefaultLogon(string username) {
            return new Identity {
                Name = username.ToLowerInvariant(),
                AuthenticationType = "undefined",
                IsAuthenticated = false
            };
        }
    }
}