using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Qorpent.Utils.Extensions;
using static System.Int32;

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
        /// <summary>
        /// Resolution with case-symbol-ignorance with weighting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <param name="name"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static T FuzzyResolve<T>(this IScope scope, string name, T def = default(T)) {
            
            //first match is exact match
            if (scope.ContainsKey(name)) {
                return scope.Get(name, def);
            }
            var targetKey = StringExtensions.FuzzySelect(name, scope.GetKeys());
            if (!string.IsNullOrWhiteSpace(targetKey)) {
                return scope.Get<T>(targetKey);
            }
            return def;
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