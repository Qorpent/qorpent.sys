using System;

namespace Qorpent.Experiments {
    /// <summary>
    /// Режим сериализации
    /// </summary>
    [Flags]
    public enum SerializeMode {
        /// <summary>
        /// Нет
        /// </summary>
        None =0,
        /// <summary>
        /// Только не пустые не нулевые
        /// </summary>
        OnlyNotNull=1,
        /// <summary>
        /// Сериализовать в любом случае
        /// </summary>
        Serialize = 2,
        /// <summary>
        /// Неизвестный тип
        /// </summary>
        Unknown = 4,
        LowerCase =8
    }
}