using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Config {
	/// <summary>
	/// Базовый класс для конфигураций
	/// </summary>
	public partial class ConfigBase : IConfig {


		/// <summary>
		/// Внутреннее хранилище опций
		/// </summary>
		protected IDictionary<string, object> options = new Dictionary<string, object>();
		/// <summary>
		/// Признак заморозки
		/// </summary>
		protected bool _freezed;
		private IConfig _parent;

		/// <summary>
		/// Конвертирует XML в словарь
		/// </summary>
		/// <param name="xml"></param>
		public void Load(XElement xml) {
			if (null == xml) return;
			string root = "";
			var e = xml;
			LoadElement(e, root);
		}
		/// <summary>
		/// Устанавливает родительский конфиг
		/// </summary>
		/// <param name="config"></param>
		public void SetParent(IConfig config) {
			_parent = config;
		}

		/// <summary>
		/// Заморозка
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
		/// Установить значение параметра
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void Set(string name, object value) {
			if (_freezed) throw new Exception("config freezed");
			options[name] = value;
		}
		

		private T ReturnIerachical<T>(string name, T def) {
			IConfig parent = this;
			while (name.StartsWith(".")) {
				if (null != parent) {
					parent = parent.GetParent();
				}
				name = name.Substring(1);
			}
			if (null != parent) {
				return parent.Get(name,def);
			}
			else {
				return def;
			}
		}

		/// <summary>
		/// Получить приведенную типизированную опцию
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		public T Get<T>(string name, T def = default(T)) {
			
			if (name.StartsWith("."))
			{
				return ReturnIerachical(name, def);
			}
			if (!options.ContainsKey(name)) {
				if (null != _parent)
				{
					return _parent.Get(name, def);
				}
				if (typeof(string) == typeof(T))
				{
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
		/// Получение родителя
		/// </summary>
		/// <returns></returns>
		public IConfig GetParent() {
			return _parent;
		}

		/// <summary>
		/// Возвращает имена всех опций
		/// </summary>
		/// <param name="withParent"></param>
		/// <returns></returns>
		public IEnumerable<string> GetNames(bool withParent = false) {
			if (!withParent || null==_parent) return options.Keys;
			return options.Keys.Union(_parent.GetNames(true)).Distinct();
		}

		/// <summary>
		/// Сериализация конфига в заданном формате
		/// </summary>
		/// <param name="rendertype"></param>
		/// <returns></returns>
		public string ToString(ConfigRenderType rendertype ) {
			if (rendertype == ConfigRenderType.SimpleBxl) {
				return GenerateSimpleBxl();
			}
			else {
				throw new Exception("unknown format " + rendertype);
			}
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
	}
}