using System.Collections.Generic;

namespace Qorpent.Charts {
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
        IEnumerable<ChartAbstractScale> Scales { get; }
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
        void AddScale(ChartAbstractScale abstractScale);
        /// <summary>
        ///     Добавление исправленного атрибута
        /// </summary>
        /// <param name="element">Элемент, с которым соотнести атрибут</param>
        /// <param name="attribute">Имя атрибута</param>
        /// <param name="value">Значение атрибута</param>
        void AddFixedAttribute(IChartElement element, string attribute, object value);
    }
}