namespace qorpent.v2.security.management {
    public interface IUserPolicyService {
        UserPolicy GetNewUserPolicy(UserUpdateInfo update);
    }
}