namespace qorpent.v2.security.user.services {
    public interface IUserStateChecker {
        bool IsLogable(IUser user);
        bool IsPasswordLogable(IUser user);
        bool IsSecureLogable(IUser user);
        UserActivityState GetActivityState(IUser user);
    }
}