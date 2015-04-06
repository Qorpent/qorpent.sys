using NUnit.Framework;
using Qorpent.Charts;
using System.Linq;

namespace Qorpent.Serialization.Tests {
    /// <summary>
    /// 
    /// </summary>
    public class ChartExtensionsTests {
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void CanAddElement() {
            var chart = new Chart();
            var ds = new ChartDataset();
            var tt = new ChartSet();
            var tl = new ChartTrendLine();
            var tl2 = new ChartTrendLine();
            var tls = new ChartTrendLines();
            tl2.SetParentElement(tls);
            tt.SetParentElement(ds);

            chart.Add(ds).Add(tt).Add(tl).Add(tl2);
            Assert.AreEqual(1, chart.Datasets.Children.Count);
            Assert.AreEqual(1, chart.Datasets.Children.First().Children.Count);
            Assert.AreEqual(1, chart.TrendLines.Children.Count);
        }
    }
}
