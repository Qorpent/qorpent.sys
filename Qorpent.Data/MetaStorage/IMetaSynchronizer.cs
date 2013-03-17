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
// PROJECT ORIGIN: Qorpent.Data/IMetaSynchronizer.cs
#endregion
using System;
using Qorpent.Model;

namespace Qorpent.Data.MetaStorage {
	/// <summary>
	/// ������������� ���������� ���� � ��
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IMetaSynchronizer<T> where T:class , IWithId, IWithCode, new() {

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