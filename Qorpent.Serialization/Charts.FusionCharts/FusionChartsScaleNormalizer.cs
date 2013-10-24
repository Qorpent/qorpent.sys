using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.AbstractCanvas;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsScaleNormalizer : FusionChartsAbstractNormalizer {
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public override IChartNormalized Normalize(IChart chart, IChartNormalized normalized) {
            normalized.AddScale(NormalizeYAxis(chart));
            return normalized;
        }
        /// <summary>
        ///     Производит прокат операции нормализации Y оси чарта
        /// </summary>
        /// <returns>Представление абстрактной нормализованной шкалы</returns>
        private FusionChartsAbstractScale NormalizeYAxis(IChart chart) {
            var abstractScale = new FusionChartsAbstractScale {
                ScaleType = FusionChartsScaleType.Y,
                NumDivLines = 0,
                MaxValue = 0.0,
                MinValue = 0.0
            };

            if (!chart.Datasets.Children.Any()) {
                return abstractScale;
            }

            var min = chart.GetYMinValueWholeChart();
            var max = chart.GetYMaxValueWholeChart();

            var normalizedMin = NormalizeMinValue(min);
            var normalizedMax = NormalizeMaxValue(max);

            while (normalizedMin >= min) {
                normalizedMin = NormalizeMinValue(normalizedMin - Math.Pow(10, normalizedMin.GetNumberOfDigits() - 1));
            }

            while (normalizedMax <= max) {
                normalizedMax = NormalizeMaxValue(normalizedMax + Math.Pow(10, normalizedMax.GetNumberOfDigits() - 1));
            }

            abstractScale.MinValue = normalizedMin;
            abstractScale.MaxValue = normalizedMax;
            abstractScale.NumDivLines = NormalizeNumDivlines(abstractScale);

            return abstractScale;
        }
        /// <summary>
        ///     Нормализует дивлайны
        /// </summary>
        /// <param name="abstractScale">Абстрактное представление шкалы</param>
        /// <returns></returns>
        private int NormalizeNumDivlines(FusionChartsAbstractScale abstractScale) {
            var delta = abstractScale.MaxValue - abstractScale.MinValue;
            return ((delta/Math.Pow(10, delta.GetNumberOfDigits() - 1)) - 1).ToInt();
        }
        /// <summary>
        ///     Нормализует минимальное значение
        /// </summary>
        /// <param name="num">Число для округления</param>
        /// <returns></returns>
        private double NormalizeMinValue(double num) {
            return !num.IsRoundNumber(num.GetNumberOfDigits() - 1) ? num.RoundDown(num.GetNumberOfDigits() - 1) : num;
        }
        /// <summary>
        ///     Нормализует максимальное значение.
        /// </summary>
        /// <remarks>
        ///     Если число круглое — возвращает его же, иначе — округляет вверх по порядку N - 1, где N - порядок переданного числа
        ///     Примеры: 500 -> 500, 510 -> 600, 599 -> 600, 600 -> 600, 577.5 -> 600, 500.1 -> 600
        /// </remarks>
        /// <param name="num">Число для округления</param>
        /// <returns></returns>
        private double NormalizeMaxValue(double num) {
            return !num.IsRoundNumber(num.GetNumberOfDigits() - 1) ? num.RoundUp(num.GetNumberOfDigits() - 1) : num;
        }
    }
    /// <summary>
    ///     Нормалайзер якорей величин графиков
    /// </summary>
    public class FusionChartsAnchorsNormalizer : FusionChartsAbstractNormalizer {
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public override IChartNormalized Normalize(IChart chart, IChartNormalized normalized) {
            chart.Datasets.Children.Where(
                _ => (
                    (_.Get<int>(FusionChartApi.Chart_AnchorRadius) == 0)
                        ||
                    (_.Get<int>(FusionChartApi.Chart_AnchorSides) == 0)
                )
            ).DoForEach(_ => {
                if (_.Get<int>(FusionChartApi.Chart_AnchorRadius) == 0) {
                    normalized.AddFixedAttribute(_, FusionChartApi.Chart_AnchorRadius, 5);
                }

                if (_.Get<int>(FusionChartApi.Chart_AnchorSides) == 0) {
                    normalized.AddFixedAttribute(_, FusionChartApi.Chart_AnchorSides, 3);
                }
            });

            return normalized;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsNumberScalingNormalizer : FusionChartsAbstractNormalizer {
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public override IChartNormalized Normalize(IChart chart, IChartNormalized normalized) {
            if (chart.Config != null) {
                if (!chart.Config.UseDefaultScaling) {
                    chart.Set(FusionChartApi.Chart_FormatNumber, 0);
                    chart.Set(FusionChartApi.Chart_FormatNumberScale, 0);
                }
            } else {
                chart.Set(FusionChartApi.Chart_FormatNumber, 0);
                chart.Set(FusionChartApi.Chart_FormatNumberScale, 0);
            }

            return normalized;
        }
    }
    /// <summary>
    ///     Нормалайзер цветов чарта
    /// </summary>
    public class FusionChartsColorNormalizer : FusionChartsAbstractNormalizer {
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public override IChartNormalized Normalize(IChart chart, IChartNormalized normalized) {
            foreach (var dataset in chart.Datasets.Children) {
                if (string.IsNullOrWhiteSpace(dataset.GetColor())) {
                    continue;
                }

                if (Convert.ToInt32(dataset.GetColor(), 16) / 20 > 1) {
                    normalized.AddFixedAttribute(dataset, FusionChartApi.Dataset_Color, new Hex("FF0000"));
                }
            }

            return normalized;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsPositionNormalizer : FusionChartsAbstractNormalizer {
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
            if (!chart.Datasets.Children.Any()) {
                return;
            }
            var canvas = new Canvas(0, chart.Config.Width.ToInt(), chart.GetYAxisMinValue(), chart.GetYAxisMaxValue());
            var dst = GetPointsByColumn(chart, canvas);
            canvas.Scale(chart.Config.Width.ToInt(), chart.Config.Height.ToInt());

            dst.DoForEach(_ => ResolveColumn(_, normalized));



            dst.DoForEach(
                _ => _.DoForEach(
                    __ => _.SelectMany(
                        ___ => canvas.Nearby(___, 20)
                    ).Skip(1).DoForEach(
                        ____ => {
                            if (canvas.Nearby(____, 5).Any()) {
                                normalized.AddFixedAttribute((____.Owner as IChartDataItem), FusionChartApi.Set_ShowValue, false);
                            } else {
                                var nb15 = canvas.Nearby(____, 15).ToList();

                                if (!nb15.Any()) {
                                    return;
                                }
                                var highest = nb15.FirstOrDefault(
                                    _____ => ____.X == nb15.OrderByDescending(______ => ______.X).First().X
                                );

                                if (highest.X > ____.X) {
                                    var owner = highest as IChartDataItem;

                                    if (owner != null) {
                                        normalized.AddFixedAttribute((____.Owner as IChartDataItem), FusionChartApi.Chart_LabelPosition, "above");
                                    }
                                } else {
                                    normalized.AddFixedAttribute((____.Owner as IChartDataItem), FusionChartApi.Set_ShowValue, false);
                                }
                            }
                        }
                    )
                )
            );
        }
        /// <summary>
        ///     Резольвит расположение лэйблов относительно друг друга внутри колонки
        /// </summary>
        /// <param name="primitives">Перечисление примитивов, относящихся к колонке</param>
        /// <param name="normalized">Представление нормализованного графика</param>
        private void ResolveColumn(IList<ICanvasPrimitive> primitives, IChartNormalized normalized) {
            
        }
        private List<IList<ICanvasPrimitive>> GetPointsByColumn(IChart chart, Canvas canvas) {
            var src = new List<IList<IChartDataItem>>();
            var dst = new List<IList<ICanvasPrimitive>>();
            for (var k = 0; k < chart.GetLabelCount(); k++) {
                src.Add(chart.Datasets.Children.Select(
                    _ => _.Children.Skip(k).FirstOrDefault()).ToList()
                );
            }

            SetBottomPositionForLabels(chart);

            src.DoForEach(_ => {
                var xpos = ((canvas.X.EndValue - canvas.X.BeginValue) / src.Count) * src.IndexOf(_);
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
    /// <summary>
    ///     Абстрактный класс нормализации чартов
    /// </summary>
    public abstract class FusionChartsAbstractNormalizer : IChartNormalizer {
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">Абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public abstract IChartNormalized Normalize(IChart chart, IChartNormalized normalized);
        /// <summary>
        ///     Определяет признак того, что переданное представление нормализованного чарта поддерживается системой
        /// </summary>
        /// <param name="normalized">Представление нормализованного чарта</param>
        /// <returns>Признак того, что переданное представление нормализованного чарта поддерживается системой</returns>
        public virtual bool IsSupported(IChartNormalized normalized) {
            return true;
        }
        /// <summary>
        ///     Определяет признак того, что переданное представление чарта поддерживается системой
        /// </summary>
        /// <param name="chart">Представление чарта</param>
        /// <returns>Признак того, что переданное представление чарта поддерживается системой</returns>
        public virtual bool IsSupported(IChart chart) {
            return true;
        }
    }
    /// <summary>
    ///     Интерфейс нормализатора чартов
    /// </summary>
    public interface IChartNormalizer {
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        IChartNormalized Normalize(IChart chart, IChartNormalized normalized);
        /// <summary>
        ///     Определяет признак того, что переданное представление нормализованного чарта поддерживается системой
        /// </summary>
        /// <param name="normalized">Представление нормализованного чарта</param>
        /// <returns>Признак того, что переданное представление нормализованного чарта поддерживается системой</returns>
        bool IsSupported(IChartNormalized normalized);
        /// <summary>
        ///     Определяет признак того, что переданное представление чарта поддерживается системой
        /// </summary>
        /// <param name="chart">Представление чарта</param>
        /// <returns>Признак того, что переданное представление чарта поддерживается системой</returns>
        bool IsSupported(IChart chart);
    }
    /// <summary>
    ///     абстрактное представление шкалы графика
    /// </summary>
    public class FusionChartsAbstractScale {
        /// <summary>
        ///     Позиция шкалы
        /// </summary>
        public FusionChartsScaleType ScaleType { get; set; }
        /// <summary>
        ///     Минимальное значение для шкалы
        /// </summary>
        public double MinValue { get; set; }
        /// <summary>
        ///     Максимальное значение для шкалы
        /// </summary>
        public double MaxValue { get; set; }
        /// <summary>
        ///     Кол-во дивлайнов для шкалы
        /// </summary>
        public int NumDivLines { get; set; }
    }
    /// <summary>
    ///     Представление типа оси абстрактного чарта
    /// </summary>
    public enum FusionChartsScaleType {
        /// <summary>
        ///     Основаная ось Y
        /// </summary>
        Y = 0,
        /// <summary>
        ///     Основаня ось X
        /// </summary>
        X = 1
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IChartNormalized {
        /// <summary>
        ///     Представление списка исправленных атрибутов
        /// </summary>
        IDictionary<IChartElement, IDictionary<string, object>> FixedAttributes { get; }
        /// <summary>
        ///     Перечисление шкал графика
        /// </summary>
        IEnumerable<FusionChartsAbstractScale> Scales { get; }
        /// <summary>
        ///     Применение нолрмализованных параметров к переданному чурта
        /// </summary>
        /// <param name="chart">Исходный чарт</param>
        /// <returns>Замыкание на нормализованный исходный чарт</returns>
        IChart Apply(IChart chart);
        /// <summary>
        ///     Добавление шкалы 
        /// </summary>
        /// <param name="abstractScale">Абстрактное представление шкалы</param>
        void AddScale(FusionChartsAbstractScale abstractScale);
        /// <summary>
        ///     Добавление исправленного атрибута
        /// </summary>
        /// <param name="element">Элемент, с которым соотнести атрибут</param>
        /// <param name="attribute">Имя атрибута</param>
        /// <param name="value">Значение атрибута</param>
        void AddFixedAttribute(IChartElement element, string attribute, object value);
    }
    /// <summary>
    /// 
    /// </summary>
    public class ChartNormalized : IChartNormalized {
        private readonly IList<FusionChartsAbstractScale> _scales;
        private readonly IDictionary<IChartElement, IDictionary<string, object>> _fixedAttributes;
        /// <summary>
        ///     Представление списка исправленных атрибутов
        /// </summary>
        public IDictionary<IChartElement, IDictionary<string, object>> FixedAttributes {
            get { return _fixedAttributes; }
        }
        /// <summary>
        ///     Перечисление шкал графика
        /// </summary>
        public IEnumerable<FusionChartsAbstractScale> Scales {
            get { return _scales; }
        }
        /// <summary>
        ///     Представление нормализованного чарта
        /// </summary>
        public ChartNormalized() {
            _scales = new List<FusionChartsAbstractScale>();
            _fixedAttributes = new Dictionary<IChartElement, IDictionary<string, object>>();
        }
        /// <summary>
        ///     Применение нолрмализованных параметров к переданному чурта
        /// </summary>
        /// <param name="chart">Исходный чарт</param>
        /// <returns>Замыкание на нормализованный исходный чарт</returns>
        public IChart Apply(IChart chart) {
            var xAxis = Scales.FirstOrDefault(_ => _.ScaleType == FusionChartsScaleType.Y);
            if (xAxis != null) {
                chart.SetYAxisMinValue(xAxis.MinValue);
                chart.SetYAxisMaxValue(xAxis.MaxValue);
                chart.SetNumDivLines(xAxis.NumDivLines);
            }

            foreach (var element in _fixedAttributes) {
                foreach (var attribute in element.Value) {
                    element.Key.Set(attribute.Key, attribute.Value);
                }
            }

            return chart;
        }
        /// <summary>
        ///     Добавление шкалы 
        /// </summary>
        /// <param name="abstractScale">Абстрактное представление шкалы</param>
        public void AddScale(FusionChartsAbstractScale abstractScale) {
            _scales.Add(abstractScale);
        }
        /// <summary>
        ///     Добавление исправленного атрибута
        /// </summary>
        /// <param name="element">Элемент, с которым соотнести атрибут</param>
        /// <param name="attribute">Имя атрибута</param>
        /// <param name="value">Значение атрибута</param>
        public void AddFixedAttribute(IChartElement element, string attribute, object value) {
            if (!_fixedAttributes.ContainsKey(element)) {
                _fixedAttributes.Add(element, new Dictionary<string, object>());
            }

            if (_fixedAttributes[element].ContainsKey(attribute)) {
                _fixedAttributes[element][attribute] = value;
            } else {
                _fixedAttributes[element].Add(attribute, value);
            }
        }
    }
}
