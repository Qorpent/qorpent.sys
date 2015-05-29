namespace qorpent.v2.security.user.storage {
    /// <summary>
    /// 
    /// </summary>
    public interface IWriteableUserSource {
        bool IsDefault { get;  }
        IUser Store(IUser user);
    }
}