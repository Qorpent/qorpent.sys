using System;

namespace Qorpent.Mvc.Binding {
	/// <summary>
	/// ��������� ������������ �������� �������� ��� ���������� ������������ ���������
	/// </summary>
	public interface ICustomBindConverter {
		/// <summary>
		/// ������������ ������ � ������ �������� �����������
		/// </summary>
		/// <param name="action"></param>
		/// <param name="val"></param>
		/// <param name="context"></param>
		/// <param name="directsetter"> </param>
		void SetConverted(object action, string val, IMvcContext context,Action<object,object> directsetter);
	}
}