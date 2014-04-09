using NUnit.Framework;
using Qorpent.Utils.BrickScaleNormalizer;

namespace Qorpent.Utils.Tests.BrickScale.DataColonNormalizer {
	/// <summary>
	///		Базовый класс фикстуры для тестирования <see cref="DataColonLabelHelper"/>
	/// </summary>
	public class DataColonTestBase {
		/// <summary>
		///		Максимальное значение по шкале
		/// </summary>
		public const decimal ScaleMax = 100;
		/// <summary>
		///		Минимальное значение по шкале
		/// </summary>
		public const decimal ScaleMin = 0;
		/// <summary>
		///		Высота шкалы
		/// </summary>
		public const decimal Height = 100;
		/// <summary>
		///		Порядок расположения данных в шкале
		/// </summary>
		public const ColonDataItemOrder Order = ColonDataItemOrder.Real;
		/// <summary>
		///		Высота лычки
		/// </summary>
		public const decimal LabelHeight = 27;
		/// <summary>
		///		Эземпляр хэлпера
		/// </summary>
		protected DataColonLabelHelper ColonLabelHelper;
		[SetUp]
		public void SetUp() {
			ColonLabelHelper = new DataColonLabelHelper {
				Height = Height,
				LabelHeight = LabelHeight,
				Order = Order,
				ScaleMax = ScaleMax,
				ScaleMin = ScaleMin
			};
		}
	}
}