namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    public static class FusionChartExtensions {
        /// <summary>
        ///     Добавление сета к датасету
        /// </summary>
        /// <param name="dataset">Исходный датасет</param>
        /// <param name="label">Метка</param>
        /// <param name="value">Значение</param>
        /// <returns>Установленный сет</returns>
        public static IChartSet AddSet(this IChartDataset dataset, string label, decimal value) {
            var set = new ChartSet();
            set.Set(FusionChartApi.Set_Label, label);
            set.Set(FusionChartApi.Set_Value, value);
            dataset.Add(set);
            return set;
        }
    }
}
