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
// PROJECT ORIGIN: Qorpent.Core/ILogger.cs
#endregion
namespace Qorpent.Log {
    /// <summary>
    /// 	Базовая реализация логгера
    /// </summary>
	public interface ILogger {
		/// <summary>
		/// 	Уровень логгирования
		/// </summary>
		LogLevel Level { get; set; }
		/// <summary>
		///     Указывает, что логгер может использоваться (если <see cref="bool"/> True), инче — false
		/// </summary>
		bool Available { get; set; }
		/// <summary>
		/// 	Человеко-понятное имя логгера
		/// </summary>
		string Name { get; set; }
        /// <summary>
        ///     Методика поведения в ситуации сбоя логгера. Если значение не установлено, то будет
        ///     использовано значение менеджера по умолчанию
        /// </summary>
		InternalLoggerErrorBehavior ErrorBehavior { get; set; }
		/// <summary>
		///     Проверяет, что данный логгер применим к переданному контексту
		/// </summary>
		/// <param name="context">Контекст</param>
		/// <returns>True, если применим</returns>
		bool IsApplyable(object context);
		/// <summary>
		///     Начинает асинхронную запись <see cref="LogMessage"/> в целевой лог
		/// </summary>
		/// <param name="message"> </param>
		void StartWrite(LogMessage message);
        /// <summary>
        ///     Синхронизирует вызов контекста к логгеру
        /// </summary>
		void Join();
	}
}