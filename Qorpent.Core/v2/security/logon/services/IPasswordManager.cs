using qorpent.v2.security.user;

namespace qorpent.v2.security.logon.services {
    public interface IPasswordManager {
        void SetPassword(IUser target, string password, bool ignorepolicy = false, string salt = null);
        IPasswordPolicy GetPolicy(string password);
        bool MatchPassword(IUser target, string password);
        void ResetPassword(IUser target, string password, string key);
        void MakeRequest(IUser target, int expireminutes, string email = null);
    }
}