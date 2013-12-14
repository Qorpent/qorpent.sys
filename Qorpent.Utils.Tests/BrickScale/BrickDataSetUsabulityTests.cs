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
        [Test]
        public void GetSeriesTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.Linear, 200);
            ds.Add(1, 1, 10);
            ds.Add(1, 1, 50);
            ds.Add(1, 1, 40);
            ds.Add(2, 1, 50);
            ds.Add(2, 1, 55);
            ds.Add(2, 1, 41);
            ds.SetSeriaName(1, "first");
            ds.SetSeriaName(2, "second");
            var series = ds.GetSeries().ToArray();
            var firstSeria = series[0].ToArray();
            var secondSeria = series[1].ToArray();
            Assert.AreEqual(2, series.Length);
            Assert.AreEqual(10, firstSeria[0].Value);
            Assert.AreEqual(50, firstSeria[1].Value);
            Assert.AreEqual(40, firstSeria[2].Value);
            Assert.AreEqual(50, secondSeria[0].Value);
            Assert.AreEqual(55, secondSeria[1].Value);
            Assert.AreEqual(41, secondSeria[2].Value);
            Assert.AreEqual("first", series[0].Name);
            Assert.AreEqual("second", series[1].Name);
        }
        [Test]
        public void CanRemoveZeroSeries() {
            var ds = GetEmptyDataSet(SeriaCalcMode.Linear, 200);
            ds.Add(1, 1, 10);
            ds.Add(1, 1, 50);
            ds.Add(1, 1, 40);
            ds.Add(2, 1, 0);
            ds.Add(2, 1, 0);
            ds.Add(2, 1, 0);
            ds.SetSeriaName(1, "first");
            ds.SetSeriaName(2, "second");
            var series = ds.GetSeries().ToArray();
            var firstSeria = series[0].ToArray();
            var secondSeria = series[1].ToArray();
            Assert.AreEqual(2, series.Length);
            Assert.AreEqual(10, firstSeria[0].Value);
            Assert.AreEqual(50, firstSeria[1].Value);
            Assert.AreEqual(40, firstSeria[2].Value);
            Assert.AreEqual(0, secondSeria[0].Value);
            Assert.AreEqual(0, secondSeria[1].Value);
            Assert.AreEqual(0, secondSeria[2].Value);
            Assert.AreEqual("first", series[0].Name);
            Assert.AreEqual("second", series[1].Name);
            ds.RemoveSeriesWhereAllValuesIs(0);
            series = ds.GetSeries().ToArray();
            Assert.AreEqual(1, series.Length);
            firstSeria = series[0].ToArray();
            Assert.AreEqual(10, firstSeria[0].Value);
            Assert.AreEqual(50, firstSeria[1].Value);
            Assert.AreEqual(40, firstSeria[2].Value);
        }
    }
}
