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

namespace Qorpent.IO.Net{
	/// <summary>
	///     Состояния сканера HTTP
	/// </summary>
	public enum HttpReaderState{
		/// <summary>
		///     Начальное положение каретки
		/// </summary>
		Start = 0,

		/// <summary>
		///     Преамбула HTTP
		/// </summary>
		Preamble = 1,

		/// <summary>
		///     Версия HTTP
		/// </summary>
		Version = 2,

		/// <summary>
		///     Чтение статуса
		/// </summary>
		State = 4,

		/// <summary>
		///     Имя статуса
		/// </summary>
		StateName = 8,

		/// <summary>
		///     Начало контента
		/// </summary>
		Content = 16,

		/// <summary>
		///     Позиция перед заголовком
		/// </summary>
		PreHeader = 32,

		/// <summary>
		///     Позиция перед значением хидера
		/// </summary>
		PreHeaderValue = 64,
		/// <summary>
		/// Чтение заголовков
		/// </summary>
		Headers = 128,

		/// <summary>
		///     Статус ошибка
		/// </summary>
		Error = 1 << 28,

		/// <summary>
		///     Статус - завершено
		/// </summary>
		Finish = 1 << 29,
		
	}
}