using System;

namespace qorpent.v2.console {
    public class ConsoleCommandResult {
        public int Status { get; set; }
        public string StatusDescription { get; set; }
        public Exception Error { get; set; } 
            
            
    }
}