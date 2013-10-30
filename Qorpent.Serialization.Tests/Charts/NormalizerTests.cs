using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Charts;
using Qorpent.Charts.FusionCharts;

namespace Qorpent.Serialization.Tests.Charts {
    public class NormalizerTests {
        [Test]
        public void ChartValuesNormalizer() {
            var chart = new Chart();
            var l1 = new ChartSet().SetValue(12000).SetLabel("l1");
            var l2 = new ChartSet().SetValue(24000).SetLabel("l2");
            var l3 = new ChartSet().SetValue(13999).SetLabel("l3");
            var l4 = new ChartSet().SetValue(10499).SetLabel("l4");
            var dataset = new ChartDataset {l1, l2, l3, l4};

            chart.Add(dataset);
            chart.EnsureConfig();
            chart.Config.ShowValuesAs = ChartShowValuesAs.Thousands;

            var normalizer = new FusionChartsValuesNormalizer();
            var normalized = normalizer.Normalize(chart, new ChartNormalized());

            normalized.Apply(chart);

            Assert.AreEqual(12, l1.GetValue());
            Assert.AreEqual(24, l2.GetValue());
            Assert.AreEqual(14, l3.GetValue());
            Assert.AreEqual(10, l4.GetValue());
        }
    }
    [TestFixture]
    public class NormalizerFactoryTests {
        [Test]
        public void CantAddExternalNormalizerWithCodeLessThan1000() {
            var normalizer = new StubNormalizer();
            var factory = new FusionChartsNormalizeFactory();
            Assert.Throws<Exception>(() => factory.AddNormalizer(normalizer.SetCode(999)));
            Assert.DoesNotThrow(() => factory.AddNormalizer(new StubNormalizer().SetCode(1000)));
        }
    }
    [TestFixture]
    public class NormalizedTests {
        [Test]
        public void CorrectCopyChartToNormalized() {
            var chart = new Chart();
            var l1 = new ChartSet().SetValue(12000).SetLabel("l1");
            var l2 = new ChartSet().SetValue(24000).SetLabel("l2");
            var l3 = new ChartSet().SetValue(13999).SetLabel("l3");
            var l4 = new ChartSet().SetValue(10499).SetLabel("l4");
            var dataset = new ChartDataset { l1, l2, l3, l4 };
            chart.Add(dataset);
            var normalized = new ChartNormalized(chart);
            Assert.AreEqual(4, normalized.GetFixedAttributes<IChartSet, string>(FusionChartApi.Set_Label).Count());
        }
    }
    class StubNormalizer : FusionChartsAbstractNormalizer {
        public override IChartNormalized Normalize(IChart chart, IChartNormalized normalized) {
            throw new System.NotImplementedException();
        }
        public StubNormalizer SetCode(int code) {
            Code = code;
            return this;
        }
    }
}
