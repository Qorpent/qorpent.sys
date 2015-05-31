using System.Security.Policy;
using Qorpent;
using Qorpent.IoC;

namespace qorpent.v2.security.management {
    [ContainerComponent(Lifestyle.Singleton,"userpolicy.cervice",ServiceType=typeof(IUserPolicyService))]
    public class UserPolicyService : ServiceBase, IUserPolicyService {

        public UserPolicyService() {
            
        }



        public UserPolicy GetNewUserPolicy(UserUpdateInfo update) {
            return new UserPolicy {
                Logable = true,
                ExpirationDays = 180,
                Active = true,
                MakePassRequest = true
            };
        }
    }
}