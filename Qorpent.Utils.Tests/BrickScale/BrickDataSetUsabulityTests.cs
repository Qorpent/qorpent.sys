using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.BrickScaleNormalizer;

namespace Qorpent.Utils.Tests.BrickScale {
    /// <summary>
    /// 
    /// </summary>
    public class BrickDataSetUsabulityTests : BrickDataSetTestBase {
        public void CanParseTwoYAxis() {
            var ds = BrickDataSet.Parse("10.2,5;11.1,6|22.1,8;99.2,2");
            var colons = ds.GetColons().ToArray();
            var series = ds.GetSeries().ToArray();
            var seria0 = series[0].ToArray();
            var seria1 = series[1].ToArray();
            var seria2 = series[2].ToArray();
            var seria3 = series[3].ToArray();
            Assert.AreEqual(2, colons.Length);
            Assert.AreEqual(4, series.Length);
            Assert.AreEqual(10.2, seria0[0]);
            Assert.AreEqual(5, seria0[1]);
            Assert.AreEqual(11.1, seria1[0]);
            Assert.AreEqual(6, seria1[1]);
            Assert.AreEqual(22.1, seria2[0]);
            Assert.AreEqual(8, seria2[1]);
            Assert.AreEqual(99.2, seria3[0]);
            Assert.AreEqual(2, seria3[1]);
        }
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
