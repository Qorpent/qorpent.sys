using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Charts {
    /// <summary>
    ///     Представление чарт-билдера
    /// </summary>
    public interface IChartBuilder {
        /// <summary>
        ///     Возвращает собарнный чарт
        /// </summary>
        /// <returns>Настроенный экземпляр класса, реализующего <see cref="IChart"/></returns>
        IChart GetChartObject();
        /// <summary>
        ///     Выставляет атрибуты на уровне чарта
        /// </summary>
        /// <param name="attributeSoruce">Атрибуты</param>
        /// <param name="custom">Лямбда для проброса своей логики</param>
        /// <returns>Замыкание</returns>
        IChartBuilder SetupChart(IChartAttribute[] attributeSoruce = null, Action<IChart> custom = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="labels"></param>
        /// <param name="attributeSource"></param>
        /// <param name="custom"></param>
        /// <returns></returns>
        IChartBuilder AddCategory(string[] labels, IChartAttribute[] attributeSource = null, Action<IChartElement> custom = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="attributeSource"></param>
        /// <param name="custom"></param>
        /// <returns></returns>
        IChartBuilder AddDataset(int[] values, IChartAttribute[] attributeSource = null, Action<IChartElement> custom = null);
    }
}
