namespace Qorpent.Charts {
    /// <summary>
    ///     Единица данных
    /// </summary>
    public class ChartLine : ChartElement<IChartTrendLines>, IChartTrendLine {
        /// <summary>
        ///     Значение линии тренда
        /// </summary>
        public double StartValue {
            get { return Get<double>(ChartDefaults.ChartLineStartValue); }
            set { Set(ChartDefaults.ChartLineStartValue, value); }
        }
        /// <summary>
        ///     Цвет
        /// </summary>
        public string Color {
            get { return Get<string>(ChartDefaults.ChartLineColor); }
            set { Set(ChartDefaults.ChartLineColor, value); }
        }
        /// <summary>
        ///     Признак того, что линия пунктирная
        /// </summary>
        public bool Dashed {
            get { return Get<bool>(ChartDefaults.ChartLineDashed); }
            set { Set(ChartDefaults.ChartLineDashed, value); }
        }
        /// <summary>
        ///     Отображаемый текст
        /// </summary>
        public string DisplayValue {
            get { return Get<string>(ChartDefaults.TrendLineDisplayValue); }
            set { Set(ChartDefaults.TrendLineDisplayValue, value); }
        }
    }
}