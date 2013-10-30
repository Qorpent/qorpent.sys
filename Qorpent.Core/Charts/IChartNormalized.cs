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
        ///     Возвращает чарт, к которому привязано представление
        /// </summary>
        /// <returns>Чарт, к которому привязан чарт</returns>
        IChart GetSnappedChart();
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
        /// <summary>
        ///     Возвращает перечисление всех атрибутов по родителю
        /// </summary>
        /// <param name="parent">Родительский элемент-владалец</param>
        /// <returns>Словарь исправленных атрибутов</returns>
        IDictionary<string, object> GetFixedAttributes(IChartElement parent);
        /// <summary>
        ///     Возвращает перечисление всех исправленных атрибутов по имени
        /// </summary>
        /// <typeparam name="T">Типизация</typeparam>
        /// <param name="attribute">Имя атрибута</param>
        /// <returns>Перечисление исправленных атрибутов</returns>
        IEnumerable<T> GetFixedAttributes<T>(string attribute);
        /// <summary>
        ///     Возвращает перечисление всех исправленных атрибутов по имени и типу родителя
        /// </summary>
        /// <typeparam name="TP">Тип родителя</typeparam>
        /// <typeparam name="T">Типизация значения</typeparam>
        /// <param name="attribute">Имя атрибута</param>
        /// <returns>Перечисление исправленных атрибутов</returns>
        IEnumerable<T> GetFixedAttributes<TP, T>(string attribute);
    }
}