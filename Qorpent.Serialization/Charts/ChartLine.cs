namespace Qorpent.Charts {
    /// <summary>
    ///     ������� ������
    /// </summary>
    public class ChartLine : ChartElement<IChartTrendLines>, IChartTrendLine {
        /// <summary>
        ///     �������� ����� ������
        /// </summary>
        public double StartValue {
            get { return Get<double>(ChartDefaults.ChartLineStartValue); }
            set { Set(ChartDefaults.ChartLineStartValue, value); }
        }
        /// <summary>
        ///     ����
        /// </summary>
        public string Color {
            get { return Get<string>(ChartDefaults.ChartLineColor); }
            set { Set(ChartDefaults.ChartLineColor, value); }
        }
        /// <summary>
        ///     ������� ����, ��� ����� ����������
        /// </summary>
        public bool Dashed {
            get { return Get<bool>(ChartDefaults.ChartLineDashed); }
            set { Set(ChartDefaults.ChartLineDashed, value); }
        }
        /// <summary>
        ///     ������������ �����
        /// </summary>
        public string DisplayValue {
            get { return Get<string>(ChartDefaults.TrendLineDisplayValue); }
            set { Set(ChartDefaults.TrendLineDisplayValue, value); }
        }
    }
}