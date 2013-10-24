using Qorpent.Charts;
using Qorpent.Mvc.Binding;

namespace Qorpent.Mvc.Actions {
    /// <summary>
    ///     Собирает примитивный график по переданным данным
    /// </summary>
    [Action("_sys.chart", Role = "DEFAULT", Help = "Собирает примитивный график по переданным данным")]
    public class ChartAction : ActionBase {
        /// <summary>
        ///     Данные для чарта
        /// </summary>
        [Bind(Required = true, Help = "Представление датасетов вида 40,50.3,50.1;56.7,20,66.8")]
        public string ChartData { get; set; }
        /// <summary>
        /// Основная фаза - тело действия
        /// </summary>
        /// <returns> </returns>
        protected override object MainProcess() {
            return ChartBuilder.ParseDatasets(ChartData);
        }
    }
}
