using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Config {
	/// <summary>
	/// ������� ����� ��� ������������
	/// </summary>
	public  class ConfigBase : IConfig {



		/// <summary>
		/// ���������� ��������� �����
		/// </summary>
		protected IDictionary<string, object> options = new Dictionary<string, object>();

		private bool _freezed;
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
		/// ���������
		/// </summary>
		public void Freeze() {
			_freezed = true;
		}

		private void LoadElement(XElement e, string root) {
			foreach (var a in e.Attributes()) {
				var optname = string.IsNullOrWhiteSpace(root)?(a.Name.LocalName):(root+"."+a.Name.LocalName);
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
			if (_freezed) throw new Exception("config freezed");
			options[name] = value;
		}

		/// <summary>
		/// �������� ��������� �����
		/// </summary>
		/// <param name="name">��� �����</param>
		/// <param name="def">�������� �� ���������</param>
		/// <returns></returns>
		public string Get(string name, string def = "") {
			if (!options.ContainsKey(name)) {
				if (null != _parent) {
					return _parent.Get(name, def);
				}
				return def;
			}
			return options[name].ToStr();
		}
		/// <summary>
		/// �������� ����������� �������������� �����
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		public T Get<T>(string name, T def = default(T)) {
			if (!options.ContainsKey(name)) {
				if (null != _parent)
				{
					return _parent.Get(name, def);
				}
				return def;
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
	}
}