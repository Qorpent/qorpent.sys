using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Charts;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.Scaling;

namespace Qorpent.Utils.Tests.ScaleNormalizeTests {
    public partial class WorkingScalesOnGMKPres {
        [Explicit]
        [TestCase("3050,2600", 0, 3200, 7, true, 600, 0, true)]
        [TestCase("1679,1962,1427,1532", 0, 2000, 3, true, 300, 0, true)]
        [TestCase("1679,1962,1427,1532", 0, 2000, 9, true, 600, 0, true)]
        [TestCase("1389,1971,1337,1773", 0, 2000, 3, true, 300, 0, true)]
        [TestCase("1389,1971,1337,1773", 0, 2000, 9, true, 600, 0, true)]
        [TestCase("893,424,306,606,424,-537,-457,-261,-349,-214", -600, 1200, 5, true, 300, 0, true)]
        [TestCase("995,1073,795,774,501", 0, 1200, 3, true, 300, 0, true)]
        [TestCase("2323,2093,1556,1712,1110", 0, 2500, 4, true, 600, 0, true)]
        [TestCase("11694,3119,11394,3963", 0, 12000, 3, true, 300,0,false)]
        [TestCase("2709,2956,2041,592", 0, 3000, 5, true, 300,0,false)]
        [TestCase("2709,2956,2041,592", 0, 3000, 5, true, 600,0,true)]
        [TestCase("523,450,383,313,197,69", 0, 600, 5, true, 300,0,false)]
        [TestCase("523,450,383,313,197,69", 0, 600, 5, true, 600,0,false)]
        [TestCase("98.5,96.5,10.8,101.1", 0, 120, 3, true, 300,0,true)]
        [TestCase("3726,10610,8558,9264,1942", 0, 12000, 3, true, 300,0,false)]
        [TestCase("3120.5,1865.6,1399.6,1883.6,1330.9,772.6,237.3", 0, 3200, 7, true, 600,0 ,true)]
        [TestCase("16322.3,14065.2,10489.5,11295.5,7335.3,4008.1", 0, 18000, 3, true, 300,0, true)]
        [TestCase("1484,1558,1445,1434,1453", 0, 1600, 7, true, 600,0,true)]
        [TestCase("543,520,528,516,494,490,523,555,547,526", 0, 600, 5, true, 600,0, true)]
        [TestCase("0,1250000", 0, 2000000, 3, true, 300,0, true)]
        [TestCase("19.26,19.21,19.23,19.05", 0, 20, 3, true, 300,-1, true)]
		public void UchalGokTests(string dataRow, double expectedMin, double expectedMax, double divline, bool checkDivlines, int height, int minvalue, bool upperlabel ) {
			ExecuteScaleTest(dataRow, expectedMin, expectedMax, divline, checkDivlines, height, minvalue, upperlabel);
		}

	    private static void ExecuteScaleTest(string dataRow, double expectedMin, double expectedMax, double divline,
	                                         bool checkDivlines, int height,int minvalue, bool upperlabel) {
		    var data = dataRow.SmartSplit(false, true, new[] {','}).Select(_=>Convert.ToDouble(_,CultureInfo.InvariantCulture));
		    var normalized = ScaleNormalizerImproved.Normalize(new ChartConfig {Height = height.ToString(), MinValue = minvalue != -1 ? minvalue.ToString() : ""}, data);
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
