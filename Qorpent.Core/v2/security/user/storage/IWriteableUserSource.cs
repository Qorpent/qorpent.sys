namespace qorpent.v2.security.user.storage {
    /// <summary>
    /// </summary>
    public interface IWriteableUserSource {
        bool IsDefault { get; }
        bool WriteUsersEnabled { get; }
        IUser Store(IUser user);
    }
}