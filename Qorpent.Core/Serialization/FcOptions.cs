using Qorpent.Mvc;

namespace Qorpent.Serialization {
    /// <summary>
    /// Параметры FusionCharts графиков
    /// </summary>
    public class FcOptions {
        /// <summary>
        /// Ширина графика
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// Ширина графика
        /// </summary>
        public string Height { get; set; }

        /// <summary>
        /// Формат: xml/json
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Дополнительный объект - источник информации
        /// </summary>
        public object CustomData { get; set; }

        /// <summary>
        /// Внешний контекст вызова графа
        /// </summary>
        public IMvcContext Context { get; set; }

        /// <summary>
        /// Включить режим отладки
        /// </summary>
        public bool Debug { get; set; }
    }
}
