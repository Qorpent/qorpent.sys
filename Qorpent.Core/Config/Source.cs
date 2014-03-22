namespace Qorpent.Config {
	/// <summary>
	///		����� ������������ ���������
	/// </summary>
	public abstract class Source {
		/// <summary>
		///		����������� ��������
		/// </summary>
		private object _source;
		/// <summary>
		///		��������� ���������
		/// </summary>
		/// <param name="source">��������</param>
		public void SetSource(object source) {
			_source = source;
		}
		/// <summary>
		///		�������������� ��������� ���������
		/// </summary>
		/// <typeparam name="T">��������� ����������� ���������</typeparam>
		/// <returns>�������������� ��������</returns>
		public T GetSource<T>() {
			return (T) _source;
		}
	}
}