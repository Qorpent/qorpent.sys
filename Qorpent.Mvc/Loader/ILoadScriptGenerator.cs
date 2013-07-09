namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Интерфейс генератора скриптов
    /// </summary>
    public interface ILoadScriptGenerator {
        /// <summary>
        /// Сформировать скрипт из переданного нормализованного набора пакетов
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        string Generate(LoadPackage[] set);
    }
}