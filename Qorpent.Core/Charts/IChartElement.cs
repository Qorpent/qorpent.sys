using Qorpent.Config;

namespace Qorpent.Charts {

    /// <summary>
    /// 
    /// </summary>
    public interface IChartElement:IConfig {
        /// <summary>
        ///     Устанавливает родительский элемент
        /// </summary>
        /// <param name="parent">Родительский элемент</param>
        void SetParentElement(IChartElement parent);
    }
    /// <summary>
    ///     Элемент чарта
    /// </summary>
    public interface IChartElement<T> :IChartElement  where T:IChartElement {
        /// <summary>
        ///     Родительский элемент
        /// </summary>
         T Parent { get; }
    }
}
