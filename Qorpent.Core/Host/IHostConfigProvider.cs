namespace Qorpent.Host {
    /// <summary>
    /// Поставщик текущей конфигурации
    /// </summary>
    public interface IHostConfigProvider {
        /// <summary>
        /// Получить конфигурацию
        /// </summary>
        /// <returns></returns>
        HostConfig GetConfig();
    }
}