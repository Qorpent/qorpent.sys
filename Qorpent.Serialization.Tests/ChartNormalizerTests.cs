using System;
using NUnit.Framework;
using Qorpent.Charts;
using Qorpent.Charts.FusionCharts;

namespace Qorpent.Serialization.Tests {
    /// <summary>
    /// 
    /// </summary>
    public class ChartNormalizerTests {
        [Test]
        public void CanReduceY() {
            var chart = GenerateStubChart();
            chart.Normalize(new FusionChartsNormalizerConfig {});
        }
        private Chart GenerateStubChart() {
            var result = new Chart { Caption = "Monthly Revenue" };
            result.AddSets(new {
                Jan = 420000,
                Feb = 910000,
                Mar = 720000,
                Apr = 550000,
                May = 810000,
                Jun = 510000,
                Jul = 680000,
                Aug = 620000,
                Sep = 610000,
                Oct = 490000,
                Nov = 530000,
                Dec = 330000
            });
            return result;
        }
    }
}
