using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Charts.Implementation {
    /// <summary>
    /// 
    /// </summary>
    public class ChartBuilder : IChartBuilder {
        /// <summary>
        /// 
        /// </summary>
        private IChartConfig _internalChartConfig;
        /// <summary>
        /// 
        /// </summary>
        private IChartConfig InternalChartConfig {
            get { return _internalChartConfig ?? (_internalChartConfig = new ChartConfig()); }
        }
        /// <summary>
        ///     Возвращает собарнный чарт
        /// </summary>
        /// <returns>Настроенный экземпляр класса, реализующего <see cref="IChart"/></returns>
        public IChart GetChartObject() {
            return Chart.Initialize(InternalChartConfig);
        }
        /// <summary>
        ///     Выставляет атрибуты на уровне чарта
        /// </summary>
        /// <param name="attributeSoruce">Атрибуты</param>
        /// <param name="custom">Лямбда для проброса своей логики</param>
        /// <returns>Замыкание</returns>
        public IChartBuilder SetupChart(IChartAttribute[] attributeSoruce = null, Action<IChart> custom = null) {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="labels"></param>
        /// <param name="attributeSource"></param>
        /// <param name="custom"></param>
        /// <returns></returns>
        public IChartBuilder AddCategory(string[] labels, IChartAttribute[] attributeSource = null, Action<IChartElement> custom = null) {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="attributeSource"></param>
        /// <param name="custom"></param>
        /// <returns></returns>
        public IChartBuilder AddDataset(int[] values, IChartAttribute[] attributeSource = null, Action<IChartElement> custom = null) {
            throw new NotImplementedException();
        }
    }
}
