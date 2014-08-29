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
// PROJECT ORIGIN: Qorpent.Core/ConsoleLogWriter.cs
#endregion
using System;

namespace Qorpent.Log {
	/// <summary>
	/// 	ћенеджер записи в консоль, используемый по умолчанию
	/// </summary>
	public class ConsoleLogWriter : BaseLogWriter {
		/// <summary>
		///     ¬нутренн€€ операци€ записи в лог
		/// </summary>
		/// <param name="message"> </param>
		protected override void InternalWrite(LogMessage message) {
			var text = GetText(message);
			try {
				switch (message.Level) {
					case LogLevel.Debug:
						Console.ForegroundColor = ConsoleColor.DarkGray;
						break;
					case LogLevel.Trace:
						Console.ForegroundColor = ConsoleColor.Gray;
						break;
					case LogLevel.Info:
						Console.ForegroundColor = ConsoleColor.White;
						break;
					case LogLevel.Warning:
						Console.ForegroundColor = ConsoleColor.Yellow;
						break;
					case LogLevel.Error:
						Console.ForegroundColor = ConsoleColor.Red;
						break;
					case LogLevel.Fatal:
						Console.ForegroundColor = ConsoleColor.Magenta;
						break;
				}
				Console.WriteLine(text);
			}
			finally {
				Console.ResetColor();
			}
		}
		/// <summary>
        ///     —оздаЄт новый экземпл€р <see cref="IUserLog"/> c экземпл€ром <see cref="ConsoleLogWriter"/>
		/// </summary>
		/// <param name="logname">„еловеко-пон€тное им€ лога</param>
		/// <param name="level">”ровень логгировани€</param>
		/// <param name="customFormat">ѕереопределЄнный формат сообщени€</param>
        /// <returns>Ёкземпл€р <see cref="IUserLog"/> c экземпл€ром <see cref="ConsoleLogWriter"/></returns>
		public static IUserLog CreateLog(string logname="main", LogLevel level = LogLevel.Trace, string customFormat = "${Time} ${Message}") {
			return new LoggerBasedUserLog(
				new[]
					{
						new BaseLogger {Writers = new ILogWriter[] {
							new ConsoleLogWriter {CustomFormat = customFormat},
						},Level = level}
					}, null, logname) {Level = level};
		}
		/// <summary>
        /// 	—оздаЄт новый экземпл€р <see cref="ILogger"/> c экземпл€ром <see cref="ConsoleLogWriter"/>
		/// </summary>
		/// <param name="regex">ћаска в виде регул€рного выражени€</param>
		/// <param name="level">”ровень логгировани€</param>
        /// <param name="customFormat">ѕереопределЄнный формат сообщени€</param>
        /// <returns>Ёкземпл€р <see cref="ILogger"/> c экземпл€ром <see cref="ConsoleLogWriter"/></returns>
		public static ILogger CreateLogger(string regex = "", LogLevel level = LogLevel.Trace, string customFormat = "") {
			return new BaseLogger
				{Mask = regex, Level = level, Writers = new ILogWriter[] {new ConsoleLogWriter {CustomFormat = customFormat}}};
		}
	}
}