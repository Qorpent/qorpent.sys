using NUnit.Framework;
using Qorpent.Utils.BrickScaleNormalizer;

namespace Qorpent.Utils.Tests.BrickScale.DataColonNormalizer {
	/// <summary>
	///		Фикстура для тестирования корректности вычисленя температуры
	/// </summary>
	[TestFixture(Description = "Фикстура для тестирования корректности вычисленя температуры")]
	public class DataColonTemperatureTests : DataColonTestBase {
		/// <summary>
		///		Тест выражает то, что температура будет бесконечна, если лычка бОльшего значения будет под лычкой меньшего
		/// </summary>
		[Test(Description = "Тест выражает то, что температура будет бесконечна, если лычка бОльшего значения будет под лычкой меньшего")]
		public void IsInfinityWhenHigerValueLower() {
			ColonLabelHelper.Add(new DataItem {Value = 51, LabelPosition = LabelPosition.Below});
			ColonLabelHelper.Add(new DataItem {Value = 50, LabelPosition = LabelPosition.Above});
			Assert.AreEqual(double.PositiveInfinity, ColonLabelHelper.GetTemperature());
		}
		[Test(Description = "Тест выражает то, что температра бесконечна, если лычка вылезает за рамки графика сверху")]
		public void IsInfinityWhenLabelPositionHigerThanScaleAbove() {
			ColonLabelHelper.Add(new DataItem {Value = 100, LabelPosition = LabelPosition.Above});
			Assert.AreEqual(double.PositiveInfinity, ColonLabelHelper.GetTemperature());
		}
		[Test(Description = "Тест выражает то, что температра бесконечна, если лычка вылезает за рамки графика снизу")]
		public void IsInfinityWhenLabelPositionHigerThanScaleBelow() {
			ColonLabelHelper.Add(new DataItem {Value = 0, LabelPosition = LabelPosition.Below});
			Assert.AreEqual(double.PositiveInfinity, ColonLabelHelper.GetTemperature());
		}
		[Test(Description = "Тест выражает то, что лычки двусмыслены, если лычка бОльшего значения будет под лычкой меньшего")]
		public void IsAmbiguousTest() {
			ColonLabelHelper.Add(new DataItem {Value = 51, LabelPosition = LabelPosition.Below});
			ColonLabelHelper.Add(new DataItem {Value = 50, LabelPosition = LabelPosition.Above});
			Assert.IsTrue(ColonLabelHelper.IsAmbiguous);
		}
		[Test(Description = "Тест выражает корректность подсчёта коллизий при классическом наезжании двух лэйблов")]
		public void IsCorrectCollisionCountWithThoItemsSingleColision() {
			ColonLabelHelper.Add(new DataItem {Value = 80, LabelPosition = LabelPosition.Below});
			ColonLabelHelper.Add(new DataItem {Value = 45, LabelPosition = LabelPosition.Above});
			Assert.IsFalse(ColonLabelHelper.IsAmbiguous);
			Assert.AreEqual(1, ColonLabelHelper.CollisionCount);
		}
		[Test]
		[ExpectedException]
		public void ThrowsWhenScaleMinLowerThanDataMin() {
			ColonLabelHelper.Add(new DataItem {Value = -100});
			ColonLabelHelper.EnsureBestLabels();
		}
		[Explicit]
		[Test(Description = "Тест выражает то, что предпочтение отдаётся лычке над якорьком, чем под")]
		public void TemperaturnLowerIfLabelAboveInsteadOfBelow() {
			var dataItem = new DataItem {Value = 50};
			ColonLabelHelper.Add(dataItem);
			dataItem.LabelPosition = LabelPosition.Above;
			var aboveTemperature = ColonLabelHelper.GetTemperature();
			dataItem.LabelPosition = LabelPosition.Below;
			var belowTemperature = ColonLabelHelper.GetTemperature();
			Assert.Greater(belowTemperature, aboveTemperature);
		}
	}
}