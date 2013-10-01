using System.Collections.Generic;

namespace Qorpent.Charts {
    /// <summary>
    /// Оболочка нал списком элементов
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IChartElementList<T> :IChartElement{
        /// <summary>
        /// Акцессор как к списку
        /// </summary>
        IList<T> AsList { get; } 
    }
}