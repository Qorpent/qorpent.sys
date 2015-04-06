using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.BrickScaleNormalizer;

namespace Qorpent.Utils.Tests.BrickScale.DataColonNormalizer {
	[TestFixture]
	public class DataColonLabelHelperOrderTests {
		/// <summary>
		///		Эземпляр хэлпера
		/// </summary>
		private DataColonLabelHelper _colonLabelHelper;
		[SetUp]
		public void SetUp() {
			_colonLabelHelper = new DataColonLabelHelper {
				new DataItem {NormalizedValue = 50},
				new DataItem {NormalizedValue = 40},
				new DataItem {NormalizedValue = 70}
			};
		}
		[Test]
		public void IsCorrectAsSuppliedOrder() {
			_colonLabelHelper.Order = ColonDataItemOrder.AsSupplied;
			var ordered = _colonLabelHelper.GetOrderedItems().ToArray();
			Assert.AreEqual(3, ordered.Length);
            Assert.AreEqual(50, ordered[0].NormalizedValue);
            Assert.AreEqual(40, ordered[1].NormalizedValue);
            Assert.AreEqual(70, ordered[2].NormalizedValue);
		}
		[Test]
		public void IsCorrectInvertedOrder() {
			_colonLabelHelper.Order = ColonDataItemOrder.Inverted;
			var ordered = _colonLabelHelper.GetOrderedItems().ToArray();
			Assert.AreEqual(3, ordered.Length);
            Assert.AreEqual(70, ordered[0].NormalizedValue);
            Assert.AreEqual(50, ordered[1].NormalizedValue);
            Assert.AreEqual(40, ordered[2].NormalizedValue);
		}
		[Test]
		public void IsCorrectRealOrder() {
			_colonLabelHelper.Order = ColonDataItemOrder.Real;
			var ordered = _colonLabelHelper.GetOrderedItems().ToArray();
			Assert.AreEqual(3, ordered.Length);
            Assert.AreEqual(40, ordered[0].NormalizedValue);
            Assert.AreEqual(50, ordered[1].NormalizedValue);
            Assert.AreEqual(70, ordered[2].NormalizedValue);
		}
	}
}