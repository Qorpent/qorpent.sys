using System;
using System.Collections.Generic;
using Qorpent.Model;

namespace Qorpent.Data.MetaStorage {
	/// <summary>
	/// Abstract meta storage interface
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IMetaStorage<T> where T : class, IWithId, IWithCode, new() {
		/// <summary>
		/// ��� ID - ��������
		/// </summary>
		IDictionary<int, T> IdCache { get; }

		/// <summary>
		/// ��� CODE - ��������
		/// </summary>
		IDictionary<string, T> CodeCache { get; }

		/// <summary>
		/// ��� ������ - ����� ���������
		/// </summary>
		IDictionary<string, T[]> QueryCache { get; }

		/// <summary>
		/// ����� ��c������ �������������
		/// </summary>
		DateTime LastSyncTime { get; set; }

		/// <summary>
		/// True - ���� ������ ������������ �� ������
		/// </summary>
		bool IsInWriteState { get; }

		/// <summary>
		/// True - ���� �� ������� ������ ����������� �������� ������
		/// </summary>
		bool IsInReadState { get; }

		/// <summary>
		/// ������� ���������������� �����
		/// </summary>
		IDictionary<string, string> Options { get; }

		/// <summary>
		/// �������� ������ ������������� �� ������
		/// </summary>
		/// <returns></returns>
		IDisposable EnterWrite();

		/// <summary>
		/// ��������� ������ ������������� �� ������ (������ ���� � try/finally) - �����������
		/// </summary>
		/// <summary>
		/// �������� ������ ������������� �� ������ (������ ���� � try/finally) - ������������
		/// </summary>
		/// <returns></returns>
		IDisposable EnterRead();

		/// <summary>
		/// ���������� ����� ��������� �������� ���� �� ��
		/// </summary>
		void AfterInitialLoad();

		/// <summary>
		/// ���������� ����� ��������� �������� ���� �� ��
		/// </summary>
		/// <param name="updatedItems"></param>
		void AfterUpdate(T[] updatedItems);

		/// <summary>
		/// ������� ���������
		/// </summary>
		void Clear();

		/// <summary>
		/// ��
		/// </summary>
		/// <returns></returns>
		IMetaSynchronizer<T> GetSynchronizer();

		/// <summary>
		/// ������� ����� ��������� ������������ ������
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		IMetaFacade<T> CreateFacade();
	}
}