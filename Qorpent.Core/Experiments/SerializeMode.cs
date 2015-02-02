namespace Qorpent.Experiments {
    /// <summary>
    /// Режим сериализации
    /// </summary>
    public enum SerializeMode {
        /// <summary>
        /// Нет
        /// </summary>
        None,
        /// <summary>
        /// Только не пустые не нулевые
        /// </summary>
        OnlyNotNull,
        /// <summary>
        /// Сериализовать в любом случае
        /// </summary>
        Serialize
    }
}