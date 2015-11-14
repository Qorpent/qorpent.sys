using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using qorpent.v2.security.authorization;
using qorpent.v2.security.messaging;
using qorpent.v2.security.user;
using qorpent.v2.security.user.storage;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.Log.NewLog;

namespace qorpent.v2.security.management {
    [ContainerComponent(Lifestyle.Singleton, "updateuser.processor", ServiceType = typeof (IUpdateUserProcessor))]
    public class UpdateUserProcessor : ServiceBase, IUpdateUserProcessor {
        private IUpdateUserChecker _checker;
        private IUserPolicyService _userPolicy;

        public UpdateUserProcessor() {
            LoggyName = "updateuser.processor";
        }

        [Inject]
        public IUpdateUserChecker Checker {
            get { return _checker ?? (_checker = new UpdateUserChecker()); }
            set { _checker = value; }
        }

        [Inject]
        public IUserService Users { get; set; }

        [Inject]
        public IRoleResolverService Roles { get; set; }

        [Inject]
        public IUserPolicyService UserPolicy {
            get { return _userPolicy ?? (_userPolicy = new UserPolicyService()); }
            set { _userPolicy = value; }
        }

        [Inject]
        public IUserMessagingService Messendger { get; set; }



        public UpdateResult[] DefineUser(IIdentity actor, UserUpdateInfo[] updateinfos, IUser target = null,
            bool store = true) {
            return  updateinfos.Select(_=>UpdateResult(actor, target, store, _)).ToArray();
        }

        private UpdateResult UpdateResult(IIdentity actor, IUser target, bool store, UserUpdateInfo updateinfo
          ) {
            UpdateResult res = null;
            Logg.Info(new {updateusr = "start", usr = actor.Name, info = updateinfo}.stringify());
            if (string.IsNullOrWhiteSpace(updateinfo.Login)) {
                updateinfo.Login = actor.Name;
            }
            target = target ?? Users.GetUser(updateinfo.Login);
            var newuser = target == null;
            if (!updateinfo.HasDelta(target)) {
                return new UpdateResult {Ok = true, Message="notchanged", ResultUser = target};
            }

            res = Checker.ValidateUpdate(actor, updateinfo, target);
            if (!res.Ok) {
                Logg.Warn(
                    new {updateusr = "invalid", validation = res, usr = actor.Name, info = updateinfo}.stringify());

                return res;
            }
            if (null == target) {
                target = new User {Login = updateinfo.Login};
            }
            updateinfo.Apply(target);
            UserPolicy policy = null;
            if (newuser) {
                policy = UserPolicy.GetNewUserPolicy(updateinfo);
            }
            if (null != policy) {
                policy.Apply(target);
            }
            if (store) {
                Users.Store(target);
            }
            if (null != policy && policy.MakePassRequest && null != Messendger) {
                if (newuser) {
                    Messendger.SendWelcome(target);
                }
            }
            Logg.Info(new {updateusr = "finish", result = target, usr = actor.Name, info = updateinfo}.stringify());
            Users.Clear();
            Roles.Clear();
            res = new UpdateResult {Ok = true, ResultUser = target};
            return res;
        }
    }
}