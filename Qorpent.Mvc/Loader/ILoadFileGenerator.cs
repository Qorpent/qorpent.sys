namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Интерфейс генератора скриптов загрузки уровня приложения
    /// </summary>
    public interface ILoadFileGenerator {
        /// <summary>
        /// Формирует на диске готовые к загрузке JS файлы
        /// </summary>
        void Generate(string rootdir = "~/.tmp/", string template = "load.{0}.js");
    }


}