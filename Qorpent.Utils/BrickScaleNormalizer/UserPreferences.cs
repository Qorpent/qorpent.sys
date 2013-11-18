namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	/// Класс описания пользовательских предпочтений при рассчетах
	/// </summary>
	public class UserPreferences {
		/// <summary>
		/// 
		/// </summary>
		public UserPreferences() {
			Height = 400;
			YTop = 20;
			SYTop = 20;
			YMin = "0";
			SYMin = "0";
			YMax = "auto";
			SYMax = "auto";

		}
		/// <summary>
		/// Режим приведения серий
		/// </summary>
		public SeriaCalcMode SeriaCalcMode { get; set; }
		/// <summary>
		/// Размер целевой канвы в пикселах
		/// </summary>
		public int Height { get; set; }
		/// <summary>
		/// Общее определение шкалы Y
		/// </summary>
		public string Y { get; set; }
		/// <summary>
		/// Общее определение шкалы SY
		/// </summary>
		public string SY { get; set; }

		/// <summary>
		/// Рекомендованный минимум по Y
		/// </summary>
		public string YMin { get; set; }
		/// <summary>
		/// Рекомендованный максимум по Y
		/// </summary>
		public string YMax { get; set; }
		/// <summary>
		/// Шапка по Y
		/// </summary>
		public int YTop { get; set; }
		/// <summary>
		/// Норма отклонения по Y
		/// </summary>
		public int YSignDelta { get; set; }
		/// <summary>
		///	Фиксированный минимум по Y
		/// </summary>
		public decimal YFixMin { get; set; }
		/// <summary>
		///	Фиксированный максимум по Y
		/// </summary>
		public decimal YFixMax { get; set; }
		/// <summary>
		/// Рекомендованный минимум по SY
		/// </summary>
		public string SYMin { get; set; }
		/// <summary>
		/// Рекомендованный максимум по SY
		/// </summary>
		public string SYMax { get; set; }
		/// <summary>
		/// Шапка по SY
		/// </summary>
		public int SYTop { get; set; }
		/// <summary>
		/// Норма отклонения по SY
		/// </summary>
		public int SYSignDelta { get; set; }
		/// <summary>
		///	Фиксированный минимум по SY
		/// </summary>
		public decimal SYFixMin { get; set; }
		/// <summary>
		///	Фиксированный максимум по SY
		/// </summary>
		public decimal SYFixMax { get; set; }
		/// <summary>
		/// Фиксированное число дивлайнов по Y
		/// </summary>
		public int YFixDiv { get; set; }
		/// <summary>
		/// Фиксированное число дивлайнов по SY
		/// </summary>
		public int SYFixDiv { get; set; }
	}
}