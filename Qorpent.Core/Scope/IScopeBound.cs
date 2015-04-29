namespace Qorpent {
    public interface IScopeBound {
        object Get(IScope scope, string key, ScopeOptions options);
    }
}