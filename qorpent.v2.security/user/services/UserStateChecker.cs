using System;
using qorpent.v2.security.user.storage;
using Qorpent.IoC;

namespace qorpent.v2.security.user.services
{
    [ContainerComponent(Lifestyle.Transient,"user.statechecker",ServiceType=typeof(IUserStateChecker))]
    public class UserStateChecker:IUserStateChecker
    {

        [Inject]
        public IUserService UserService { get; set; }

        public bool IsLogable(IUser user) {
            if (null == user) return false;
            if (!user.Logable) return false;
            if (user.IsGroup) return false;
            return true;
        }

        public bool IsPasswordLogable(IUser user) {
            if (!IsLogable(user)) return false;
            if (string.IsNullOrWhiteSpace(user.Hash)) return false;
            if (string.IsNullOrWhiteSpace(user.Salt)) return false;
            return true;
        }

        public bool IsSecureLogable(IUser user) {
            if (!IsLogable(user)) return false;
            if (string.IsNullOrWhiteSpace(user.PublicKey)) return false;
            return true;
        }


        public UserActivityState GetActivityState(IUser user) {
            var result = UserActivityState.Ok;
            result = CheckSelf(user, result);
            if (RequireMasterChecking(user)) {
                result = CheckMaster(user, result);
            }
            return result;
        }

        private UserActivityState CheckMaster(IUser user, UserActivityState result) {
            var usrvb = user as IUserServiceBound;
            IUserService usersrv = UserService;
            if (null != usrvb) {
                usersrv = usrvb.UserService ?? usersrv;
            }
            if (null == usersrv) {
                result |= UserActivityState.MasterNotChecked;
            }
            else {
                var grp = usersrv.GetUser(user.MasterGroup + "@groups");
                if (null == grp) {
                    result |= UserActivityState.InvalidMaster;
                }
                else {
                    if (!grp.Active) {
                        result |= UserActivityState.MasterBaned;
                    }
                    if (grp.Expire < DateTime.Now) {
                        result |= UserActivityState.MasterExpired;
                    }
                }
            }
            return result;
        }

        private static bool RequireMasterChecking(IUser user) {
            return !user.IsGroup && !string.IsNullOrWhiteSpace(user.MasterGroup);
        }

        private static UserActivityState CheckSelf(IUser user, UserActivityState result) {
            if (!user.Active) {
                result |= UserActivityState.Baned;
            }
            if (user.Expire < DateTime.Now) {
                result |= UserActivityState.Expired;
            }
            
            return result;
        }
    }
}
