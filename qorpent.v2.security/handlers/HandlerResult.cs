namespace qorpent.v2.security.handlers {
    public class HandlerResult {
        public int State = 200;
        public object Result = true;
        public string Mime = "application/json";
        public static readonly HandlerResult Null = new HandlerResult {Result = null};
        public object Data;
    }
}