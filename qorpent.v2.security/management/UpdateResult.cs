using qorpent.v2.security.user;

namespace qorpent.v2.security.management {
    public class UpdateResult {
        public bool Ok { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public IUser ResultUser { get; set; }
    }
}