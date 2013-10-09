using Qorpent.Charts.FusionCharts;

namespace Qorpent.Charts {
    /// <summary>
    ///     Представление датасета
    /// </summary>
    public class ChartDataset : ChartElementList<IChartDatasets,IChartDataItem>, IChartDataset {
        /// <summary>
        ///     Имя серии
        /// </summary>
        public string SeriesName {
            get { return Get<string>(FusionChartApi.Dataset_SeriesName); }
            set { Set(FusionChartApi.Dataset_SeriesName, value); } 
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Dashed {
            get { return Get<bool>(FusionChartApi.Dataset_Dashed); }
            set { Set(FusionChartApi.Dataset_Dashed, value); }
        }
        /// <summary>
        ///     Цвет линии датасета
        /// </summary>
        public string Color {
            get { return Get<string>(FusionChartApi.Dataset_Color); }
            set { Set(FusionChartApi.Dataset_Color, value);}
        }
        /// <summary>
        ///     Радиус вершин
        /// </summary>
        public int AnchorRadius {
            get { return Get<int>(FusionChartApi.Chart_AnchorRadius); }
            set { Set(FusionChartApi.Chart_AnchorRadius, value); }
        }
        /// <summary>
        ///     Количество сторое вершины
        /// </summary>
        public int AnchorSides {
            get { return Get<int>(FusionChartApi.Chart_AnchorSides); }
            set { Set(FusionChartApi.Chart_AnchorSides, value); }
        }
    }
}
