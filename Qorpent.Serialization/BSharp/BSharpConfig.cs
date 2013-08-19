using System.Collections.Generic;
using Qorpent.Config;

namespace Qorpent.BSharp {
	/// <summary>
	///     Конфигурация для компилятора BxlSharp
	/// </summary>
	public class BSharpConfig : ConfigBase, IBSharpConfig {


		/// <summary>
		///     Флаг использования интерполяций
		/// </summary>
		public const string USEINTERPOLATION = "useinterpolation";

		/// <summary>
		///     Флаг режима объединения источников
		/// </summary>
		public const string SINGLESOURCE = "singlesource";
		/// <summary>
		/// Условия компиляции
		/// </summary>
		public const string CONDITIONS = "conditions";

		/// <summary>
		///     Признак использования интерполяции при компиляции
		/// </summary>
		public bool UseInterpolation {
			get { return Get(USEINTERPOLATION, false); }
			set { Set(USEINTERPOLATION, value); }
		}

		/// <summary>
		///     Если включено все исходники рассматриваются как один большой источник
		///     с общим индексом
		/// </summary>
		public bool SingleSource {
			get { return Get(SINGLESOURCE, false); }
			set { Set(SINGLESOURCE, value); }
		}

		/// <summary>
		/// Условия компиляции 
		/// </summary>
		public IDictionary<string, string> Conditions {
			get { return Get<IDictionary<string,string>>(CONDITIONS); }
			set { Set(CONDITIONS, value); }
		}
	}
}