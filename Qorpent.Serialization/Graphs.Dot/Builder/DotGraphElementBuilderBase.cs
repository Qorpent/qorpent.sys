namespace Qorpent.Graphs.Dot.Builder {
    /// <summary>
    /// Абстактный построитель элементов графа
    /// </summary>
    public abstract class DotGraphElementBuilderBase<TElement> : IGraphElementBuilder where TElement:GraphElementBase{
        /// <summary>
        /// Целевой настраиваемый элемент
        /// </summary>
        protected readonly TElement Element;
        /// <summary>
        /// 
        /// </summary>
        internal DotGraphElementBuilderBase() {
            
        } 
        /// <summary>
        /// Создает NodeBuilder в привязке к целевому узлу
        /// </summary>
        /// <param name="element"></param>
        internal DotGraphElementBuilderBase(TElement element)
        {
            Element = element;
        }

        /// <summary>
        /// Получить значение атрибута
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Get(string name) {
            return Element.Get<string>(name);
        }

        /// <summary>
        /// Возвращает исходный целевой объект графа
        /// </summary>
        /// <returns></returns>
        public object GetNative() {
            return Element;
        }

        /// <summary>
        /// Возвращает код элемента
        /// </summary>
        /// <returns></returns>
        public string GetCode() {
            return Element.Code;
        }

        /// <summary>
        /// Типизированное значение атрибута
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Get<T>(string name) {
            return Element.Get<T>(name);
        }

        /// <summary>
        /// Проверить наличие атрибута
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Has(string name) {
            return Element.HasAttribute(name);
        }

        /// <summary>
        /// Установить значение атрибута
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IGraphElementBuilder Set(string name, object value) {
            Element.Attributes[name] = value;
            return this;
        }
        /// <summary>
        /// Устанавливает связанные данные
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IGraphElementBuilder SetData(object data) {
            Element.Data = data;
            return this;
        }
        /// <summary>
        /// Возвращает связанные 
        /// </summary>
        /// <returns></returns>
        public object GetData() {
            return Element.Data;
        }
    }
}