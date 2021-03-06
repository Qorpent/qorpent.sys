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
// PROJECT ORIGIN: Qorpent.Core/BaseLogWriter.cs
#endregion
using System;
using System.Text.RegularExpressions;

namespace Qorpent.Log {
	/// <summary>
    ///     Абстрактный менеджер записи в лог с Thread-safe обёрткой к райтеру
	/// </summary>
	public abstract class BaseLogWriter : ServiceBase, ILogWriter {
        /// <summary>
        /// 	Минимальный уровень логгирования
        /// </summary>
        public LogLevel Level { get; set; }

	    public bool Active { get; set; } = true;

	    /// <summary>
		/// 	Формат выходящих сообщений
		/// </summary>
		public string CustomFormat { get; set; }
		/// <summary>
		/// 	Записывает сообщение <see cref="LogMessage"/> в лог на нижний уровень
		/// </summary>
        /// <param name="message">Представление сообщения <see cref="LogMessage"/></param>
		public void Write(LogMessage message) {
		    if (!Active) return;
			lock (Sync) {
				InternalWrite(message);
			}
		}
        /// <summary>
        ///     Внутренняя операция записи в лог
        /// </summary>
        /// <param name="message">Текст сообщения в формате <see cref="LogMessage"/></param>
		protected abstract void InternalWrite(LogMessage message);
		/// <summary>
		///     Возвращет форматированный текст из представления сообщения <see cref="LogMessage"/>. Фасад над <see cref="GetFormatted"/>
		/// </summary>
        /// <param name="message">Представление сообщения <see cref="LogMessage"/></param>
		/// <returns>Форматированная строка сообщения</returns>
		protected virtual string GetText(LogMessage message) {
			return GetFormatted(message);
		}
		/// <summary>
        ///     Возвращет форматированный текст из представления сообщения <see cref="LogMessage"/>
		/// </summary>
        /// <param name="message">Представление сообщения <see cref="LogMessage"/></param>
        /// <returns>Форматированная строка сообщения</returns>
		protected virtual string GetFormatted(LogMessage message) {
			string text;
			if (!string.IsNullOrEmpty(CustomFormat)) {
				text = Regex.Replace(CustomFormat, @"\$\{(\w+)\}", m =>
					{
						var propName = m.Groups[1].Value;
						var prop = message.GetType().GetProperty(propName);
						if (null != prop) {
							var val = prop.GetValue(message, null);
							if (null == val) {
								return "";
							}
							if (val is DateTime) {
								return ((DateTime) val).ToString("yyyy-MM-dd HH:mm:ss");
							}
							return val.ToString();
						}
						return "";
					});
			}
			else {
				text = message.ToString();
			}
			return text;
		}
	}
}