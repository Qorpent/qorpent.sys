using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Config {
	/// <summary>
	///     ������� ����� ��� ������������
	/// </summary>
    [Obsolete("Use Scope instead")]
    public partial class ConfigBase : IConfig, ICollection {
        /// <summary>
        ///     ���������� ��������� ����������, ������������ ��������� ������������
        /// </summary>
	    private bool _useInheritance = true;
        /// <summary>
        ///     ������� ����� ��� ������������
        /// </summary>
		public ConfigBase() { }
		/// <summary>
		///     ������� ����� ��� ��������������
		/// </summary>
		public ConfigBase(IConfig parent) {
			SetParent(parent);
		}
	    /// <summary>
	    ///     ��������� ������������ �� ��������
	    /// </summary>
	    [IgnoreSerialize]
	    public bool UseInheritance {
            get { return _useInheritance; }
            set { _useInheritance = value; }
	    }
        /// <summary>
        ///     ������� ����� ��� ������������
        /// </summary>
		/// <param name="dict"></param>
		public ConfigBase(IDictionary<string, object> dict) {
			if (null != dict) {
				foreach (var d in dict) {
					Set(d.Key, d.Value);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dict"></param>
		public ConfigBase(IDictionary<string, string> dict) {
			if (null != dict) {
				foreach (var d in dict) {
					Set(d.Key, d.Value);
				}
			}
		}
		/// <summary>
		/// ���������� ��������� �����
		/// </summary>
		protected IDictionary<string, object> options = new Dictionary<string, object>();
		/// <summary>
		/// ������� ���������
		/// </summary>
		protected bool _freezed;
		private IConfig _parent;
	    /// <summary>
		/// ������������ XML � �������
		/// </summary>
		/// <param name="xml"></param>
		public void Load(XElement xml) {
			if (null == xml) return;
			string root = "";
			var e = xml;
			LoadElement(e, root);
		}
		/// <summary>
		/// ������������� ������������ ������
		/// </summary>
		/// <param name="config"></param>
		public void SetParent(IConfig config) {
			_parent = config;
		}
		/// <summary>
		/// ����� ������������� �������
		/// </summary>
		public void RemoveParent(){
			_parent = null;
		}

		/// <summary>
		/// ���������
		/// </summary>
		public void Freeze() {
			_freezed = true;
		}

		private void LoadElement(XElement e, string root) {
			foreach (var a in e.Attributes()) {
				var optname = string.IsNullOrWhiteSpace(root) ? (a.Name.LocalName) : (root+"."+a.Name.LocalName);
				options[optname] = a.Value;
			}

			var text = e.Nodes().OfType<XText>().FirstOrDefault();
			if (null != text) {
				options[root] = text.Value;
			}

			root = string.IsNullOrWhiteSpace(root) ? e.Name.LocalName : (root + "." + e.Name.LocalName);
			foreach (var c in e.Elements()) {
				LoadElement(c,root);
			}
		}

		/// <summary>
		/// ���������� �������� ���������
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void Set(string name, object value) {
			if (_freezed) {
			    throw new Exception("config freezed");
			}

			options[name] = value;

		}
	

		private T ReturnIerachical<T>(string name, T def) {
			var basis = name.Replace(".", "");
			var skips = name.Count(_ => _ == '.');
			var result = AllByName(basis).Skip(skips).FirstOrDefault();
			
            if (null == result) {
				result = AllByName(basis).Last();
			}

			if (null == result) {
			    return def;
			}

			return (T) result;
		}
		/// <summary>
		///     ���������� � ������� �������� �������� ������� � ������ ��� ��� �������������
		/// </summary>
		/// <typeparam name="T">��������� �������� �������� �������</typeparam>
		/// <param name="key">���� �������� �������</param>
		/// <returns>�������� �������� �� �����</returns>
		public T Ensure<T>(string key) where T : new() {
			return Ensure(key, new T());
		}
		/// <summary>
		///		���������� � ������� �������� � ��������������� ���������� � ���� �������� �� ������������,
		///		�������������� ������� ������� �� ����������� ����� ��������� ���������
		/// </summary>
		/// <typeparam name="T">��������� �������� �������� �������</typeparam>
		/// <param name="key">���� �������� �������</param>
		/// <param name="value">�������� ��� �������������</param>
		/// <returns>�������� �������� �� �����</returns>
		public T Ensure<T>(string key, T value) {
			if (!Exists<T>(key)) {
				Set(key, value);
			}
			return Get<T>(key);
		}
		/// <summary>
		///		���������� � ������� �������� � ��������������� ���������� � ���� �������� �� ������������,
		///		�������������� ������� ������� �� ����������� ����� ��������� ���������
		/// </summary>
		/// <typeparam name="T">��������� �������� �������� �������</typeparam>
		/// <param name="key">���� �������� �������</param>
		/// <param name="value">�������� ��� �������������</param>
		/// <returns>�������� �������� �� �����</returns>
		public T Ensure<T>(string key, Func<T> value) {
			if (!Exists<T>(key)) {
				Set(key, value());
			}
			return Get<T>(key);
		}
		/// <summary>
		///		��������� ������� ��������������� �������� ������� �� �����
		/// </summary>
		/// <typeparam name="T">��������� �������� �������</typeparam>
		/// <param name="key">���� �������� �������</param>
		/// <returns>������� ������� ��������������� �������� �������</returns>
		public bool Exists<T>(string key) {
			var current = Get<object>(key);
			if (current != null) {
				return current is T;
			}
			return false;
		}
		/// <summary>
		/// ���������� ��� �� ����� 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public IEnumerable<object> AllByName(string name) {
			if (options.ContainsKey(name)) {
				yield return options[name];
			}

            if (!_useInheritance)
            {
                yield return null;
            }
			
            if (
                (null != _parent)
            ) {
                

				foreach (var p in ((ConfigBase) _parent).AllByName(name)) {
					yield return p;
				}
			}
		}
        /// <summary>
        /// �������� ������ �� ������ �������
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
	    public string ResolveBest(params string[] names) {
            foreach (var name in names) {
                var subresult = Get(name, "");
                if (!string.IsNullOrWhiteSpace(subresult)) {
                    return subresult;
                }
            }
            return "";
        }
	    /// <summary>
	    /// �������� ����������� �������������� �����
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <param name="name"></param>
	    /// <param name="def"></param>
	    /// <returns></returns>
	    public T Get<T>(string name, T def = default(T)){
		    if (null == name || name.Length == 0){
			    throw new ArgumentException("name");
		    }
			if (name[0]=='.') {
				return ReturnIerachical(name, def);
			}
			
			if (!options.ContainsKey(name)){
				if (_stornated) return def;
				if (null != _parent) {
				    if (_useInheritance) {
				        return _parent.Get(name, def);
				    }
				}

				if (typeof(string) == typeof(T)) {
					if (Equals(def, null)) {
						return (T)(object)string.Empty;
					}
				}
				return def;
			}

			if (typeof (T) == typeof (object)) {
				return (T)options[name];
			}

			return options[name].To<T>();
		}

		/// <summary>
		/// ��������� ��������
		/// </summary>
		/// <returns></returns>
		public IConfig GetParent() {
			return _parent;
		}

		/// <summary>
		///     ������������ ������� � �������� �������
		/// </summary>
		/// <param name="rendertype"></param>
		/// <returns></returns>
		public string ToString(ConfigRenderType rendertype ) {
		    if (rendertype == ConfigRenderType.SimpleBxl) {
				return GenerateSimpleBxl();
			}

		    throw new Exception("unknown format " + rendertype);
		}
		/// <summary>
		///		���������� �������� ������� ������ �������
		/// </summary>
		/// <param name="config">������ ��� ����������</param>
		/// <param name="insertNotExists">����� �� ��������� �������� � ������� ������ �� �����������, ���� � ������ ������� ������ ����� �� ����������</param>
		public void Override(IConfig config, bool insertNotExists = true) {
			Override(config as IDictionary<string, object>, insertNotExists);
		}
		/// <summary>
		///		���������� �������� ������� ������ �������
		/// </summary>
		/// <param name="config">������ ��� ����������</param>
		/// <param name="insertNotExists">����� �� ��������� �������� � ������� ������ �� �����������, ���� � ������ ������� ������ ����� �� ����������</param>
		public void Override(IDictionary<string, object> config, bool insertNotExists = true) {
			Override(config as IEnumerable<KeyValuePair<string, object>>, insertNotExists);
		}
		/// <summary>
		///		���������� �������� ������� ������ �������
		/// </summary>
		/// <param name="config">������ ��� ����������</param>
		/// <param name="insertNotExists">����� �� ��������� �������� � ������� ������ �� �����������, ���� � ������ ������� ������ ����� �� ����������</param>
		public void Override(IEnumerable<KeyValuePair<string, object>> config, bool insertNotExists = true) {
			foreach (var item in config) {
				if (insertNotExists) {
					this[item.Key] = item.Value;
				} else {
					if (Exists(item.Key)) {
						this[item.Key] = item.Value;
					}
				}
			}
		}
		/// <summary>
		///     SQL-�������� LIKE ������� ����� �������� �� ����� ��������
		/// </summary>
		/// <param name="pattern">�������� �������</param>
		/// <returns>������������ �������� ��� ������� �� ����� ��������� �������</returns>
		public IEnumerable<KeyValuePair<string, object>> Like(string pattern) {
			return this.Where(_ => Regex.IsMatch(_.Key, pattern.Replace("%", ".*").Replace("_", ".")));
		}
		/// <summary>
		///		���������� ������� ������� ������� ������� �� �����
		/// </summary>
		/// <param name="key">����</param>
		/// <returns>������� ������� �������� ������� �� �����</returns>
		public bool Exists(string key) {
			return Get<object>(key) != null;
		}
	    private string GenerateSimpleBxl() {
			var asdict = this as IDictionary<string,object>;
			var sb = new StringBuilder();
			sb.AppendLine("config");
			foreach (var d in asdict) {
				sb.Append("\t");
				sb.Append(d.Key);
				var strval = d.Value.ToStr();
				if (string.IsNullOrEmpty(strval)) {
					sb.AppendLine(" = \"\"");
				}else if (strval.All(c => char.IsLetterOrDigit(c) ||c=='.')) {
					sb.AppendLine(" = "+strval);
				}else if (strval.Any(c => c == '"' || c == '\r' || c == '\n')) {
					sb.AppendLine(" = \"\"\"");
					sb.AppendLine(strval);
					sb.AppendLine("\t\"\"\"");
				}
				else {
					sb.Append(" = \"");
					sb.Append(strval);
					sb.AppendLine("\"");
				}
				
			}
			return sb.ToString();
		}

		private bool _stornated = false;

	    public ConfigBase(object advctx) {
	        if (advctx is IConfig) {
	            this.SetParent((IConfig) advctx);
	        }
	        else {
	            var d = advctx as IDictionary<string, object>;
	            if (null == d) d = advctx.ToDict();
                foreach (var o in d)
                {
                    Set(o.Key,o.Value);
                }
	        }
	        
	    }

	    /// <summary>
		/// ������� ���������� ��������� �� ������������ ������� �����
		/// </summary>
		public void Stornate(){
			lock (this){
				if (_stornated) return;
				if (null != _parent){
					_parent.Stornate();
					foreach (var _ in _parent)
					{
						if (!options.ContainsKey(_.Key))
						{
							options[_.Key] = _.Value;
						}
					}
				}
				
				_stornated = true;
			}
		}

	    public void CopyTo(Array array, int index) {
	        throw new NotImplementedException();
	    }

	    public int Count {
	        get { return options.Count; }
	    }

	    public object SyncRoot {
	        get { return ((ICollection) options).SyncRoot; }
	    }

	    public bool IsSynchronized {
	        get { return ((ICollection) options).IsSynchronized; }
	    }
	}
}