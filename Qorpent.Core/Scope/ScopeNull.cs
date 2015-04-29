namespace Qorpent {
    internal class ScopeNull  : IScopeBound {
        public object Get(IScope scope, string key, ScopeOptions options) {
            return null;
        }
    }
}