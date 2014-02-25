using System.Linq;
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
        [Test]
        public void GetSeriesTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.Linear, 200);
            ds.Add(1, 1, 10);
            ds.Add(1, 1, 50);
            ds.Add(1, 1, 40);
            ds.Add(2, 1, 50);
            ds.Add(2, 1, 55);
            ds.Add(2, 1, 41);
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
