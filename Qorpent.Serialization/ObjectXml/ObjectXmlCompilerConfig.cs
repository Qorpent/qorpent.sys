using Qorpent.Config;

namespace Qorpent.ObjectXml {
	/// <summary>
	/// Конфигурация для компилятора ObjectXml
	/// </summary>
	public class ObjectXmlCompilerConfig :ConfigBase, IObjectXmlCompilerConfig {
		/// <summary>
		/// Флаг использования интерполяций
		/// </summary>
		public const string USEINTERPOLATION = "useinterpolation";
		/// <summary>
		/// Флаг режима объединения источников
		/// </summary>
		public const string SINGLESOURCE = "useinterpolation";

		public bool UseInterpolation {
			get { return Get(USEINTERPOLATION, false); }
			set { Set(USEINTERPOLATION, value); }
		}
		/// <summary>
		/// Если включено все исходники рассматриваются как один большой источник
		/// с общим индексом
		/// </summary>
		public bool SingleSource
		{
			get { return Get(SINGLESOURCE, false); }
			set { Set(SINGLESOURCE, value); }
		}
	}
}