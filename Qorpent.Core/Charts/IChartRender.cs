using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Charts {
    /// <summary>
    ///     Интерфейс рендера графиков
    /// </summary>
    public interface IChartRender {
        /// <summary>
        ///     Инициализация чарт-рендера
        /// </summary>
        /// <param name="chartRenderConfig"></param>
        /// <returns></returns>
        IChartRender Initialize(IChartRenderConfig chartRenderConfig);
    }
}
