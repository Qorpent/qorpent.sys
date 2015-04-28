namespace Qorpent.Graphs {
    /// <summary>
    /// Интерфейс построителя узлов
    /// </summary>
    public interface IGraphElementBuilder {
        /// <summary>
        /// Получить значение атрибута
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string Get(string name);
        /// <summary>
        /// Возвращает исходный целевой объект графа
        /// </summary>
        /// <returns></returns>
        object GetNative();
        /// <summary>
        /// Типизированное значение атрибута
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T Get<T>(string name);
        /// <summary>
        /// Проверить наличие атрибута
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Has(string name);
        /// <summary>
        /// Установить значение атрибута
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IGraphElementBuilder Set(string name, object value);

        /// <summary>
        /// Возвращает код элемента
        /// </summary>
        /// <returns></returns>
        string GetCode();

        /// <summary>
        /// Устанавливает связанные данные
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IGraphElementBuilder SetData(object data);

        /// <summary>
        /// Возвращает связанные 
        /// </summary>
        /// <returns></returns>
        object GetData();
    }
}