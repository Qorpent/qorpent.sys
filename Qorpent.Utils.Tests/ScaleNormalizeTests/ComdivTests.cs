using NUnit.Framework;
using Qorpent.Charts;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.Scaling;
using System;
using System.Linq;

namespace Qorpent.Utils.Tests.ScaleNormalizeTests {
    public partial class WorkingScalesOnGMKPres {
		[TestCase("543,520,528,516,494,490,523,555,547,526", 0, 600, 5, true, 600)]
		public void UchalGokTests(string dataRow, double expectedMin, double expectedMax, double divline, bool checkDivlines, int height) {
			ExecuteScaleTest(dataRow, expectedMin, expectedMax, divline, checkDivlines, height);
		}

	    private static void ExecuteScaleTest(string dataRow, double expectedMin, double expectedMax, double divline,
	                                         bool checkDivlines, int height) {
		    var data = dataRow.SmartSplit(false, true, new[] {','}).Select(Convert.ToDouble);
		    var normalized = ScaleNormalizerImproved.Normalize(new ChartConfig {Height = height.ToString()}, data);
		    Console.WriteLine("Expected: from {0} to {1} with {2} divlines", expectedMin, expectedMax, divline);
		    Console.WriteLine("Gotten: from {0} to {1} with {2} divlines", normalized.Minimal, normalized.Maximal,
		                      normalized.Divline);
		    Assert.AreEqual(expectedMin, normalized.Minimal);
		    Assert.AreEqual(expectedMax, normalized.Maximal);
		    if (checkDivlines) {
			    Assert.AreEqual(divline, normalized.Divline);
		    }
	    }
    }
}
