using Qorpent.Config;

namespace Qorpent.Charts {
    /// <summary>
    ///     Описание конфига чарта
    /// </summary>
    public interface IChartConfig : IConfig {
        /// <summary>
        /// Тип чарта
        /// </summary>
        string Type { get; set; }
        /// <summary>
        /// Ширина чарта
        /// </summary>
        string Width { get; set; }
        /// <summary>
        /// Высота чарта
        /// </summary>
        string Height { get; set; }
        /// <summary>
        /// Режим отладки чарта
        /// </summary>
        string Debug { get; set; }
        /// <summary>
        /// Id чарта
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// Контейнер чарта
        /// </summary>
        string Container { get; set; }
        /// <summary>
        /// Рендер чарта средствами svg
        /// </summary>
        bool JavaScriptRender { get; set; }
        /// <summary>
        /// Тип данных графика
        /// </summary>
        string DataType { get; set; }
        /// <summary>
        ///     Требуется ли подгонка максимальных и минимальных значений осей
        /// </summary>
        bool FitAxis { get; set; }
    }
}
