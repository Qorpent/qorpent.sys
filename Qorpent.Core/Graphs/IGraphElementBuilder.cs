namespace Qorpent.Graphs {
    /// <summary>
    /// ��������� ����������� �����
    /// </summary>
    public interface IGraphElementBuilder {
        /// <summary>
        /// �������� �������� ��������
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string Get(string name);
        /// <summary>
        /// ���������� �������� ������� ������ �����
        /// </summary>
        /// <returns></returns>
        object GetNative();
        /// <summary>
        /// �������������� �������� ��������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T Get<T>(string name);
        /// <summary>
        /// ��������� ������� ��������
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Has(string name);
        /// <summary>
        /// ���������� �������� ��������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IGraphElementBuilder Set(string name, object value);

        /// <summary>
        /// ���������� ��� ��������
        /// </summary>
        /// <returns></returns>
        string GetCode();

        /// <summary>
        /// ������������� ��������� ������
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IGraphElementBuilder SetData(object data);

        /// <summary>
        /// ���������� ��������� 
        /// </summary>
        /// <returns></returns>
        object GetData();
    }
}