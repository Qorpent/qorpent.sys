using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Charts;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.Scaling;

namespace Qorpent.Utils.Tests.ScaleNormalizeTests {
    public partial class ComdivRemallocTests {
        [TestCase("-744,999,-831,523,663,-1128,504,-591,325,-361", -1800.0, 1200.0, 4, true, 400)]
        public void RemallocTestCases(string dataRow, double expectedMin, double expectedMax, double divline, bool checkDivlines, int height) {
            var data = dataRow.SmartSplit(false, true, new[] { ',' }).Select(Convert.ToDouble);
            var normalized = ScaleNormalizerImproved.Normalize(new ChartConfig {Height = height.ToString()}, data);
            Console.WriteLine("Expected: from {0} to {1} with {2} divlines", expectedMin, expectedMax, divline);
            Console.WriteLine("Gotten: from {0} to {1} with {2} divlines", normalized.Minimal, normalized.Maximal, normalized.Divline);
            Assert.AreEqual(expectedMin, normalized.Minimal);
            Assert.AreEqual(expectedMax, normalized.Maximal);
            if (checkDivlines) {
                Assert.AreEqual(divline, normalized.Divline);
            }
        }
    }
}
