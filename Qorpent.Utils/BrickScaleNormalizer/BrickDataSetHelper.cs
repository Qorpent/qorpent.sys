namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	/// 
	/// </summary>
	public static class BrickDataSetHelper {
		/// <summary>
		///		Рассчёт нормализованного значения
		/// </summary>
		/// <param name="scaleMin"></param>
		/// <param name="value"></param>
		/// <param name="valueInPixel"></param>
		/// <returns></returns>
		public static decimal GetNormalizedValue(decimal scaleMin, decimal value, decimal valueInPixel) {
			return (value - scaleMin)/valueInPixel;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="scaleMin"></param>
		/// <param name="scaleMax"></param>
		/// <param name="height"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static decimal GetNormalizedValue(decimal scaleMin, decimal scaleMax, decimal height, decimal value) {
			return GetNormalizedValue(scaleMin, value, GetValuesInPixel(scaleMin, scaleMax, height));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="scaleMin"></param>
		/// <param name="scaleMax"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static decimal GetValuesInPixel(decimal scaleMin, decimal scaleMax, decimal height) {
			return (scaleMax - scaleMin) / height;
		}
	}
}