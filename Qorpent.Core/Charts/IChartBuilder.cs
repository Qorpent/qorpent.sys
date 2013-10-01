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
        IChart GenerateChart();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IChartBuilder AddDataset(IChartElement dataset);
    }
}
