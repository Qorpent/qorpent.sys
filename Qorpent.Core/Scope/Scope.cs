using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent {
    public class Scope : IScope,ICollection {

        public Scope() {
            
        }
        public Scope(params object[] sources) {
            if (null != sources) {
                foreach (var source in sources) {
                    if (null == source) {
                        continue;
                    }
                    if (source is IScope) {
                        AddParent(source as IScope);
                    }
                    else if (source is XElement) {
                        ApplyXml(source);
                    }
                    else {
                        ApplySource(source);
                    }
                }
            }

        }


        public void ApplySource(object source) {
            var dict = source.ToDict();
            foreach (var o in dict) {
                this[o.Key] = o.Value;
            }
        }

        public void ApplyXml(object source) {
            var x = (XElement) source;
            this["__xmlname"] = x.Name.LocalName;
            if (!x.HasElements && !string.IsNullOrEmpty(x.Value)) {
                this["__xmlvalue"] = x.Value;
            }
            foreach (var attribute in x.Attributes()) {
                this[attribute.Name.LocalName] = attribute.Value;
            }
        }

        private ScopeOptions _options = new ScopeOptions();
        private readonly IList<IScope> _parents = new List<IScope>();
        protected readonly IDictionary<string, object> _storage = new ConcurrentDictionary<string, object>();
        private bool _useInheritance =true;

        public ScopeOptions Options {
            get { return _options; }
            set { _options = value; }
        }

        
        public T Get<T>(string key, T def = default(T), ScopeOptions options = null) {
            object result = Get(key,options);
            
            if (null == result) {
                return def;
            }
            return result.To<T>();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
            IList<string> processedKeys = new List<string>();
            foreach (var pair in _storage) {
                processedKeys.Add(pair.Key);
                yield return pair;
            }
            if (UseInheritance) {
                foreach (var parent in _parents) {
                    foreach (var pair in parent) {
                        if (processedKeys.Contains(pair.Key)) {
                            continue;
                        }
                        processedKeys.Add(pair.Key);
                        yield return pair;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item) {
            _storage.Add(item);
        }

        public void Clear() {
            _storage.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item) {
            if (_storage.Contains(item)) {
                return true;
            }
            if (UseInheritance) {
                return _parents.Any(_ => _.Contains(item));
            }
            return false;
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
            if (null == array) {
                throw new ArgumentNullException("array");
            }
            var idx = arrayIndex;
            if (idx < 0) {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }
            foreach (var item in this) {
                if (idx >= array.Length) {
                    throw new ArgumentException("source size exceed given array", "array");
                }
                array[idx] = item;
                idx++;
            }
        }

        public bool Remove(KeyValuePair<string, object> item) {
            if (_storage.Remove(item)) {
                return true;
            }

            if (UseInheritance) {
                return _parents.Any(parent => parent.Remove(item));
            }

            return false;
        }

        public void CopyTo(Array array, int index) {
            if (null == array)
            {
                throw new ArgumentNullException("array");
            }
            var idx = index;
            if (idx < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            var myarray = this.ToArray();
            myarray.CopyTo(array,index);
        }

        public int Count {
            get {
                IList<string> processedKeys = _storage.Select(pair => pair.Key).ToList();
                if (UseInheritance) {
                    foreach (var pair in 
                        _parents.SelectMany(parent => parent.Where(pair => !processedKeys.Contains(pair.Key)))) {
                        processedKeys.Add(pair.Key);
                    }
                }
                return processedKeys.Count;
            }
        }

        public object SyncRoot {
            get { return ((ICollection) _storage).SyncRoot; }
        }

        public bool IsSynchronized {
            get { return ((ICollection) _storage).IsSynchronized; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public bool ContainsKey(string key) {
            return ContainsKey(key, Options);
        }

        public void Add(string key, object value) {
            Set(key, value);
        }

        public bool Remove(string key) {
            return _storage.Remove(key);
        }

        public bool TryGetValue(string key, out object value) {
            value = null;
            if (ContainsKey(key)) {
                value = Get(key, Options);
                return true;
            }
            return false;
        }

        public object this[string key] {
            get { return Get(key, Options); }
            set { Set(key, value); }
        }

        public ICollection<string> Keys {
            get {
                IList<string> processedKeys = _storage.Select(pair => pair.Key).ToList();
                if (UseInheritance) {
                    foreach (
                        var pair in
                            _parents.SelectMany(parent => parent.Where(pair => !processedKeys.Contains(pair.Key)))) {
                        processedKeys.Add(pair.Key);
                    }
                }
                return processedKeys;
            }
        }

        public ICollection<object> Values {
            get {
                IList<string> processedKeys = new List<string>();
                IList<object> result = new List<object>();
                foreach (var pair in _storage) {
                    processedKeys.Add(pair.Key);
                    result.Add(pair.Value);
                }
                if (UseInheritance) {
                    foreach (
                        var pair in
                            _parents.SelectMany(parent => parent.Where(pair => !processedKeys.Contains(pair.Key)))) {
                        processedKeys.Add(pair.Key);
                        result.Add(pair.Value);
                    }
                }
                return result;
            }
        }

        public bool UseInheritance {
            get { return _useInheritance; }
            set { _useInheritance = value; }
        }

        public IScope Set(string key, object value) {
            if (null == key) {
                throw new ArgumentNullException("key");
            }
            _storage[key] = value;
            return this;
        }

        public object Get(string key, ScopeOptions options = null) {
            options = options ?? Options;
            if (SimplifiedContainsKey(key, options)) {
                return PreparedValue(key, options);
            }
            if (-1 != key.IndexOfAny(new[] {'.', '^'})) {
                options = options.Copy();
                var skips = 0;
                foreach (var c in key) {
                    if (c == '.') {

                        options.SkipResults++;                     
                        skips++;
                    }
                    else if (c == '^') {
                        options.SkipLevels++;
                        skips++;
                    }
                    else {
                        break;
                    }
                }
                var correctedkey = key.Substring(skips);
                return PreparedValue(correctedkey,options);
            }
            return null;
        }

        private object PreparedValue(string key, ScopeOptions options) {
            object result = null;
            if (key.Contains('.') && !ContainsKey(key)) {
                var split = key.Split('.');
                result = NativeGet(split[0], options);
                if (null != result) {
                    var jpath = string.Join(".", split.Skip(1));
                    result = Experiments.Json.Get(result, jpath);
                }
            }
            else
            {
                result = NativeGet(key, options);
            }
            if (result is IScopeBound) {
                return ((IScopeBound) result).Get(this, key, options);
            }
            return result;
        }


        public bool ContainsKey(string key, ScopeOptions options) {
            if (null == key) {
                return false;
            }
            var directMatch = GetDirectMatch(key);
            if (directMatch || options.DirectMatchOnly) return directMatch;
            if (options.KeySimplification != SimplifyOptions.None) {
                if (SimplifiedContainsKey(key, options)) return true;
            }
            if ( (key.StartsWith(".") || key.StartsWith("^"))) {

                var rekey = Regex.Match(key, @"^[\.\^]+([\s\S]+)$").Groups[1].Value;
                return ContainsKey(rekey,options);
            }
            return false;
        }

        public bool ContainsOwnKey(string key) {
            if (null == key) return false;
            var directMatch = GetDirectMatch(key,false);
            if (directMatch) return true;
            if (this.Options.KeySimplification != SimplifyOptions.None)
            {
                if (SimplifiedContainsKey(key, this.Options,false)) return true;
            }
            return false;
        }

        private bool GetDirectMatch(string key,bool deep = true) {
            if (this._storage.ContainsKey(key)) return true;
            if (deep) {
                foreach (var parent in _parents) {
                    var result = parent.ContainsKey(key, ScopeOptions.DirectMatch);
                    if (result) return true;
                }
            }
            return false;
        }

        private bool SimplifiedContainsKey(string key, ScopeOptions options,bool deep =true) {

            options = options ?? Options;
            key = key.Simplify(options.KeySimplification);
            if (deep) {
                return GetKeys(options).Any(_ => key == _.Simplify(options.KeySimplification));
            }
            return _storage.Keys.Any(_ => key == _.Simplify(options.KeySimplification));

        }

        public IEnumerable<string> GetKeys(ScopeOptions options = null) {
            options = options ?? Options;
            IList<string> processedkeys = new List<string>();
            if (options.SkipLevels == 0) {
                foreach (var o in _storage.Keys) {
                    processedkeys.Add(o);
                    yield return o;
                }
            }
            if (options.UseInheritance && UseInheritance) {
                foreach (var parent in _parents) {
                    foreach (var key in parent.GetKeys(options)) {
                        if (processedkeys.Contains(key)) {
                            continue;
                        }
                        processedkeys.Add(key);
                        yield return key;
                    }
                }
            }
        }

        public void AddParent(IScope parent) {
            
            if (null != parent && !_parents.Contains(parent) && this!=parent) {
                _parents.Add(parent);
            }
        }

        public void RemoveParent(IScope parent) {
            _parents.Remove(parent);
        }

        public IEnumerable<IScope> GetParents() {
            return _parents.ToArray();
        }

        public void ClearParents() {
            _parents.Clear();
        }

        public void Stornate() {

            foreach (
                var pair in
                    _parents.Reverse()
                        .ToArray()
                        .SelectMany(parent => parent.Where(pair => !_storage.ContainsKey(pair.Key)))) {
                _storage[pair.Key] = pair.Value;
            }
        }

        object IScope.this[string key, ScopeOptions options] {
            get { return Get(key, options); }
        }

        private object NativeGet(string key, ScopeOptions options) {
            if (null == key) {
                return null;
            }
            options = options ?? Options;





            var resultCount = options.ResultCount;
            object result = null;
            if (options.SkipLevels == 0) {
                var localkey = FindKey(key, options);
                if (null != localkey) {
                    result = _storage[localkey];
                    resultCount ++;
                }
            }
            if (result != null && options.SkipResults < resultCount) {
                return result;
            }
            if (options.UseInheritance && UseInheritance) {
                var parentOptions = options.LevelUp(resultCount);
                
                var usableParent = _parents.FirstOrDefault(_ => _.ContainsKey(key, parentOptions));
                if (null != usableParent) {
                    result = usableParent.Get(key, parentOptions);
                    if (null != result) {
                        return result;
                    }
                }
            }
            if (null != result && options.LastOnSkipOverflow) {
                if (options.TreatFirstDotAsLevelUp && resultCount == 1 && options.Level==0) {
                    return null;
                }
                return result;
            }
            return null;
        }

        private string FindKey(string key, ScopeOptions options) {
            if (null == key) {
                return null;
            }
            key = key.Simplify(options.KeySimplification);
            return _storage.Keys.FirstOrDefault(_ => key == _.Simplify(options.KeySimplification));
        }
        /// <summary>
        /// Scope - compatible set parent method
        /// </summary>
        /// <param name="cfgbase"></param>
        public void SetParent(IScope parentScope) {
            _parents.Clear();
            AddParent(parentScope);
        }

        public IScope GetParent() {
            return _parents.FirstOrDefault();
        }

        public static readonly IScopeBound Null = new ScopeNull();

        public static IScopeBound Append(object extension, string delimiter = " ", string refKey="") {
            return new ScopeAppend(extension,delimiter,refKey);
        }
    }
}