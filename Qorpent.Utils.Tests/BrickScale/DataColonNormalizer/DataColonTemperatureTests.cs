using NUnit.Framework;
using Qorpent.Utils.BrickScaleNormalizer;

namespace Qorpent.Utils.Tests.BrickScale.DataColonNormalizer {
	/// <summary>
	///		�������� ��� ������������ ������������ ��������� �����������
	/// </summary>
	[TestFixture(Description = "�������� ��� ������������ ������������ ��������� �����������")]
	public class DataColonTemperatureTests : DataColonTestBase {
		/// <summary>
		///		���� �������� ��, ��� ����������� ����� ����������, ���� ����� �������� �������� ����� ��� ������ ��������
		/// </summary>
		[Test(Description = "���� �������� ��, ��� ����������� ����� ����������, ���� ����� �������� �������� ����� ��� ������ ��������")]
		public void IsInfinityWhenHigerValueLower() {
			ColonLabelHelper.Add(new DataItem {Value = 51, LabelPosition = LabelPosition.Below});
			ColonLabelHelper.Add(new DataItem {Value = 50, LabelPosition = LabelPosition.Above});
			Assert.AreEqual(double.PositiveInfinity, ColonLabelHelper.GetTemperature());
		}
		[Test(Description = "���� �������� ��, ��� ���������� ����������, ���� ����� �������� �� ����� ������� ������")]
		public void IsInfinityWhenLabelPositionHigerThanScaleAbove() {
			ColonLabelHelper.Add(new DataItem {Value = 100, LabelPosition = LabelPosition.Above});
			Assert.AreEqual(double.PositiveInfinity, ColonLabelHelper.GetTemperature());
		}
		[Test(Description = "���� �������� ��, ��� ���������� ����������, ���� ����� �������� �� ����� ������� �����")]
		public void IsInfinityWhenLabelPositionHigerThanScaleBelow() {
			ColonLabelHelper.Add(new DataItem {Value = 0, LabelPosition = LabelPosition.Below});
			Assert.AreEqual(double.PositiveInfinity, ColonLabelHelper.GetTemperature());
		}
		[Test(Description = "���� �������� ��, ��� ����� �����������, ���� ����� �������� �������� ����� ��� ������ ��������")]
		public void IsAmbiguousTest() {
			ColonLabelHelper.Add(new DataItem {Value = 51, LabelPosition = LabelPosition.Below});
			ColonLabelHelper.Add(new DataItem {Value = 50, LabelPosition = LabelPosition.Above});
			Assert.IsTrue(ColonLabelHelper.IsAmbiguous);
		}
		[Test(Description = "���� �������� ������������ �������� �������� ��� ������������ ��������� ���� �������")]
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
		[Test(Description = "���� �������� ��, ��� ������������ ������� ����� ��� ��������, ��� ���")]
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