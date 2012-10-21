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
// Original file : StubUserLog.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

namespace Qorpent.Log {
	/// <summary>
	/// 	mute out logging
	/// </summary>
	public class StubUserLog : IUserLog {
// ReSharper disable InconsistentNaming
		private static readonly IUserLog _default = new StubUserLog();
// ReSharper restore InconsistentNaming

		/// <summary>
		/// 	Default pseudo logger
		/// </summary>
		public static IUserLog Default {
			get { return _default; }
		}


		/// <summary>
		/// 	����������� ������, � �������� �������� ������ ������, ����������� ������ ����� 
		/// 	��������������� � ������ ��� <see cref="LogMessage.HostObject" />
		/// </summary>
		public object HostObject { get; set; }

		/// <summary>
		/// 	Generates debug UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Debug(string message, object context = null, object host = null) {}

		/// <summary>
		/// 	Generate error UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Error(string message, object context = null, object host = null) {}

		/// <summary>
		/// 	Generate trace UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Trace(string message, object context = null, object host = null) {}

		/// <summary>
		/// 	Generate user info UserLog message
		/// </summary>
		/// <param name="item"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Info(string item, object context = null, object host = null) {}

		/// <summary>
		/// 	Generate warn UserLog message
		/// </summary>
		/// <param name="item"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Warn(string item, object context = null, object host = null) {}

		/// <summary>
		/// 	Generate fatal UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Fatal(string message, object context = null, object host = null) {}

		/// <summary>
		/// </summary>
		public LogLevel Level {
			get { return LogLevel.Fatal; }
			set { }
		}

		/// <summary>
		/// 	Writes out UserLog info
		/// </summary>
		/// <param name="level"> </param>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Write(LogLevel level, string message, object context, object host) {}

		/// <summary>
		/// 	Writes out fully complicated message
		/// </summary>
		/// <param name="logmessage"> </param>
		public void Write(LogMessage logmessage) {}
	}
}