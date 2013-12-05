using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Utils.BrickScaleNormalizer;

namespace Qorpent.Utils.Tests.BrickScale {
    /// <summary>
    /// 
    /// </summary>
    public class BrickDataSetUsabulityTests : BrickDataSetTestBase {
        [Test]
        public void CanGetMinAndMaxValueFromDataset() {
            var ds = GetEmptyDataSet(SeriaCalcMode.SeriaLinear, 200);
            ds.Add(1, 1, 10);
            ds.Add(1, 1, 50);
            ds.Add(1, 1, 40);
            ds.Add(2, 1, 50);
            ds.Add(2, 1, 55);
            ds.Add(2, 1, 41);
            Assert.AreEqual(10, ds.GetMin());
            Assert.AreEqual(55, ds.GetMax());
        }
    }
}
