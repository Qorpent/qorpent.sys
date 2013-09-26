using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Charts {
    /// <summary>
    /// 
    /// </summary>
    public interface IChartRenderConfig {
        /// <summary>
        ///     «Нечто» — представление графика
        /// </summary>
        IChart ChartObject { get; set; }
    }
}
