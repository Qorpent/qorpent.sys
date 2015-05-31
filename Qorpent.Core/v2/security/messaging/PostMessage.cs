using System;
using System.Collections.Generic;

namespace qorpent.v2.security.messaging {
    public class PostMessage {
        public string Id { get; set; }
        public string[] Addresses { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public IDictionary<string, object> Tags { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime SentTime { get; set; }
        public bool WasSent { get; set; }
        public string Message { get; set; }
        public bool CanUseDefault { get; set; }
        public DateTime CreateTime { get; set; }
    }
}