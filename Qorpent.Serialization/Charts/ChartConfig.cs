using Qorpent.Config;

namespace Qorpent.Charts {
    /// <summary>
    ///     Имплементация конфига чарта
    /// </summary>
    public class ChartConfig : ConfigBase, IChartConfig {
        /// <summary>
        ///     Тип чарта
        /// </summary>
        public string Type {
            get { return Get<string>(ChartDefaults.ChartTypeAttributeName); }
            set { Set(ChartDefaults.ChartTypeAttributeName, value); }
        }
        /// <summary>
        ///     Ширина чарта
        /// </summary>
        public string Width {
            get { return Get<string>(ChartDefaults.ChartWidthAttributeName); }
            set { Set(ChartDefaults.ChartWidthAttributeName, value); }
        }
        /// <summary>
        ///     Высота чарта
        /// </summary>
        public string Height {
            get { return Get<string>(ChartDefaults.ChartHeightAttributeName); }
            set { Set(ChartDefaults.ChartHeightAttributeName, value); }
        }
        /// <summary>
        ///     Режим отладки чарта
        /// </summary>
        public string Debug {
            get { return Get<string>(ChartDefaults.ChartDebugAttributeName); }
            set { Set(ChartDefaults.ChartDebugAttributeName, value); }
        }
        /// <summary>
        /// Id чарта
        /// </summary>
        public string Id {
            get { return Get<string>(ChartDefaults.ChartIdAttributeName); }
            set { Set(ChartDefaults.ChartIdAttributeName, value); }
        }
        /// <summary>
        ///     Контейнер чарта
        /// </summary>
        public string Container {
            get { return Get<string>(ChartDefaults.ChartContainerAttributeName); }
            set { Set(ChartDefaults.ChartContainerAttributeName, value); }
        }
        /// <summary>
        ///     Рендер чарта средствами svg
        /// </summary>
        public bool JavaScriptRender {
            get { return Get<bool>(ChartDefaults.ChartRenderAttributeName); }
            set { Set(ChartDefaults.ChartRenderAttributeName, value); }
        }

        /// <summary>
        /// Тип данных графика
        /// </summary>
        public string DataType {
            get { return Get<string>(ChartDefaults.ChartDataTypeAttributeName); }
            set { Set(ChartDefaults.ChartDataTypeAttributeName, value); }
        }
    }
}
