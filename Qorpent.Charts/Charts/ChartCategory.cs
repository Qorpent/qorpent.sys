using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Charts {
    /// <summary>
    ///     Представление категории
    /// </summary>
    public partial class ChartCategory : ChartElement<IChartCategories>,IChartCategory {
        /// <summary>
        /// 
        /// </summary>
        public string LabelOptionName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ChartCategory() {
            LabelOptionName = "label";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        public IChartCategory SetLabelValue(string label) {
            Set(LabelOptionName, label);
            return this;
        }
    }
}
