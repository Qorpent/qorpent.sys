namespace Qorpent.Graphs.Dot.Builder {
    /// <summary>
    /// ���������� ����������� ��������� �����
    /// </summary>
    public abstract class DotGraphElementBuilderBase<TElement> : IGraphElementBuilder where TElement:GraphElementBase{
        /// <summary>
        /// ������� ������������� �������
        /// </summary>
        protected readonly TElement Element;
        /// <summary>
        /// 
        /// </summary>
        internal DotGraphElementBuilderBase() {
            
        } 
        /// <summary>
        /// ������� NodeBuilder � �������� � �������� ����
        /// </summary>
        /// <param name="element"></param>
        internal DotGraphElementBuilderBase(TElement element)
        {
            Element = element;
        }

        /// <summary>
        /// �������� �������� ��������
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Get(string name) {
            return Element.Get<string>(name);
        }

        /// <summary>
        /// ���������� �������� ������� ������ �����
        /// </summary>
        /// <returns></returns>
        public object GetNative() {
            return Element;
        }

        /// <summary>
        /// ���������� ��� ��������
        /// </summary>
        /// <returns></returns>
        public string GetCode() {
            return Element.Code;
        }

        /// <summary>
        /// �������������� �������� ��������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Get<T>(string name) {
            return Element.Get<T>(name);
        }

        /// <summary>
        /// ��������� ������� ��������
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Has(string name) {
            return Element.HasAttribute(name);
        }

        /// <summary>
        /// ���������� �������� ��������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IGraphElementBuilder Set(string name, object value) {
            Element.Attributes[name] = value;
            return this;
        }
        /// <summary>
        /// ������������� ��������� ������
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IGraphElementBuilder SetData(object data) {
            Element.Data = data;
            return this;
        }
        /// <summary>
        /// ���������� ��������� 
        /// </summary>
        /// <returns></returns>
        public object GetData() {
            return Element.Data;
        }
    }
}