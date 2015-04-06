using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Charts.FusionCharts
{
    /// <summary>
    /// Описывает атрибут в API FusionChart
    /// </summary>
    public class FusionChartAttributeDescriptor
    {
        /// <summary>
        /// Имя атрибута
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Целевые графики
        /// </summary>
        public FusionChartType Charts { get; set; }
        /// <summary>
        /// Целевой элемент
        /// </summary>
        public FusionChartElementType Element { get; set; }

    }
}
