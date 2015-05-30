using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.Host;
using Qorpent.IoC;

namespace qorpent.v2.security.handlers
{
    [ContainerComponent(Lifestyle.Transient,"securityhandlers.setup",ServiceType=typeof(IHostServerInitializer))]
    class SecurityHandlerSetup : IHostServerInitializer
    {
        private ILogonHandler _logonHandler;
        private ILogoutHandler _logoutHandler;
        private IIsAuthHandler _isAuthHandler;
        private IIsInRoleHandler _isInRoleHandler;
        private IMyInfoHandler _myInfoHandler;
        private IDefineUserHandler _defineUserHandler;

        [Inject]
        public ILogonHandler LogonHandler {
            get { return _logonHandler ??(_logonHandler = new LogonHandler()); }
            set { _logonHandler = value; }
        }

        [Inject]
        public ILogoutHandler LogoutHandler {
            get { return _logoutHandler ?? (_logoutHandler = new LogoutHandler()); }
            set { _logoutHandler = value; }
        }

        [Inject]
        public IIsAuthHandler IsAuthHandler {
            get { return _isAuthHandler??(_isAuthHandler=new IsAuthHandler()); }
            set { _isAuthHandler = value; }
        }

        [Inject]
        public IIsInRoleHandler IsInRoleHandler {
            get { return _isInRoleHandler ?? (_isInRoleHandler =new IsInRoleHandler()); }
            set { _isInRoleHandler = value; }
        }

        [Inject]
        public IMyInfoHandler MyInfoHandler {
            get { return _myInfoHandler??(_myInfoHandler=new MyInfoHandler()); }
            set { _myInfoHandler = value; }
        }

        [Inject]
        public IDefineUserHandler DefineUserHandler {
            get { return _defineUserHandler??(_defineUserHandler = new DefineUserHandler()); }
            set { _defineUserHandler = value; }
        }

        public void Initialize(IHostServer server) {
           server.Factory.Register("/logon",LogonHandler);
           server.Factory.Register("/logout",LogonHandler);
           server.Factory.Register("/isauth",IsAuthHandler);
           server.Factory.Register("/isrole",IsInRoleHandler);
           server.Factory.Register("/myinfo",MyInfoHandler);
           server.Factory.Register("/defusr",DefineUserHandler);
        }
    }
}
