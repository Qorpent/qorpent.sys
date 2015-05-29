using qorpent.v2.security.user;
using Qorpent;

namespace qorpent.v2.security.logon.services {
    public interface ISecureLogonService {
        string GetSalt(IUser record, IScope context);
        void CheckSecureInfo(SecureLogonInfo info, IUser record,IScope context);
    }
}