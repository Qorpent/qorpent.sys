using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.AbstractCanvas;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsPositionNormalizer : FusionChartsAbstractNormalizer {
        /// <summary>
        /// 
        /// </summary>
        public FusionChartsPositionNormalizer() {
            Area = ChartNormalizerArea.Labels;
            AddDependency(FusionChartsNormalizerCodes.ScaleNormalizer);
        }
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public override IChartNormalized Normalize(IChart chart, IChartNormalized normalized) {
            FixLabelsPosition(chart, normalized);
            return normalized;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="normalized"></param>
        private void FixLabelsPosition(IChart chart, IChartNormalized normalized) {
            if (!chart.DatasetsExists()) {
                return;
            }
            
            var yAxis = normalized.Scales.FirstOrDefault(
                _ => _.ScaleType == ChartAbstractScaleType.Y
            );

            if (yAxis == null) {
                return;
            }

            var canvas = new Canvas(0, chart.Config.Width.ToInt(), yAxis.MinValue, yAxis.MaxValue);
            canvas.Scale(chart.Config.Width.ToInt(), chart.Config.Height.ToInt());

            GetPointsByColumn(chart, canvas).DoForEach(
                _ => ResolveColumn(_, canvas, normalized)
            );
        }
        /// <summary>
        ///     Резольвит расположение лэйблов относительно друг друга внутри колонки
        /// </summary>
        /// <param name="primitives">Перечисление примитивов, относящихся к колонке</param>
        /// <param name="canvas">Представление канвы</param>
        /// <param name="normalized">Представление нормализованного графика</param>
        private void ResolveColumn(IEnumerable<ICanvasPrimitive> primitives, Canvas canvas, IChartNormalized normalized) {
            var ps = primitives;

            foreach (var primitive in ps) {
                var min = ps.OrderBy(_ => _.Y).First();
                foreach (var nbp in canvas.Nearby(primitive, 15).Where(__ => !__.Equals(min) && ps.Contains(__))) {
                    if (!canvas.Nearby(primitive, 10).Where(__ => !__.Equals(min) && ps.Contains(__)).Contains(nbp)) {
                        normalized.AddFixedAttribute((nbp.Owner as IChartDataItem), FusionChartApi.Set_ValuePosition, "above");
                    } else {
                        normalized.AddFixedAttribute((nbp.Owner as IChartDataItem), FusionChartApi.Set_ShowLabel, false);
                    }
                }
            }
        }
        private IEnumerable<IList<ICanvasPrimitive>> GetPointsByColumn(IChart chart, Canvas canvas) {
            var src = new List<IList<IChartDataItem>>();
            var dst = new List<IList<ICanvasPrimitive>>();
            for (var k = 0; k < chart.GetLabelCount(); k++) {
                src.Add(chart.Datasets.Children.Select(
                    _ => _.Children.Skip(k).FirstOrDefault()).ToList()
                    );
            }

            SetBottomPositionForLabels(chart);

            src.DoForEach(_ => {
                var xpos = chart.Config.Width.ToInt()*src.IndexOf(_);
                var nb = new List<ICanvasPrimitive>();
                _.DoForEach(__ => {
                    var p = canvas.SetPoint(xpos, Convert.ToDouble(__.GetValue())).SetOwner(__);
                    nb.Add(p);
                });
                dst.Add(nb);
            });

            return dst;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        private void SetBottomPositionForLabels(IChart chart) {
            foreach (var el in chart.Datasets.Children.SelectMany(_ => _.Children)) {
                el.Set(FusionChartApi.Set_ValuePosition, "bottom");
                el.SetShowValue(true);
            }
        }
    }
}