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
// PROJECT ORIGIN: Qorpent.Core/StubUserLog.cs
#endregion

using System.Collections.Generic;

namespace Qorpent.Log {
	/// <summary>
	/// 	mute out logging
	/// </summary>
	public class StubUserLog : IUserLog {
// ReSharper disable InconsistentNaming
		private static readonly IUserLog _default = new StubUserLog();
		private IList<IUserLog> _subLoggers =new List<IUserLog>();
// ReSharper restore InconsistentNaming

		/// <summary>
		/// 	Default pseudo logger
		/// </summary>
		public static IUserLog Default {
			get { return _default; }
		}


		/// <summary>
		/// 	Контекстный объект, к которому привязан данный логгер, контекстный объект будет 
		/// 	транслироваться в журнал как <see cref="LogMessage.HostObject" />
		/// </summary>
		public object HostObject { get; set; }

		/// <summary>
		/// 	Generates debug UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Debug(string message, object context = null, object host = null){
			foreach (var subLogger in SubLoggers){
				subLogger.Debug(message,context,host);
			}
		}

		/// <summary>
		/// 	Generate error UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Error(string message, object context = null, object host = null){
			foreach (var subLogger in SubLoggers)
			{
				subLogger.Error(message, context, host);
			}
		}

		/// <summary>
		/// 	Generate trace UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Trace(string message, object context = null, object host = null){
			foreach (var subLogger in SubLoggers)
			{
				subLogger.Trace(message, context, host);
			}
		}

		/// <summary>
		/// 	Generate user info UserLog message
		/// </summary>
		/// <param name="item"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Info(string item, object context = null, object host = null){
			foreach (var subLogger in SubLoggers)
			{
				subLogger.Info(item, context, host);
			}
		}

		/// <summary>
		/// 	Generate warn UserLog message
		/// </summary>
		/// <param name="item"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Warn(string item, object context = null, object host = null){
			foreach (var subLogger in SubLoggers)
			{
				subLogger.Warn(item, context, host);
			}
		}

		/// <summary>
		/// 	Generate fatal UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Fatal(string message, object context = null, object host = null){
			foreach (var subLogger in SubLoggers)
			{
				subLogger.Warn(message, context, host);
			}
		}

		private LogLevel _level = LogLevel.Fatal;
		/// <summary>
		/// </summary>
		public LogLevel Level {
			get { return _level; }
			set { _level = value; }
		}

		/// <summary>
		/// 	Writes out UserLog info
		/// </summary>
		/// <param name="level"> </param>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Write(LogLevel level, string message, object context, object host){
			foreach (var subLogger in SubLoggers)
			{
				subLogger.Write(level,message, context, host);
			}
		}

		/// <summary>
		/// 	Writes out fully complicated message
		/// </summary>
		/// <param name="logmessage"> </param>
		public void Write(LogMessage logmessage){
			foreach (var subLogger in SubLoggers)
			{
				subLogger.Write(logmessage);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public IList<IUserLog> SubLoggers{
			get { return _subLoggers; }
		}

	    /// <summary>
	    /// Синхронизатор журнала
	    /// </summary>
	    public void Synchronize() {
		    foreach (var subLogger in SubLoggers) {
			    subLogger.Synchronize();
		    }
	    }

	    /// <summary>
	    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
	    /// </summary>
	    /// <filterpriority>2</filterpriority>
	    public void Dispose() {
		    foreach (var subLogger in SubLoggers) {
			    subLogger.Dispose();
		    }
	    }
	}
}