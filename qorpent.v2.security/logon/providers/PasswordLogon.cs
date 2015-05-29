using System.Security;
using System.Security.Principal;
using qorpent.v2.security.user;
using qorpent.v2.security.user.services;
using qorpent.v2.security.user.storage;
using Qorpent;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.logon.providers
{
    /// <summary>
    /// Allows to configure 
    /// </summary>
    [ContainerComponent(Lifestyle.Transient,"hash.logonservice",ServiceType = typeof(ILogonProvider))]
    public class PasswordLogon : ServiceBase, ILogonProvider,IPasswordLogon
    {
        


        public PasswordLogon() {
            Idx = WinLogon.WINLOGONIDX - 10;
        }


        [Inject]
        public IUserService UserService { get; set; }
        private IUserStateChecker _stateChecker;
        /// <summary>
        /// 
        /// </summary>
        [Inject]
        public IUserStateChecker StateChecker {
            get { return _stateChecker ?? (_stateChecker = new UserStateChecker()); }
            set { _stateChecker = value; }
        }


        public IIdentity Logon(string username, string password, IScope context = null) {
            if (null == UserService) return null;
            var user = UserService.GetUser(username);
            if (!StateChecker.IsPasswordLogable(user)) return null;
            var result = new Identity {
                Name = username,
                AuthenticationType = "hash"
            };
            var state = StateChecker.GetActivityState(user);
            if (state != UserActivityState.Ok) {
                result.IsError = true;
                result.Error = new SecurityException(state.ToStr());
            }
            else {
                var hash = (username + password + user.Salt).GetMd5();
                if (hash == user.Hash) {
                    result.IsAuthenticated = true;
                    result.IsAdmin = user.IsAdmin;
                    result.User = user;
                }
                else {
                    result.IsError = true;
                    result.Error = new SecurityException("invalid hash");
                }
            }        
            return result;
        }

        public int Idx { get; set; }
    }
}
