using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Charts;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.Scaling;

namespace Qorpent.Utils.Tests.ScaleNormalizeTests {
    public partial class WorkingScalesOnGMKPres {
        [TestCase("-744,999,-831,523,663,-1128,504,-591,325,-361", -1800.0, 1200.0, 4, true, 400,0,true)]
        public void GayGokTests(string dataRow, double expectedMin, double expectedMax, double divline, bool checkDivlines, int height, int minvalue, bool upperlabel) {
			ExecuteScaleTest(dataRow, expectedMin, expectedMax, divline, checkDivlines, height, minvalue,upperlabel);
        }
    }
}
