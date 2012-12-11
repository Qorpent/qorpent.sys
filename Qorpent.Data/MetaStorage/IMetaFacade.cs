using System;
using System.Collections.Generic;
using Qorpent.Model;

namespace Qorpent.Data.MetaStorage {
	/// <summary>
	/// ����� ����� ����-�����������
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IMetaFacade<T> where T:class,IWithId,IWithCode,new() {
		/// <summary>
		/// ���������� ������ �� ���������� ��������
		/// </summary>
		/// <param name="criteria">������� ������ - ID,Code,Query,Func</param>
		/// <returns></returns>
		T Get(object criteria);


		/// <summary>
		/// ���������� ������ �� ���������� ��������
		/// </summary>
		/// <param name="criteria">������� ������ - ID,Code,Query,Func</param>
		/// <param name="persistentCode">��� ��� ���������� ������� � ����������� ����</param>
		/// <returns> </returns>
		IEnumerable<T> Select(Func<T,bool> criteria, string persistentCode = null);

		/// <summary>
		/// ���������� ������ �� ���������� ��������
		/// </summary>
		/// <param name="criteria">������� ������ - ID,Code,Query,Func</param>
		/// <param name="persistentCode">��� ��� ���������� ������� � ����������� ����</param>
		/// <returns> </returns>
		IEnumerable<T> Select(object criteria, string persistentCode = null);

		/// <summary>
		/// ������ �� ��������� ���������
		/// </summary>
		IMetaStorage<T> Storage { get; set; }
	}
}