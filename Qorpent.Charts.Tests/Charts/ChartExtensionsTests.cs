using System.Linq;
using NUnit.Framework;
using Qorpent.Charts;
using Qorpent.Utils.BrickScaleNormalizer;

namespace Qorpent.Serialization.Tests.Charts {
    /// <summary>
    ///     Набор тестов для расширений 
    /// </summary>
    public class ChartExtensionsTests {
        /// <summary>
        ///     Тест выражает правильность сборки <see cref="BrickDataSet"/> из <see cref="IChart"/>
        /// </summary>
        [Test]
        public void CanConvertToBrickDataset() {
            var chart = ChartBuilder.ParseDatasets("40,50.3,50.1;56.7,20,66.8");
            var brick = chart.ToBrickDataset();
            var columns = brick.BuildColons().ToArray();
            Assert.AreEqual(3, columns.Length);
            Assert.AreEqual(2, columns[0].Length);
            Assert.AreEqual(2, columns[1].Length);
            Assert.AreEqual(2, columns[2].Length);
            Assert.AreEqual(40, columns[0][0].Value);
            Assert.AreEqual(56.7, columns[0][1].Value);
            Assert.AreEqual(50.3, columns[1][0].Value);
            Assert.AreEqual(20, columns[1][1].Value);
            Assert.AreEqual(50.1, columns[2][0].Value);
            Assert.AreEqual(66.8, columns[2][1].Value);
        }
        /// <summary>
        ///     Тест выражает правильность сборки <see cref="BrickDataSet"/> из <see cref="IChart"/> и обратно
        /// </summary>
        [Test]
        public void CanConvertToBrickDatasetAndBack() {
            var chart = ChartBuilder.ParseDatasets("40,50.3,50.1;56.7,20,66.8");
            var brick = chart.ToBrickDataset();
            brick.Calculate();
            var columns = brick.BuildColons().ToArray();
            Assert.AreEqual(3, columns.Length);
            Assert.AreEqual(2, columns[0].Length);
            Assert.AreEqual(2, columns[1].Length);
            Assert.AreEqual(2, columns[2].Length);
            Assert.AreEqual(40, columns[0][0].Value);
            Assert.AreEqual(56.7, columns[0][1].Value);
            Assert.AreEqual(50.3, columns[1][0].Value);
            Assert.AreEqual(20, columns[1][1].Value);
            Assert.AreEqual(50.1, columns[2][0].Value);
            Assert.AreEqual(66.8, columns[2][1].Value);
            var fromBrick = brick.ToChart();
            Assert.AreEqual("40,50.3,50.1;56.7,20,66.8", string.Join(";", fromBrick.ToBrickDataset().GetSeries().Select(_ => _.ToString())));
        }
    }
}
