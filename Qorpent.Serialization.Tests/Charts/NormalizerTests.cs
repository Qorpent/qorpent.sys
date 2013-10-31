using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Charts;
using Qorpent.Charts.FusionCharts;
using Qorpent.Utils.Extensions;

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

        private const string case1 = "216790,238688,103771;192571,105145,38828";
        private const string case2 = "200,500,250,233,286";
        private const string case3 = "2139,2066,1870,1854,1882,1823,1870;2033,2129,1936,1853,1829,1841,2033";
        [TestCase(case1, -1, -1, 300, true, 0, 250000, 5)]
        [TestCase(case1, 0, -1, 300, true, 0, 250000, 5)]
        [TestCase(case1, 38000, -1, 300, true, 38000, 240000, 1)]
        [TestCase(case2, -1, -1, 300, true, 0, 600, 5)]
        [TestCase(case3, -1, -1, 300, true, 1800, 2200, 3)]
        public void CanCorrectWorkWithYScale(string data, int minValue, int maxValue, int height, bool dropZeroes, int expectedMin,
                                             int expectedMax, int expectedNumDivLines) {
            var chart = ChartBuilder.ParseDatasets(data);
            chart.EnsureConfig();
            chart.Config.Width = "800";
            chart.Config.Height = height.ToString();
            chart.Config.SetChartType(FusionChartType.MSLine);
            if (minValue != -1) {
                chart.Config.MinValue = minValue.ToString();
            }

            if (maxValue != -1) {
                chart.Config.MaxValue = maxValue.ToString();
            }


            var normalizer = new FusionChartsScaleNormalizer();
            var normalized = normalizer.Normalize(new Chart(), new ChartNormalized(chart));
            var y = normalized.Scales.FirstOrDefault(_ => _.ScaleType == ChartAbstractScaleType.Y);
            var teststring = string.Format("{0} -> {1} | {2}", expectedMin, expectedMax, expectedNumDivLines);
            var resultstr = string.Format("{0} -> {1} | {2}", y.MinValue.ToInt(), y.MaxValue.ToInt(), y.NumDivLines.ToInt());
            Assert.AreEqual(teststring, resultstr);

                /*TODO
                 * var scaler = new ScaleAdapter( new DataSetReader().Parse(data) , minValue,maxValue, height, dropZeroes);
                 * var scale = scaler.GetScale();
                 * Assert.AreEqual(expectedMax, scale.Max);
                 * Assert.AreEqual(expectedMin, scale.Min);
                 * Assert.AreEqual(expectedNumDivLines, scale.DivLines);

                 *
                 * 
                 * 
            */

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
