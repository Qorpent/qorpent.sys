using System;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent {
    public static class ScopeExtensions {
		public static T SafeGet<T>(this IScope scope, string name, T def = default (T)) {
			if (null == scope) return def;
			return scope.Get(name, def);
		}
        public static object ResolveBest(this IScope scope,params string[] names) {
            if (null == scope) return null;
            return names.Select(name => scope.Get(name)).FirstOrDefault(subresult => null != subresult);
        }
        public static string ResolveBestString(this IScope scope, params string[] names)
        {
            if (null == scope) return null;
            return names.Select(name => scope.Get(name).ToStr()).FirstOrDefault(subresult => !string.IsNullOrWhiteSpace(subresult));
        }

        public static T Ensure<T>(this IScope scope, string key, T def) {
            if (scope.ContainsKey(key)) return scope.Get(key, def);
            scope.Set(key,def);
            return def;
        }
        public static T Ensure<T>(this IScope scope, string key, Func<T> def)
        {
            if (scope.ContainsKey(key)) return scope.Get<T>(key);
            scope.Set(key, def());
            return scope.Get<T>(key);
        }

        public static void SetParent(this IScope scope, IScope parent) {
            foreach (var source in scope.GetParents().ToArray()) {
                scope.RemoveParent(source);
            }
            scope.AddParent(parent);
        }
        public static IScope GetParent(this IScope scope) {
            return scope.GetParents().FirstOrDefault();
        }
    }
}