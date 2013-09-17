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
// PROJECT ORIGIN: Qorpent.Core/IUserLog.cs
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
		///     Уровень логгирования
		/// </summary>
		LogLevel Level { get; set; }
		/// <summary>
        /// 	Сгенерировать сообщение в лог уровня «Debug»
		/// </summary>
        /// <param name="message">Сообщение в лог</param>
        /// <param name="context">Контекст сообщения</param>
        /// <param name="host">Хост-объект</param>
		void Debug(string message, object context = null, object host = null);
		/// <summary>
        /// 	Сгенерировать сообщение в лог уровня «Error»
		/// </summary>
        /// <param name="message">Сообщение в лог</param>
        /// <param name="context">Контекст сообщения</param>
        /// <param name="host">Хост-объект</param>
		void Error(string message, object context = null, object host = null);
		/// <summary>
        /// 	Сгенерировать сообщение в лог уровня «Trace»
		/// </summary>
        /// <param name="message">Сообщение в лог</param>
        /// <param name="context">Контекст сообщения</param>
        /// <param name="host">Хост-объект</param>
		void Trace(string message, object context = null, object host = null);
		/// <summary>
        /// 	Сгенерировать сообщение в лог уровня «Info»
		/// </summary>
        /// <param name="message">Сообщение в лог</param>
        /// <param name="context">Контекст сообщения</param>
        /// <param name="host">Хост-объект</param>
		void Info(string message, object context = null, object host = null);
		/// <summary>
        /// 	Сгенерировать сообщение в лог уровня «Warn»
		/// </summary>
        /// <param name="message">Сообщение в лог</param>
        /// <param name="context">Контекст сообщения</param>
        /// <param name="host">Хост-объект</param>
		void Warn(string message, object context = null, object host = null);
		/// <summary>
		/// 	Сгенерировать сообщение в лог уровня «Fatal»
		/// </summary>
		/// <param name="message">Сообщение в лог</param>
		/// <param name="context">Контекст сообщения</param>
		/// <param name="host">Хост-объект</param>
		void Fatal(string message, object context = null, object host = null);
		/// <summary>
        /// 	Записть в лог сообщение произвольного уровня
		/// </summary>
		/// <param name="level">Уровень логгирования</param>
        /// <param name="message">Сообщение в лог</param>
        /// <param name="context">Контекст сообщения</param>
        /// <param name="host">Хост-объект</param>
		void Write(LogLevel level, string message, object context, object host);
		/// <summary>
		/// 	Записывает в лог полностью скомплектованное сообщение в виде <see cref="LogMessage"/>
		/// </summary>
		/// <param name="logmessage"> </param>
		void Write(LogMessage logmessage);
	}
}