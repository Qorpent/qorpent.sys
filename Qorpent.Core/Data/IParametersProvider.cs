using System.Collections.Generic;

namespace Qorpent.Data {
    /// <summary>
    /// 
    /// </summary>
    public interface IParametersProvider {
        /// <summary>
        /// Признак использования спец-мапинга
        /// </summary>
        bool UseCustomMapping { get; }
        /// <summary>
        /// Получить параметры
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> GetParameters();
    }
}