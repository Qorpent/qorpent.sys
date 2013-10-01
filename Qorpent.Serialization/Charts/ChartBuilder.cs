using System;

namespace Qorpent.Charts {
    /// <summary>
    /// 
    /// </summary>
    public abstract class ChartBuilder : IChartBuilder {
        /// <summary>
        ///     Внутренний экземпляр собираемого чарта
        /// </summary>
        protected IChart BuildChart;
        /// <summary>
        ///     Собираемый чарт с ленивой инициализацией
        /// </summary>
        protected virtual IChart Chart {
            get { return BuildChart ?? (BuildChart = new Chart()); }
        }
        /// <summary>
        ///     Возвращает собарнный чарт
        /// </summary>
        /// <returns>Настроенный экземпляр класса, реализующего <see cref="IChart"/></returns>
        public virtual IChart GenerateChart() {
            return Chart;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public virtual IChartBuilder AddCategory(IChartElement category) {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public virtual IChartBuilder AddDataset(IChartElement dataset) {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineset"></param>
        /// <returns></returns>
        public virtual IChartBuilder AddLineset(IChartElement lineset) {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trendline"></param>
        /// <returns></returns>
        public virtual IChartBuilder AddTrendLine(IChartElement trendline) {
            throw new NotImplementedException();
        }
    }
}
