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
				new DataItem {Value = 50},
				new DataItem {Value = 40},
				new DataItem {Value = 70}
			};
		}
		[Test]
		public void IsCorrectAsSuppliedOrder() {
			_colonLabelHelper.Order = ColonDataItemOrder.AsSupplied;
			var ordered = _colonLabelHelper.GetOrderedItems().ToArray();
			Assert.AreEqual(3, ordered.Length);
			Assert.AreEqual(50, ordered[0].Value);
			Assert.AreEqual(40, ordered[1].Value);
			Assert.AreEqual(70, ordered[2].Value);
		}
		[Test]
		public void IsCorrectInvertedOrder() {
			_colonLabelHelper.Order = ColonDataItemOrder.Inverted;
			var ordered = _colonLabelHelper.GetOrderedItems().ToArray();
			Assert.AreEqual(3, ordered.Length);
			Assert.AreEqual(50, ordered[0].Value);
			Assert.AreEqual(40, ordered[1].Value);
			Assert.AreEqual(70, ordered[2].Value);
		}
		[Test]
		public void IsCorrectRealOrder() {
			_colonLabelHelper.Order = ColonDataItemOrder.Real;
			var ordered = _colonLabelHelper.GetOrderedItems().ToArray();
			Assert.AreEqual(3, ordered.Length);
			Assert.AreEqual(50, ordered[0].Value);
			Assert.AreEqual(40, ordered[1].Value);
			Assert.AreEqual(70, ordered[2].Value);
		}
	}
}