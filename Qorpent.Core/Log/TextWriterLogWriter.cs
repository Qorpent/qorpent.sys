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
// PROJECT ORIGIN: Qorpent.Core/BaseTextWriterLogWriter.cs
#endregion

using System;
using System.IO;

namespace Qorpent.Log {
    /// <summary>
    ///     Ѕазовый класс менеджера записи в лог
    /// </summary>
	public class TextWriterLogWriter : BaseLogWriter {
        /// <summary>
        /// 
        /// </summary>
		public TextWriterLogWriter() {}
		/// <summary>
        ///     Ѕазовый класс менеджера записи в лог
		/// </summary>
        /// <param name="writer">Ёкземпл€р класса <see cref="TextWriter"/></param>
		public TextWriterLogWriter(TextWriter writer) {
			_writer = writer;
		}
        /// <summary>
        ///     ¬нутренн€€ операци€ записи в лог
        /// </summary>
        /// <param name="message">“екст сообщени€ в формате <see cref="LogMessage"/></param>
		protected override void InternalWrite(LogMessage message) {
			if (null == _writer) {
				return;
			}

			var text = GetText(message);
			_writer.WriteLine(text);
			_writer.Flush();
		}

        /// <summary>
        ///     —оздаЄт экземпл€р класса, реализующего <see cref="IUserLog"/>, нацеленный на указанный <see cref="TextWriter"/>
        /// </summary>
        /// <param name="writer">Ёкземпл€р класса <see cref="TextWriter"/></param>
        /// <param name="logname">„еловеко-пон€тное им€ лога</param>
        /// <param name="level">ћинимальный уровень логгировани€</param>
        /// <param name="synchronized"><see cref="bool"/> True, если синхронный</param>
        /// <param name="customFormat">ѕереопределение формата вывода</param>
        /// <returns>Ёкземпл€р класса, реализующего <see cref="IUserLog"/></returns>
        public static IUserLog CreateLog(TextWriter writer, string logname = "main", LogLevel level = LogLevel.Trace, bool synchronized = true, string customFormat = "") {
			return new LoggerBasedUserLog(
				new[]
					{
						new BaseLogger
							{
								Writers = new ILogWriter[] {new TextWriterLogWriter(writer) {CustomFormat = customFormat}},
								Synchronized = synchronized,
							}
					}, null, logname) {Level = level};
		}
		/// <summary>
		/// 	—оздаЄт логгер с экземпл€ром <see cref="TextWriter"/>
		/// </summary>
        /// <param name="writer"><see cref="TextWriter"/></param>
		/// <param name="regex">–егул€рное выражение дл€ логгера</param>
		/// <param name="level">ћинимальный уровень логгировани€</param>
		/// <param name="synchronized"><see cref="bool"/> True, если синхронный</param>
		/// <param name="customFormat">ѕереопределение формата вывода</param>
		/// <returns>Ёкземпл€р класса, реализующего <see cref="ILogger"/></returns>
		public static ILogger CreateLogger(TextWriter writer, string regex = "", LogLevel level = LogLevel.Trace,
		                                   bool synchronized = true, string customFormat = "") {
			return new BaseLogger
				{
					Mask = regex,
					Level = level,
					Writers = new ILogWriter[] {new TextWriterLogWriter(writer) {CustomFormat = customFormat}},
					Synchronized = synchronized
				};
		}

		private readonly TextWriter _writer;
	}
}