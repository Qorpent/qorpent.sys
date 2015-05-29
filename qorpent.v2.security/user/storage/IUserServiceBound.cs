namespace qorpent.v2.security.user.storage {
    public interface IUserServiceBound {
        IUserService UserService { get; set; }
    }
}