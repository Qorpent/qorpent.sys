using Qorpent.Config;

namespace Qorpent.Report {
	/// <summary>
	///		Запрос на элемент контента
	/// </summary>
	public class ContentItemQuery : ConfigBase, IContentItemQuery {
		/// <summary>
		///		Код элемента контента
		/// </summary>
		public string Code {
			get { return Get<string>("classname"); }
			set { Set("classname", value); }
		}
		/// <summary>
		///		Набор условий для выбора колонок
		/// </summary>
		public string Conditions {
			get { return Get<string>("conditions"); }
			set { Set("conditions", value); }
		}
		/// <summary>
		///     Год
		/// </summary>
		public int Year {
			get { return Get<int>("year"); }
			set { Set("year", value); }
		}
		/// <summary>
		///     Период
		/// </summary>
		public int Period {
			get { return Get<int>("period"); }
			set { Set("period", value); }
		}
		/// <summary>
		///     Объект
		/// </summary>
		public int Object {
			get { return Get<int>("object"); }
			set { Set("object", value); }
		}
		/// <summary>
		///		высота
		/// </summary>
		public int Height {
			get { return Get<int>("height"); }
			set { Set("height", value); }
		}
		/// <summary>
		///		Признак того, что работать в режиме дебага
		/// </summary>
		public bool IsInDebug {
			get { return Get("isInDebug", false); }
			set { Set("isInDebug", value); }
		}
	}
}