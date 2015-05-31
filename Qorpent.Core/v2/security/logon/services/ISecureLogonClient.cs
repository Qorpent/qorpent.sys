namespace qorpent.v2.security.logon.services {
    public interface ISecureLogonClient {
        SecureLogonInfo GetLogonInfo(string salt, byte[] certificate);
    }
}