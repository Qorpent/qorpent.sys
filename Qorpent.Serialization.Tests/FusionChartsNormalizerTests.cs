using System;
using NUnit.Framework;
using Qorpent.Charts;
using Qorpent.Charts.FusionCharts;


namespace Qorpent.Serialization.Tests {
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsNormalizerTests {
        [Explicit]
        [Test]
        public void CanNormalizeChartWithValuesLessThanOne() {
            var chart = new Chart {Config = new ChartConfig {Type = "MSBar2D", Height = "300"}};
            var ds1 = new ChartDataset { new ChartSet().SetValue(Convert.ToDecimal(0.2)).SetLabel("1") };
            var ds2 = new ChartDataset { new ChartSet().SetValue(Convert.ToDecimal(-0.3)).SetLabel("2") };
            var ds3 = new ChartDataset { new ChartSet().SetValue(Convert.ToDecimal(0.9)).SetLabel("3") };
            var ds4 = new ChartDataset { new ChartSet().SetValue(Convert.ToDecimal(-0.8)).SetLabel("4") };
            chart.Add(ds1);
            chart.Add(ds2);
            chart.Add(ds3);
            chart.Add(ds4);

            var normalizer = new FusionChartNormalizer().Initialize(chart.Config);
            normalizer.Normalize(chart);

            Assert.LessOrEqual(chart.GetYAxisMaxValue(), 2);
            Assert.GreaterOrEqual(chart.GetYAxisMaxValue(), 1);
            Assert.LessOrEqual(chart.GetYAxisMinValue(), -1);
            Assert.GreaterOrEqual(chart.GetYAxisMinValue(), -2);
        }
        [Explicit]
        [Test]
        public void CanNormalizeChartWithValuesLessThanOneAndOther() {
            var chart = new Chart { Config = new ChartConfig { Type = "MSBar2D", Height = "300"} };
            var ds1 = new ChartDataset { new ChartSet().SetValue(Convert.ToDecimal(-4)).SetLabel("1") };
            var ds2 = new ChartDataset { new ChartSet().SetValue(Convert.ToDecimal(0.3)).SetLabel("2") };
            var ds3 = new ChartDataset { new ChartSet().SetValue(Convert.ToDecimal(2)).SetLabel("3") };
            var ds4 = new ChartDataset { new ChartSet().SetValue(Convert.ToDecimal(-0.8)).SetLabel("4") };
            chart.Add(ds1);
            chart.Add(ds2);
            chart.Add(ds3);
            chart.Add(ds4);

            var normalizer = new FusionChartNormalizer().Initialize(chart.Config);
            normalizer.Normalize(chart);
            Assert.LessOrEqual(chart.GetYAxisMaxValue(), 10);
            Assert.GreaterOrEqual(chart.GetYAxisMaxValue(), 2);
            Assert.LessOrEqual(chart.GetYAxisMinValue(), -4);
            Assert.GreaterOrEqual(chart.GetYAxisMinValue(), -10);
        }
    }
}
