using System.Security;
using System.Security.Principal;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.user;
using qorpent.v2.security.user.services;
using qorpent.v2.security.user.storage;
using Qorpent;
using Qorpent.IoC;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.logon.providers {
    /// <summary>
    ///     Allows to configure
    /// </summary>
    [ContainerComponent(Lifestyle.Transient, "hash.logonservice", ServiceType = typeof (ILogonProvider))]
    public class PasswordLogon : ServiceBase, ILogonProvider, IPasswordLogon {
        private IPasswordManager _passwordManager;
        private IUserStateChecker _stateChecker;

        public PasswordLogon() {
            Idx = WinLogon.WINLOGONIDX - 10;
        }

        [Inject]
        public IUserService UserService { get; set; }

        /// <summary>
        /// </summary>
        [Inject]
        public IPasswordManager PasswordManager {
            get { return _passwordManager ?? (_passwordManager = new PasswordManager()); }
            set { _passwordManager = value; }
        }

        /// <summary>
        /// </summary>
        [Inject]
        public IUserStateChecker StateChecker {
            get { return _stateChecker ?? (_stateChecker = new UserStateChecker()); }
            set { _stateChecker = value; }
        }

        public int Idx { get; set; }

        public IIdentity Logon(string username, string password, IScope context = null) {
            if (null == UserService) {
                if (Logg.IsForDebug()) {
                    Logg.Debug("No user service");
                }
                return null;
            }
            var user = UserService.GetUser(username);
            if (null == user && Logg.IsForDebug()) {
                Logg.Debug("user is null");
            }
            if (!StateChecker.IsPasswordLogable(user)) {
                Logg.Debug("user not logable");
                return null;
            }
            var result = new Identity {
                Name = username,
                AuthenticationType = "hash"
            };
            var state = StateChecker.GetActivityState(user);
            if (state != UserActivityState.Ok) {
                Logg.Debug("user is in invalid state "+state);
                result.State = state;
                result.IsError = true;
                result.Error = new SecurityException(state.ToStr());
            }
            else {
                if (PasswordManager.MatchPassword(user, password)) {
                    Logg.Debug("pass matched");
                    result.IsAuthenticated = true;
                    result.IsAdmin = user.IsAdmin;
                    result.User = user;
                }
                else {
                    Logg.Debug("pass not matched");
                    result.IsError = true;
                    result.Error = new SecurityException("invalid hash");
                }
            }
            return result;
        }
    }
}