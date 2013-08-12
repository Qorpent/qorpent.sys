using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Config {
	/// <summary>
	/// Вариант конфигураци, совместимый по интерфейсу со словарем
	/// </summary>
	public partial class ConfigBase : IDictionary<string,object> {
		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() {
			return EnumerateAll().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return EnumerateAll().GetEnumerator();
		}

		private IEnumerable<KeyValuePair<string,object>>  EnumerateAll() {
			return ((IDictionary<string,object>)this).Keys.Select(_ => new KeyValuePair<string, object>(_, Get<object>(_)));
		}

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item) {
			Set(item.Key, item.Value);
		}

		void ICollection<KeyValuePair<string, object>>.Clear() {
			options.Clear();
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item) {
			var val = Get<object>(item.Key);
			if (Equals( val,item.Value)) {
				return true;
			}
			return false;
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
			var idx = arrayIndex;
			foreach (var p in this) {
				array[idx] = new KeyValuePair<string, object>(p.Key, p.Value);
				idx++;
			}
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item) {
			return ((IDictionary<string,object>)this).Remove(item.Key);
		}

		int ICollection<KeyValuePair<string, object>>.Count {
			get { return ((IDictionary<string,object>)this).Keys.Count; }
		}

		bool ICollection<KeyValuePair<string, object>>.IsReadOnly {
			get { return _freezed; }
		}

		bool IDictionary<string, object>.ContainsKey(string key) {
			if (key.StartsWith(".")) {
				var keyparent = this.GetParent() as IDictionary<string,object>;
				if (null == keyparent) return false;
				return keyparent.ContainsKey(key.Substring(1));
			}
			if (options.ContainsKey(key)) return true;
			var parent = GetParent() as IDictionary<string,object>;
			if(null!=parent) {
				return parent.ContainsKey(key);
			}
			return false;
		}

		void IDictionary<string, object>.Add(string key, object value) {
			this[key] = value;
		}

		bool IDictionary<string, object>.Remove(string key) {
			options.Remove(key);
			return true;
		}

		bool IDictionary<string, object>.TryGetValue(string key, out object value) {
			value = this[key];
			return true;
		}

		/// <summary>
		/// Gets or sets the element with the specified key.
		/// </summary>
		/// <returns>
		/// The element with the specified key.
		/// </returns>
		/// <param name="key">The key of the element to get or set.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
		public object this[string key] {
			get { return Get<object>(key); }
			set {  Set(key, value); }
		}

		ICollection<string> IDictionary<string, object>.Keys {
			get {
				var parent = GetParent() as IDictionary<string, object>;
				if (null == parent) {
					return options.Keys;
				}
				return parent.Keys.Union(options.Keys).Distinct().ToArray();
			}
		}

		ICollection<object> IDictionary<string, object>.Values {
			get { return((IDictionary<string,object>)this).Keys.Select(_ => this[_]).ToArray(); }
		}

		
	}
}