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
            Assert.AreEqual(2, columns[0].Items.Length);
            Assert.AreEqual(2, columns[1].Items.Length);
            Assert.AreEqual(2, columns[2].Items.Length);
            Assert.AreEqual(40, columns[0].Items[0].Value);
            Assert.AreEqual(56.7, columns[0].Items[1].Value);
            Assert.AreEqual(50.3, columns[1].Items[0].Value);
            Assert.AreEqual(20, columns[1].Items[1].Value);
            Assert.AreEqual(50.1, columns[2].Items[0].Value);
            Assert.AreEqual(66.8, columns[2].Items[1].Value);
        }
    }
}
