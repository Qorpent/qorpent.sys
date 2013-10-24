using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsScaleNormalizer {
        /// <summary>
        ///     внутренний экземпляр чарта
        /// </summary>
        private readonly IChart _chart;
        /// <summary>
        ///     Класс нормализации шкал графика в представлении FusionCharts
        /// </summary>
        /// <param name="chart">Экземпляр класса, реализующего <see cref="IChart"/></param>
        public FusionChartsScaleNormalizer(IChart chart) {
            _chart = chart;
        }
        /// <summary>
        ///     Нормализация указанной шкалы
        /// </summary>
        /// <param name="scaleType">Указание шкалы, с которой нужно работать</param>
        /// <returns>Абстрактное представление нормализованной шкалы</returns>
        public FusionChartsAbstractScale Normalize(FusionChartsScaleType scaleType) {
            if (!IsSupported(scaleType)) {
                throw new NotSupportedException();
            }

            if (scaleType == FusionChartsScaleType.Y) {
                return NormalizeYAxis();
            }

            throw new NotSupportedException();
        }
        /// <summary>
        ///     Применяет нормализованную абстракную шкалу к представлению чарта
        /// </summary>
        /// <param name="abstractScale">Абстрактное представление нормализованной шкалы</param>
        /// <returns>Замыкание на нормализованный чарт</returns>
        public IChart Apply(FusionChartsAbstractScale abstractScale) {
            if (!IsSupported(abstractScale)) {
                throw new NotSupportedException();
            }

            if (abstractScale.ScaleType == FusionChartsScaleType.Y) {
                _chart
                    .SetNumDivLines(abstractScale.NumDivLines)
                    .SetYAxisMinValue(abstractScale.MinValue)
                    .SetYAxisMaxValue(abstractScale.MaxValue);
            }

            return _chart;
        }
        /// <summary>
        ///     Производит прокат операции нормализации Y оси чарта
        /// </summary>
        /// <returns>Представление абстрактной нормализованной шкалы</returns>
        private FusionChartsAbstractScale NormalizeYAxis() {
            var abstractScale = new FusionChartsAbstractScale {
                ScaleType = FusionChartsScaleType.Y,
                NumDivLines = 0,
                MaxValue = 0.0,
                MinValue = 0.0
            };

            if (!_chart.Datasets.Children.Any()) {
                return abstractScale;
            }

            var min = _chart.GetYMinValueWholeChart();
            var max = _chart.GetYMaxValueWholeChart();

            min = min.RoundDown(min.GetNumberOfDigits() - 1);
            max = max.RoundUp(max.GetNumberOfDigits() - 1);
            
            abstractScale.MinValue = min;
            abstractScale.MaxValue = max;
            throw new NotImplementedException();
           
        }
        /// <summary>
        ///     Указывает на то, что данный тип абстрактной шкалы поддерживается нормалайзером
        /// </summary>
        /// <param name="scaleType">Тип абстрактной шкалы</param>
        /// <returns>Признак поддержки</returns>
        private bool IsSupported(FusionChartsScaleType scaleType) {
            if (scaleType == FusionChartsScaleType.Y) {
                return true;
            }

            return false;
        }
        /// <summary>
        ///     Указывает на то, что данное представление абстрактной шкалы поддерживается нормалайзером
        /// </summary>
        /// <param name="abstractScale">Представление абстрактной шкалы поддерживается нормалайзером</param>
        /// <returns>Признак поддержки</returns>
        private bool IsSupported(FusionChartsAbstractScale abstractScale) {
            return IsSupported(abstractScale.ScaleType);
        }
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
    /// 
    /// </summary>
    public enum FusionChartsScaleType {
        /// <summary>
        /// 
        /// </summary>
        Y = 0,
        /// <summary>
        /// 
        /// </summary>
        X = 1,
        /// <summary>
        /// 
        /// </summary>
        YSecond = 3
    }
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsNormalizedChart {
        private IList<FusionChartsAbstractScale> _scales = new List<FusionChartsAbstractScale>();
        /// <summary>
        ///     Перечисление шкал графика
        /// </summary>
        public IEnumerable<FusionChartsAbstractScale> Scales {
            get { return _scales; }
        }
        /// <summary>
        ///     Добавление шкалы 
        /// </summary>
        /// <param name="abstractScale">Абстрактное представление шкалы</param>
        public void AddScale(FusionChartsAbstractScale abstractScale) {
            _scales.Add(abstractScale);
        }
    }
}
