using System;
using Qorpent.Log;

namespace qorpent.v2.security.handlers {
    /// <summary>
    /// 
    /// </summary>
    public class UserOpAttribute:Attribute {
        public UserOpAttribute(string name) {
            this.Name = name;
        }
        public bool IsUserOperation = true;
        public LogLevel ErrorLevel = LogLevel.Error;
        public LogLevel ExceptionLevel = LogLevel.None;
        public LogLevel SuccessLevel = LogLevel.Trace;
        public bool TreatFalseAsError = false;
        public bool Secure { get; set; }
        public string Prefix = "user.op.";
        public string Name;

        public string GetName() {
            return Prefix + (Secure ? "secure." : "") + Name;
        }
    }
}