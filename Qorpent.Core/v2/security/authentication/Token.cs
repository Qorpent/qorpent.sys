using System;

namespace qorpent.v2.security.authentication {
    public class Token {
        public string User { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expire { get; set; }
        public string ImUser { get; set; }
        public string Metrics { get; set; }
        public bool IsAdmin { get; set; }
    }
}