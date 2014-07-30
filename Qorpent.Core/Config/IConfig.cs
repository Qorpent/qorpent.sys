using System.Collections.Generic;

namespace Qorpent.Config {
	/// <summary>
	///     ��������� ������������ �������
	/// </summary>
	public interface IConfig : IDictionary<string, object> {
		/// <summary>
		///     ������������� ������������ ������
		/// </summary>
		/// <param name="config"></param>
		void SetParent(IConfig config);
		/// <summary>
		///     ���������� �������� ���������
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		void Set(string name, object value);
		/// <summary>
		///     �������� ����������� �������������� �����
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		T Get<T>(string name, T def = default(T));
		/// <summary>
		///     ��������� ��������
		/// </summary>
		/// <returns></returns>
		IConfig GetParent();
		/// <summary>
		///     ���������� ����� ���� �����
		/// </summary>
		/// <param name="withParent"></param>
		/// <returns></returns>
		IEnumerable<string> GetNames(bool withParent = false);
		/// <summary>
		///     ������������ ������� � �������� �������
		/// </summary>
		/// <param name="rendertype"></param>
		/// <returns></returns>
		string ToString(ConfigRenderType rendertype);

		/// <summary>
		/// ���������
		/// </summary>
		void Freeze();
		/// <summary>
		/// 
		/// </summary>
		void Stornate();

		/// <summary>
		/// ����� ������������� �������
		/// </summary>
		void RemoveParent();
	}
}