// Copyright 2007-2014  Qorpent Team - http://github.com/Qorpent
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
// Created : 2014-09-01

using System;

namespace Qorpent.IO.Net{
	/// <summary>
	///     Тип значения HTTP
	/// </summary>
	[Flags]
	public enum HttpEntityType{
		/// <summary>
		///     Неопределенное
		/// </summary>
		Undefined = 0,

		/// <summary>
		///     Определение метода
		/// </summary>
		Method = 1,

		/// <summary>
		///     Определение адреса
		/// </summary>
		Url = 1 << 1,

		/// <summary>
		///     Версия протокола
		/// </summary>
		Version = 1 << 2,

		/// <summary>
		///     Определение хоста
		/// </summary>
		Host = 1 << 3,

		/// <summary>
		///     Статус отклика
		/// </summary>
		State = 1 << 4,

		/// <summary>
		///     Имя статуса
		/// </summary>
		StateName = 1 << 5,

		/// <summary>
		///     Имя заголовка
		/// </summary>
		HeaderName = 1 << 6,

		/// <summary>
		///     Значение заголовка
		/// </summary>
		HeaderValue = 1 << 7,

		/// <summary>
		///     Данные чанка
		/// </summary>
		Chunk = 1 << 8,

		/// <summary>
		///     Начало контента
		/// </summary>
		ContentStart = 1 << 9,

		/// <summary>
		///     Ошибка
		/// </summary>
		Error = 1 << 29,

		/// <summary>
		///     Завершающая сущность
		/// </summary>
		Finish = 1 << 30,
	}
}