#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Data/IMetaStorage.cs
#endregion
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