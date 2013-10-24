using System.Linq;
using NUnit.Framework;
using Qorpent.Charts;
using Qorpent.Charts.FusionCharts;

namespace Qorpent.Serialization.Tests {
    public class FusionChartsScaleNormalizerTests {
        [TestCase("200,500,250,233,486", 600, 100)]
        [TestCase("90,105.3,120,200,180", 300, 0)]
        public void CanNormalizeAp330(string ds, int max, int min) {
            var chart = ChartBuilder.ParseDatasets(ds);
            var normalizer = new FusionChartsScaleNormalizer();
            var normalized = normalizer.Normalize(chart, new ChartNormalized()).Scales.FirstOrDefault(_ => _.ScaleType == ChartAbstractScaleType.Y);
            Assert.IsNotNull(normalized);
            Assert.AreEqual(max, normalized.MaxValue);
            Assert.AreEqual(min, normalized.MinValue);
        }
    }
}
