using System.Collections.Generic;

namespace Qorpent.Dot {
	/// <summary>
	/// Базовые свойства элементов графа
	/// </summary>
	public abstract class GraphElementBase {
		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string, string> Attributes = new Dictionary<string, string>();
		/// <summary>
		/// Защищенный метод доступа к атрибутам на чтение
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public string Get(string code) {
			if (Attributes.ContainsKey(code)) {
				return Attributes[code];
			}
			return string.Empty;
		}
		/// <summary>
		/// Установить атрибут
		/// </summary>
		/// <param name="code"></param>
		/// <param name="value"></param>
		public void Set(string code, string value) {
			Attributes[code] = value;
		}

		/// <summary>
		/// Заголовок
		/// </summary>
		public string Label {
			get { return Get(DotConstants.LabelAttribute); }
			set { Set(DotConstants.LabelAttribute,value); }
		}
	}
}