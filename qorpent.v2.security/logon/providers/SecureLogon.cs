using System;
using System.Security;
using System.Security.Principal;
using qorpent.v2.security.encryption;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.user;
using qorpent.v2.security.user.services;
using qorpent.v2.security.user.storage;
using Qorpent;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.logon.providers {
    /// <summary>
    ///     Allows to configure
    /// </summary>
    [ContainerComponent(Lifestyle.Transient, "hash.logonservice", ServiceType = typeof (ILogonProvider))]
    public class SecureLogon : ServiceBase, ILogonProvider, ISecureLogon {
        private ISecureLogonService _secureLogon;
        private IUserStateChecker _stateChecker;

        /// <summary>
        ///     MS timeout to logon with given salt, 1 minute by default
        /// </summary>
        public int LogonTimeout = 60000;

        public SecureLogon() {
            Idx = 9990;
            Encryptor = new Encryptor(Guid.NewGuid().ToString());
        }

        [Inject]
        public ISecureLogonService SecureLogonService {
            get { return _secureLogon ?? (_secureLogon = new SecureLogonService()); }
            set { _secureLogon = value; }
        }

        public Encryptor Encryptor { get; set; }

        [Inject]
        public IUserService UserService { get; set; }

        /// <summary>
        /// </summary>
        [Inject]
        public IUserStateChecker StateChecker {
            get { return _stateChecker ?? (_stateChecker = new UserStateChecker()); }
            set { _stateChecker = value; }
        }

        public int Idx { get; set; }

        public IIdentity Logon(string username, SecureLogonInfo info, IScope context = null) {
            if (null == UserService) {
                return null;
            }
            var user = UserService.GetUser(username);
            if (!StateChecker.IsSecureLogable(user)) {
                return null;
            }
            var result = new Identity {
                Name = username,
                AuthenticationType = "secure"
            };
            var state = StateChecker.GetActivityState(user);
            if (state != UserActivityState.Ok) {
                result.IsError = true;
                result.Error = new SecurityException(state.ToStr());
            }
            else {
                try {
                    SecureLogonService.CheckSecureInfo(info, user, context);
                    result.IsAuthenticated = true;
                    result.User = user;
                    result.IsAdmin = user.IsAdmin;
                }
                catch (Exception e) {
                    result.IsError = true;
                    result.Error = e;
                }
            }

            return result;
        }

        public string GetSalt(string username, IScope context = null) {
            if (null == UserService) {
                return null;
            }
            var user = UserService.GetUser(username);

            if (!StateChecker.IsSecureLogable(user)) {
                return null;
            }

            return SecureLogonService.GetSalt(user, context);
        }
    }
}