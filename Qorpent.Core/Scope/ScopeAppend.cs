using Qorpent.Utils.Extensions;

namespace Qorpent {
    public class ScopeAppend : IScopeBound {
        private readonly object _extension;
        private readonly string _delimiter;
        private readonly string _refKey;

        public ScopeAppend(object extension, string delimiter = " ",string refKey = "") {
            _extension = extension;
            _delimiter = delimiter;
            _refKey = refKey;
        }
        public object Get(IScope scope, string key, ScopeOptions options) {
            var realKey = string.IsNullOrWhiteSpace(_refKey) ? ("^"+key) : _refKey;
            var basis = scope.Get(realKey, "") ?? "";
            if (!string.IsNullOrWhiteSpace(basis)) {
                basis += _delimiter;
            }
            return basis + _extension.ToStr();
        }
    }
}