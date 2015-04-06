namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	/// Описатель шкалы
	/// </summary>
	public class Scale {
		/// <summary>
		/// 
		/// </summary>
		public decimal Min { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public decimal Max { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int DivLines { get; set; }
		/// <summary>
		/// Признак обсчитанной шкалы
		/// </summary>
		public bool Prepared { get; set; }
		/// <summary>
		/// Количество единиц на один пиксель
		/// </summary>
		public decimal ValueInPixel { get; set; }
	}
}