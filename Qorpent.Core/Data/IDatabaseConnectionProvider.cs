﻿#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : IDatabaseConnectionProvider.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Data;

namespace Qorpent.Data {
	/// <summary>
	/// 	Фабрика строк подключений и самих подключений, с поддержкой настройки
	/// </summary>
	public interface IDatabaseConnectionProvider {
		/// <summary>
		/// 	Получить соединение по имени
		/// </summary>
		/// <param name="name"> Имя соединения </param>
		/// <returns> Содениение </returns>
		IDbConnection GetConnection(string name);

		/// <summary>
		/// 	Получить строку подключения по имени
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		string GetConnectionString(string name);

		/// <summary>
		/// 	Зарегистрировать новое соединение
		/// </summary>
		void Register(ConnectionDescriptor connectionDescriptor, bool persistent);

		/// <summary>
		/// 	Удалить строку из регистратора
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="persistent"> </param>
		void UnRegister(string name, bool persistent);

		/// <summary>
		/// 	Проверяет наличие подключения у поставщика
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		bool Exists(string name);

		/// <summary>
		/// 	Возвращает список всех имеющихся конфигураций подключений
		/// </summary>
		/// <returns> </returns>
		IEnumerable<ConnectionDescriptor> Enlist();
	}
}