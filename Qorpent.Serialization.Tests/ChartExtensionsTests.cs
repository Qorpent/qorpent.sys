using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Charts;

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
            tt.SetParentElement(ds);

            chart.AddElement(ds).AddElement(tt);
            Assert.AreEqual(1, chart.Datasets.Children.Count);
            Assert.AreEqual(1, chart.Datasets.Children.First().Children.Count);
        }
    }
}
