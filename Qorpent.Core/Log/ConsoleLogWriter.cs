#region LICENSE

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
// Original file : ConsoleLogWriter.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Log {
	/// <summary>
	/// 	Default console writer
	/// </summary>
	public class ConsoleLogWriter : BaseLogWriter {
		/// <summary>
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
		/// 	Creates new userlog with consoler writer
		/// </summary>
		/// <param name="logname"> </param>
		/// <param name="level"> </param>
		/// <param name="customFormat"> </param>
		/// <returns> </returns>
		public static IUserLog CreateLog(string logname, LogLevel level = LogLevel.Trace, string customFormat = "") {
			return new LoggerBasedUserLog(
				new[]
					{
						new BaseLogger {Writers = new ILogWriter[] {new ConsoleLogWriter {CustomFormat = customFormat}}}
					}, null, logname) {Level = level};
		}

		/// <summary>
		/// 	Creates logger with console writer
		/// </summary>
		/// <param name="regex"> </param>
		/// <param name="level"> </param>
		/// <param name="customFormat"> </param>
		/// <returns> </returns>
		public static ILogger CreateLogger(string regex = "", LogLevel level = LogLevel.Trace, string customFormat = "") {
			return new BaseLogger
				{Mask = regex, Level = level, Writers = new ILogWriter[] {new ConsoleLogWriter {CustomFormat = customFormat}}};
		}
	}
}