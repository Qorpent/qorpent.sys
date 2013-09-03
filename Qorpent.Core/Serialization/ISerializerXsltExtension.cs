namespace Qorpent.Serialization {
    /// <summary>
    /// Интерфейс расширения XSLT для сериализатора (Q-17)
    /// </summary>
    public interface ISerializerXsltExtension {
        /// <summary>
        /// Пространство имен для расширения
        /// </summary>
        /// <returns></returns>
        string GetNamespace();
        /// <summary>
        /// Рекомендованный префикс для расширения
        /// </summary>
        /// <returns></returns>
        string GetPrefix();
    }
}