using System.Linq;
using NUnit.Framework;
using Qorpent.Charts;
using Qorpent.Charts.FusionCharts;

namespace Qorpent.Serialization.Tests {
    public class FusionChartsScaleNormalizerTests {
        [Test]
        public void CanNormalizeAp330() {
            var chart = ChartBuilder.ParseDatasets("200,500,250,233,486");
            var normalizer = new FusionChartsScaleNormalizer();
            var normalized = normalizer.Normalize(chart, new ChartNormalized()).Scales.FirstOrDefault(_ => _.ScaleType == ChartAbstractScaleType.Y);
            Assert.IsNotNull(normalized);
            Assert.AreEqual(600, normalized.MaxValue);
            Assert.AreEqual(100, normalized.MinValue);
            Assert.AreEqual(4, normalized.NumDivLines);
        }
    }
}
