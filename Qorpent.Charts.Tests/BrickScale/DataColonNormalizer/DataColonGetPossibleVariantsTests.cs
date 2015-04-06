using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.BrickScaleNormalizer;

namespace Qorpent.Utils.Tests.BrickScale.DataColonNormalizer {
	[TestFixture(Description = "Фикстура для тестирования корректности получениея всех возможных вариантов лычек")]
	public class DataColonGetPossibleVariantsTests {
		/// <summary>
		///		Эземпляр хэлпера
		/// </summary>
		private DataColonLabelHelper _colonLabelHelper;
		/// <summary>
		///		Установка теста
		/// </summary>
		[SetUp]
		public void SetUp() {
			_colonLabelHelper = new DataColonLabelHelper();
		}
		[TestCase(new[] {10, 20, 30}, 27, 0)]
		[TestCase(new[] {10, 20, -1}, 27, 1)]
		[TestCase(new[] {10, -1}, 9, 1)]
		public void IsCorrectRotation(int[] values, int expectedCount, int hiddenInEachItemExpectedCount) {
			foreach (var value in values) {
				var dataItem = new DataItem();
				if (value < 0) {
					dataItem.Hide = true;
				}
				_colonLabelHelper.Add(dataItem);
			}
			var rotation = _colonLabelHelper.GetPossibleVariants().ToArray();
			Assert.AreEqual(expectedCount, rotation.Length);
			Assert.IsTrue(rotation.All(_ => _.Count(__ => __ == LabelPosition.Hidden) == hiddenInEachItemExpectedCount));
		}
	}
}