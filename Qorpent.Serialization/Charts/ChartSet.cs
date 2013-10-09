using Qorpent.Charts.FusionCharts;

namespace Qorpent.Charts {
    /// <summary>
    /// Единица данных
    /// </summary>
    public class ChartSet :ChartElement<IChartDataset>, IChartSet {
        /// <summary>
        /// 
        /// </summary>
        public string ValuePosition {
            get { return Get(FusionChartApi.Set_ValuePosition, "AUTO"); }
            set { Set(FusionChartApi.Set_ValuePosition, value);}
        }
    }
}