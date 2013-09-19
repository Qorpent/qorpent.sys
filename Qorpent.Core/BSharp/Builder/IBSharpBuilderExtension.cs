namespace Qorpent.BSharp.Builder {
    /// <summary>
    /// Интерфейc расширения билдера
    /// </summary>
    public interface IBSharpBuilderExtension {
        /// <summary>
        /// Вызывается при присоединении расширения к билдеру
        /// </summary>
        /// <param name="builder"></param>
        void SetUp(IBSharpBuilder builder);
    }
}