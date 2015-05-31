using qorpent.v2.security.handlers.logon;
using qorpent.v2.security.handlers.management;
using qorpent.v2.security.handlers.userinfo;
using Qorpent.Host;
using Qorpent.IoC;

namespace qorpent.v2.security.handlers {
    [ContainerComponent(Lifestyle.Transient, "securityhandlers.setup", ServiceType = typeof (IHostServerInitializer))]
    internal class HandlerInitializer : IHostServerInitializer {
        private IDefineUserHandler _defineUserHandler;
        private IIsAuthHandler _isAuthHandler;
        private IIsInRoleHandler _isInRoleHandler;
        private ILogonHandler _logonHandler;
        private ILogoutHandler _logoutHandler;
        private IMyInfoHandler _myInfoHandler;
        private IRequireResetPasswordHandler _requireResetPasswordHandler;
        private IResetPasswordHandler _resetPasswordHandler;
        private ISendMailHandler _sendMailHandler;

        [Inject]
        public ILogonHandler LogonHandler {
            get { return _logonHandler ?? (_logonHandler = new LogonHandler()); }
            set { _logonHandler = value; }
        }

        [Inject]
        public ILogoutHandler LogoutHandler {
            get { return _logoutHandler ?? (_logoutHandler = new LogoutHandler()); }
            set { _logoutHandler = value; }
        }

        [Inject]
        public IIsAuthHandler IsAuthHandler {
            get { return _isAuthHandler ?? (_isAuthHandler = new IsAuthHandler()); }
            set { _isAuthHandler = value; }
        }

        [Inject]
        public IIsInRoleHandler IsInRoleHandler {
            get { return _isInRoleHandler ?? (_isInRoleHandler = new IsInRoleHandler()); }
            set { _isInRoleHandler = value; }
        }

        [Inject]
        public IMyInfoHandler MyInfoHandler {
            get { return _myInfoHandler ?? (_myInfoHandler = new MyInfoHandler()); }
            set { _myInfoHandler = value; }
        }

        [Inject]
        public IDefineUserHandler DefineUserHandler {
            get { return _defineUserHandler ?? (_defineUserHandler = new DefineUserHandler()); }
            set { _defineUserHandler = value; }
        }

        [Inject]
        public IRequireResetPasswordHandler RequireResetPasswordHandler {
            get {
                return _requireResetPasswordHandler ?? (_requireResetPasswordHandler = new RequireResetPasswordHandler());
            }
            set { _requireResetPasswordHandler = value; }
        }

        [Inject]
        public IResetPasswordHandler ResetPasswordHandler {
            get { return _resetPasswordHandler ?? (_resetPasswordHandler = new ResetPasswordHandler()); }
            set { _resetPasswordHandler = value; }
        }

        [Inject]
        public ISendMailHandler SendMailHandler {
            get { return _sendMailHandler ?? (_sendMailHandler = new SendMailHandler()); }
            set { _sendMailHandler = value; }
        }

        public void Initialize(IHostServer server) {
            server.Factory.Register("/logon", LogonHandler);
            server.Factory.Register("/logout", LogoutHandler);
            server.Factory.Register("/isauth", IsAuthHandler);
            server.Factory.Register("/isrole", IsInRoleHandler);
            server.Factory.Register("/myinfo", MyInfoHandler);
            server.Factory.Register("/defusr", DefineUserHandler);
            server.Factory.Register("/resetpwd", ResetPasswordHandler);
            server.Factory.Register("/resetpwdreq", RequireResetPasswordHandler);
            server.Factory.Register("/sendmail", SendMailHandler);
        }
    }
}