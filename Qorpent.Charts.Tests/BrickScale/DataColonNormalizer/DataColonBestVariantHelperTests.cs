using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.BrickScaleNormalizer;

namespace Qorpent.Utils.Tests.BrickScale.DataColonNormalizer {
	[TestFixture]
	public class DataColonBestVariantHelperTests : DataColonTestBase {
		[TestCase(new[] {52, 48}, new[] {LabelPosition.Below, LabelPosition.Above})]
		public void IsCorrectDecision(int[] input, LabelPosition[] expected) {
			foreach (var value in input) {
				var dataItem = new DataItem {Value = value};
				dataItem.NormalizedValue = BrickDataSetHelper.GetNormalizedValue(ScaleMin, ScaleMax, Height, dataItem.Value);
				ColonLabelHelper.Add(dataItem);
			}
			ColonLabelHelper.EnsureBestLabels();
			var orderedPositions = ColonLabelHelper.GetOrderedItems().ToArray();
			Assert.AreEqual(orderedPositions.Length, expected.Length);
			for (var i = 0; i < expected.Length; i++) {
				Assert.AreEqual(expected[i], orderedPositions[i].LabelPosition);
			}
		}
	}
}