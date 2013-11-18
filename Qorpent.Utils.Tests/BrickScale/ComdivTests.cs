using System;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.BrickScaleNormalizer;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests.ScaleNormalizeTests {
	public partial class BrickScalerTest {
		[TestCase("8170,8070,7663,7203,7249,7019", 6700, 8450, 6, true, 400, 6700, true)]
		[TestCase("8170,8070,7663,7203,7249,7019", 6700, 8300, 7, true, 600, 6700, true)]
      

        [TestCase("3050,2600", 0, 3200, 7, true, 600, 0, true)]
        [TestCase("1679,1962,1427,1532", 0, 2500, 4, true, 300, 0, true)]
        [TestCase("1679,1962,1427,1532", 0, 2000, 3, true, 300, 0, false)]
        [TestCase("1679,1962,1427,1532", 0, 2100, 6, true, 600, 0, true)] 
        
		[TestCase("1389,1971,1337,1773", 0, 2100, 6, true, 400, 0, true)]
		[TestCase("1389,1971,1337,1773", 0, 2000, 3, true, 400, 0, false)]
        [TestCase("1389,1971,1337,1773", 0, 2500, 4, true, 300, 0, true)]

        [TestCase("1389,1971,1337,1773", 0, 2100, 6, true, 600, 0, true)]
        [TestCase("1389,1971,1337,1773", 0, 2000, 9, true, 600, 0, false)]

        [TestCase("893,424,306,606,424,-537,-457,-261,-349,-214", -750, 1000, 6, true, 400, -1, true)]
        [TestCase("893,424,306,606,424,-537,-457,-261,-349,-214", -750, 1000, 6, true, 400, -1, false)]
        [TestCase("893,424,306,606,424,-537,-457,-261,-349,-214", -750, 1000, 6, true, 600, -1, false)]

		[TestCase("6.3,9.8,10,12.4,4.6", 0, 14, 6, true, 400, 0, true)]
		[TestCase("6.3,9.8,10,12.4,4.6", 4, 14, 4, true, 300, -1, true)]
		[TestCase("6.3,9.8,10,12.4,4.6", 4, 14, 4, true, 300, -1, false)]

        [TestCase("995,1073,795,774,501", 0, 1200, 3, true, 300, 0, true)]
        [TestCase("2323,2093,1556,1712,1110", 0, 2500, 9, true, 600, 0, true)]
        
		[TestCase("11694,3119,11394,3963", 0, 12000, 5, true, 800, 0, false)]
		[TestCase("11694,3119,11394,3963", 0, 12000, 5, true, 600, 0, false)]
		[TestCase("11694,3119,11394,3963", 0, 12000, 5, true, 400, 0, false)]
		[TestCase("11694,3119,11394,3963", 0, 12000, 3, true, 300, 0, false)]
		[TestCase("11694,3119,11394,3963", 0, 12000, 0, true, 200, 0, false)]
		[TestCase("11694,3119,11394,3963", 0, 12000, 0, true, 100, 0, false)]


		[TestCase("32420", 0, 35000, 6, true, 800, 0, false)]
		[TestCase("32420,31048", 31000, 32600, 7, true, 800, -1, false)]
		[TestCase("32420,31048", 20000, 32500, 4, true, 800, 20000, false)]
		[TestCase("2641,-191",-1000,3000,3,true,300,0,true)]
        [TestCase("2709,2956,2041,592", 0, 3000, 2, true, 300, 0, false)]
        [TestCase("2709,2956,2041,592", 0, 3200, 7, true, 600, 0, true)]
        [TestCase("523,450,383,313,197,69", 0, 600, 2, true, 300, 0, true)]
        [TestCase("98.5,96.5,10.8,101.1", 0, 120, 3, true, 300, 0, true)]
        [TestCase("3726,10610,8558,9264,1942", 0, 12000, 3, true, 300, 0, false)]
        [TestCase("3120.5,1865.6,1399.6,1883.6,1330.9,772.6,237.3", 0, 3500, 6, true, 600, 0, true)]
        [TestCase("16322.3,14065.2,10489.5,11295.5,7335.3,4008.1", 0, 20000, 3, true, 300, 0, true)]
        [TestCase("1484,1558,1445,1434,1453", 0, 1750, 6, true, 600, 0, true)]
        [TestCase("1484,1558,1445,1434,1453", 0, 1600, 7, true, 600, 0, false)]
        [TestCase("543,520,528,516,494,490,523,555,547,526", 0, 600, 5, true, 600, 0, true)]

		[TestCase("-151109.00000,3195556.00000", -500000, 3500000, 7, true, 670, 0, true)]
		[TestCase("0,1187234.00000", 0, 1500000, 4, true, 325, 0, true)]


        public void UchalGokFixedTests(string dataRow, double expectedMin, double expectedMax, double divline, bool checkDivlines, int height, int minvalue, bool upperlabel) {
            ExecuteScaleTest(dataRow, expectedMin, expectedMax, divline, checkDivlines, height, minvalue, upperlabel);
        }
		[Test]
		public void SignedDeltaTest() {
			var req = new BrickRequest();
			req.SourceMinValue = 2009;
			req.SourceMaxValue = 2020;
			req.Setup("","","","","5");
			Assert.AreEqual(MiniamlScaleBehavior.FitMin,req.MinimalScaleBehavior);
			Assert.Greater(req.SourceMinValue,0);
			Assert.Greater(req.SourceMaxValue,2020);
			Assert.Less(req.SourceMinValue,2009);
		}

		[TestCase(21,24,20,30,20,30)]
		[TestCase(210,240,20,30,200,300)]
		[TestCase(2100,2400,20,30,2000,3000)]
		[TestCase(21000,24000,20,30,20000,30000)]
		[TestCase(210000,240000,20,30,200000,300000)]
		public void MinMaxDirectFitWithRangeMix(int actualmin, int actualmax, int reqmin,int reqmax, int resultmin, int resultmax)
		{
			var req = new BrickRequest();
			req.SourceMinValue = actualmin;
			req.SourceMaxValue = actualmax;
			req.Setup("", reqmin.ToString(), reqmax.ToString(), "", "");
			Assert.AreEqual(resultmin,req.SourceMinValue);
			Assert.AreEqual(resultmax,req.SourceMaxValue);
		}

	    private static void ExecuteScaleTest(string dataRow, double expectedMin, double expectedMax, double divline,
	                                         bool checkDivlines, int height,int minvalue, bool upperlabel) {
		    var data = dataRow.SmartSplit(false, true, new[] {','}).Select(_=>Convert.ToDecimal(_,CultureInfo.InvariantCulture));
		    decimal maxval = data.Max();
		    decimal minval =  data.Min() ;//: minvalue;
		    var behavior = MiniamlScaleBehavior.KeepZero;
			if (minvalue == -1) {
				behavior = MiniamlScaleBehavior.FitMin;
			}
			if (minvalue != -1 && minvalue != 0) {
				behavior = MiniamlScaleBehavior.MatchMin;
				if (minvalue < minval) {
					minval = minvalue;
				}
			}
		    var request = new BrickRequest(maxval, minval, upperlabel) {
			    Size = height,
				MinimalScaleBehavior = behavior

		    };
		    var catalog = new BrickCatalog();
		    var variant = catalog.GetBestVariant(request);
		    Console.WriteLine("Expected: from {0} to {1} with {2} divlines", expectedMin, expectedMax, divline);
		    Console.WriteLine("Gotten: from {0} to {1} with {2} divlines", variant.ResultMinValue, variant.ResultMaxValue,
		                      variant.ResultDivCount);
		    Assert.AreEqual(expectedMin,  variant.ResultMinValue);
		    Assert.AreEqual(expectedMax,  variant.ResultMaxValue);
		    if (checkDivlines) {
			    Assert.AreEqual(divline, variant.ResultDivCount);
		    }
	    }
    }
}
