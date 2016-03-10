using System.Collections.Generic;
using Qorpent.Log;

namespace Qorpent.BSharp{
	/// <summary>
	///     Конфигурация для компилятора BxlSharp
	/// </summary>
	public class BSharpConfig : Scope, IBSharpConfig{
		/// <summary>
		///     Флаг использования интерполяций
		/// </summary>
		public const string USEINTERPOLATION = "useinterpolation";

		/// <summary>
		///     Флаг режима объединения источников
		/// </summary>
		public const string SINGLESOURCE = "singlesource";

		/// <summary>
		///     Флаг обработки конструкции Requre
		/// </summary>
		public const string DOREQUIRE = "dorequire";

		/// <summary>
		/// </summary>
		public const string KEEPLEXINFO = "keeplexinfo";
        /// <summary>
        /// Пространство имен по умолчанию Q-273
        /// </summary>
        public const string DEFAULTNAMESPACE = "defaultnamespace";

		/// <summary>
		///     Перечень корневых элементов, которые должны быть проигнорированы
		/// </summary>
		public const string IGNOREELEMENTS = "ignoreelements";

		/// <summary>
		///     Условия компиляции
		/// </summary>
		public const string CONDITIONS = "conditions";

		/// <summary>
		///     Журнал
		/// </summary>
		public const string LOG = "log";

		private readonly IUserLog _log = new StubUserLog();
	    private IDictionary<string, object> _requireMap;

	    /// <summary>
		/// </summary>
		public BSharpConfig(){
			UseInterpolation = true;
			SingleSource = true;
		}

		/// <summary>
		///     Признак использования интерполяции при компиляции
		/// </summary>
		public bool UseInterpolation{
			get { return Get(USEINTERPOLATION, false); }
			set { Set(USEINTERPOLATION, value); }
		}


		/// <summary>
		///     Если включено все исходники рассматриваются как один большой источник
		///     с общим индексом
		/// </summary>
		public bool SingleSource{
			get { return Get(SINGLESOURCE, false); }
			set { Set(SINGLESOURCE, value); }
		}

		/// <summary>
		///     Журнал проекта
		/// </summary>
		public IUserLog Log{
			get { return Get(LOG, _log); }
			set { Set(LOG, value); }
		}

		/// <summary>
		///     Условия компиляции
		/// </summary>
		public IDictionary<string, string> Conditions{
			get { return Get<IDictionary<string, string>>(CONDITIONS); }
			set { Set(CONDITIONS, value); }
		}

		/// <summary>
		///     Элементы корневого уровня для игнора
		/// </summary>
		public string[] IgnoreElements{
			get { return Get<string[]>(IGNOREELEMENTS, null); }
			set { Set(IGNOREELEMENTS, value); }
		}

		/// <summary>
		///     Глобальные константы
		/// </summary>
		public IScope Global { get; set; }

		/// <summary>
		///     Признак необходимости выполнять Require
		/// </summary>
		public bool DoProcessRequires{
			get { return Get(DOREQUIRE, true); }
			set { Set(DOREQUIRE, value); }
		}

		/// <summary>
		///     Признак необходимости выполнять Require
		/// </summary>
		public bool KeepLexInfo{
			get { return Get(KEEPLEXINFO, false); }
			set { Set(KEEPLEXINFO, value); }
		}

	    /// <summary>
	    /// Базовое пространство имен
	    /// </summary>
        public string DefaultNamespace
        {
            get { return Get(DEFAULTNAMESPACE, ""); }
            set { Set(DEFAULTNAMESPACE, value); }
        }

	    public IDictionary<string, object> RequireMap {
	        get { return _requireMap ?? (_requireMap = new Dictionary<string, object>()); }
	        set { _requireMap = value; }
	    }
	}
}