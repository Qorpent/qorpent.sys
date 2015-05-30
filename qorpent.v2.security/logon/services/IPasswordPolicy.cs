namespace qorpent.v2.security.logon.services {
    public interface IPasswordPolicy {
        bool Ok { get; }
        bool Good { get; }
    }
}