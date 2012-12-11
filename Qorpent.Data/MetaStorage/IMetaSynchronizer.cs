using System;

namespace Qorpent.Data.MetaStorage {
	/// <summary>
	/// ������������� ���������� ���� � ��
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IMetaSynchronizer<T> where T:class ,new() {

		/// <summary>
		/// ������� ���������
		/// </summary>
		IMetaStorage<T> Storage { get; set; } 
		/// <summary>
		/// ��������� ��������
		/// </summary>
		void Load();

		/// <summary>
		/// ��������� ������� ����������
		/// </summary>
		/// <returns></returns>
		DateTime GetLastVersion();

		/// <summary>
		/// ��������� ��������� �� �� � ���
		/// </summary>
		void Update();

		/// <summary>
		/// ��������� ������� ��������������� ����������� � ��������� ��������������
		/// </summary>
		/// <param name="seconds"></param>
		void StartAutoUpdate(int seconds);

		/// <summary>
		/// ��������� ������� ��������������� �����������
		/// </summary>
		void StopAutoUpdate();
	}
}