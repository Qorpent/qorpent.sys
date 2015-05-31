namespace qorpent.v2.security.user.storage {
    public interface IUserSourceBound {
        IUserSource UserSource { get; set; }
    }
}