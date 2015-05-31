using System;

namespace qorpent.v2.security.authorization {
    public class NotAuthReaction {
        public string Redirect { get; set; }
        public bool Process { get; set; }
        public Exception Error { get; set; }
    }
}