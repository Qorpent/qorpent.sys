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
// Original file : BaseTextWriterLogWriter.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.IO;

namespace Qorpent.Log {
	/// <summary>
	/// </summary>
	public class BaseTextWriterLogWriter : BaseLogWriter {
		/// <summary>
		/// </summary>
		public BaseTextWriterLogWriter() {}

		/// <summary>
		/// </summary>
		/// <param name="writer"> </param>
		public BaseTextWriterLogWriter(TextWriter writer) {
			this.writer = writer;
		}

		/// <summary>
		/// </summary>
		/// <param name="message"> </param>
		protected override void InternalWrite(LogMessage message) {
			if (null == writer) {
				return;
			}

			var text = GetText(message);
			writer.WriteLine(text);
			writer.Flush();
		}

		/// <summary>
		/// 	Creates new userlog targeted to given textwriter
		/// </summary>
		/// <param name="logname"> </param>
		/// <param name="writer"> </param>
		/// <param name="level"> </param>
		/// <param name="synchronized"> </param>
		/// <param name="customFormat"> </param>
		/// <returns> </returns>
		public static IUserLog CreateLog(string logname, TextWriter writer, LogLevel level = LogLevel.Trace,
		                                 bool synchronized = true, string customFormat = "") {
			return new LoggerBasedUserLog(
				new[]
					{
						new BaseLogger
							{
								Writers = new[] {new BaseTextWriterLogWriter(writer) {CustomFormat = customFormat}},
								Synchronized = synchronized,
							}
					}, null, logname) {Level = level};
		}

		/// <summary>
		/// 	Creates logger with text writer
		/// </summary>
		/// <param name="writer"> </param>
		/// <param name="regex"> </param>
		/// <param name="level"> </param>
		/// <param name="synchronized"> </param>
		/// <param name="customFormat"> </param>
		/// <returns> </returns>
		public static ILogger CreateLogger(TextWriter writer, string regex = "", LogLevel level = LogLevel.Trace,
		                                   bool synchronized = true, string customFormat = "") {
			return new BaseLogger
				{
					Mask = regex,
					Level = level,
					Writers = new[] {new BaseTextWriterLogWriter(writer) {CustomFormat = customFormat},},
					Synchronized = synchronized
				};
		}

		private readonly TextWriter writer;
	}
}