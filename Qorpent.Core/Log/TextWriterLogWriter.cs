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
    ///     ������� ����� ��������� ������ � ���
    /// </summary>
	public class TextWriterLogWriter : BaseLogWriter {
        /// <summary>
        /// 
        /// </summary>
		public TextWriterLogWriter() {}
		/// <summary>
        ///     ������� ����� ��������� ������ � ���
		/// </summary>
        /// <param name="writer">��������� ������ <see cref="TextWriter"/></param>
		public TextWriterLogWriter(TextWriter writer) {
			_writer = writer;
		}
        /// <summary>
        ///     ���������� �������� ������ � ���
        /// </summary>
        /// <param name="message">����� ��������� � ������� <see cref="LogMessage"/></param>
		protected override void InternalWrite(LogMessage message) {
			if (null == _writer) {
				return;
			}

			var text = GetText(message);
			_writer.WriteLine(text);
			_writer.Flush();
		}

        /// <summary>
        ///     ������ ��������� ������, ������������ <see cref="IUserLog"/>, ���������� �� ��������� <see cref="TextWriter"/>
        /// </summary>
        /// <param name="writer">��������� ������ <see cref="TextWriter"/></param>
        /// <param name="logname">��������-�������� ��� ����</param>
        /// <param name="level">����������� ������� ������������</param>
        /// <param name="synchronized"><see cref="bool"/> True, ���� ����������</param>
        /// <param name="customFormat">��������������� ������� ������</param>
        /// <returns>��������� ������, ������������ <see cref="IUserLog"/></returns>
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
		/// 	������ ������ � ����������� <see cref="TextWriter"/>
		/// </summary>
        /// <param name="writer"><see cref="TextWriter"/></param>
		/// <param name="regex">���������� ��������� ��� �������</param>
		/// <param name="level">����������� ������� ������������</param>
		/// <param name="synchronized"><see cref="bool"/> True, ���� ����������</param>
		/// <param name="customFormat">��������������� ������� ������</param>
		/// <returns>��������� ������, ������������ <see cref="ILogger"/></returns>
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