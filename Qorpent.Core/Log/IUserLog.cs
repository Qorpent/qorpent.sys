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
// Original file : IUserLog.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

namespace Qorpent.Log {
	/// <summary>
	/// 	Abstract UserLog- user friendly API for calling log
	/// </summary>
	public interface IUserLog {
		/// <summary>
		/// 	Контекстный объект, к которому привязан данный логгер, контекстный объект будет 
		/// 	транслироваться в журнал как <see cref="LogMessage.HostObject" />
		/// </summary>
		object HostObject { get; set; }

		/// <summary>
		/// </summary>
		LogLevel Level { get; set; }

		/// <summary>
		/// 	Generates debug UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		void Debug(string message, object context = null, object host = null);

		/// <summary>
		/// 	Generate error UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> пользовательский Host </param>
		void Error(string message, object context = null, object host = null);

		/// <summary>
		/// 	Generate trace UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		void Trace(string message, object context = null, object host = null);

		/// <summary>
		/// 	Generate user info UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		void Info(string message, object context = null, object host = null);

		/// <summary>
		/// 	Generate warn UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		void Warn(string message, object context = null, object host = null);

		/// <summary>
		/// 	Generate fatal UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		void Fatal(string message, object context = null, object host = null);


		/// <summary>
		/// 	Writes out UserLog info
		/// </summary>
		/// <param name="level"> </param>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		void Write(LogLevel level, string message, object context, object host);

		/// <summary>
		/// 	Writes out fully complicated message
		/// </summary>
		/// <param name="logmessage"> </param>
		void Write(LogMessage logmessage);
	}
}