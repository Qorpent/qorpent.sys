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



		[TestCase("523,450,380,305,197,80", 0, 600, 2, true, 300,0)]

		[TestCase("98.5,96.5,10.8,101.1", 0, 120, 3, true, 300,0)]

		[TestCase("3726,10610,8558,9264,1942", 0, 12000, 3, true, 300,0)]
		[TestCase("3120.5,1865.6,1399.6,1883.6,1330.9,772.6,237.3", 0, 3500, 6, true, 600,0)]
		[TestCase("16322.3,14065.2,10489.5,11295.5,7335.3,4008.1", 0, 18000, 2, true, 300,0)]
		[TestCase("1484,1558,1445,1434,1453", 0, 1600, 3, true, 600,0)]
		[TestCase("543,520,528,516,494,490,523,555,547,526", 0, 600, 5, true, 600,0)]
		public void UchalGokTests(string dataRow, double expectedMin, double expectedMax, double divline, bool checkDivlines, int height, int minvalue) {
			ExecuteScaleTest(dataRow, expectedMin, expectedMax, divline, checkDivlines, height, minvalue);
		}

	    private static void ExecuteScaleTest(string dataRow, double expectedMin, double expectedMax, double divline,
	                                         bool checkDivlines, int height,int minvalue) {
		    var data = dataRow.SmartSplit(false, true, new[] {','}).Select(_=>Convert.ToDouble(_,CultureInfo.InvariantCulture));
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
