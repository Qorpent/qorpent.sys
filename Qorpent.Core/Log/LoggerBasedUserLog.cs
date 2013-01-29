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
// Original file : LoggerBasedUserLog.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Applications;

namespace Qorpent.Log {
	/// <summary>
	/// 	Logger avared UserLog listener - convert given data to message and process it on loggers
	/// </summary>
	public class LoggerBasedUserLog : IUserLog {
		/// <summary>
		/// </summary>
		/// <param name="loggers"> </param>
		/// <param name="manager"> </param>
		/// <param name="name"> </param>
		public LoggerBasedUserLog(IEnumerable<ILogger> loggers, ILogManager manager, string name) {
			_loggers = loggers ?? new ILogger[] {};
			_helper = new LogHelper();
			_manager = manager;
			_name = name;
		}

		/// <summary>
		/// 	Reserved (not supported by framework itself) 
		/// 	set it true to log all copys and levels of exception
		/// 	if false (by default) exception-bound errors , warnings
		/// 	will be just in one copy in nest-most level of log,
		/// 	except that new message is LOWER by level than basic
		/// </summary>
		public bool AllExceptionsInLogStack { get; set; }


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
		public void Debug(string message, object context = null, object host = null) {
			lock (this) {
				Write(LogLevel.Debug, message, context, host);
			}
		}

		/// <summary>
		/// 	Generate error UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Error(string message, object context = null, object host = null) {
			lock (this) {
				Write(LogLevel.Error, message, context, host);
			}
		}

		/// <summary>
		/// 	Generate trace UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Trace(string message, object context = null, object host = null) {
			lock (this) {
				Write(LogLevel.Trace, message, context, host);
			}
		}

		/// <summary>
		/// 	Generate user info UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Info(string message, object context = null, object host = null) {
			lock (this) {
				Write(LogLevel.Info, message, context, host);
			}
		}

		/// <summary>
		/// 	Generate warn UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Warn(string message, object context = null, object host = null) {
			lock (this) {
				Write(LogLevel.Warning, message, context, host);
			}
		}

		/// <summary>
		/// 	Generate fatal UserLog message
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Fatal(string message, object context = null, object host = null) {
			lock (this) {
				Write(LogLevel.Fatal, message, context, host);
			}
		}

		/// <summary>
		/// 	Writes out UserLog info
		/// </summary>
		/// <param name="level"> </param>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		public void Write(LogLevel level, string message, object context, object host) {
			if (Level > level) {
				return;
			}
			lock (this) {
				var logmessage = _helper.CreateMessage(level, message, context);
				if (Application.HasCurrent) {
					if(string.IsNullOrWhiteSpace(logmessage.User)) {
						logmessage.User = Application.Current.Principal.CurrentUser.Identity.Name;
					}
					logmessage.ApplicationName = Application.Current.ApplicationName;
				}
				logmessage.HostObject = host ?? HostObject;
				logmessage.Name = _name;
				//call UserLog write asynchronous - SO WE DON'T WANT WAIT LOGGERS to finish operations, we call synchronization on start of UserLog operation
				Write(logmessage);
			}
		}


		/// <summary>
		/// 	Writes out fully complicated message
		/// </summary>
		/// <param name="logmessage"> </param>
		public void Write(LogMessage logmessage) {
			if (IsDuplicatedMessage(logmessage)) {
				return;
			}


			if (null == logmessage.HostObject) {
				logmessage.HostObject = HostObject;
			}
			foreach (var logger in _loggers) {
				if (logger.Available && logger.Level <= logmessage.Level) {
					try {
						logger.Join();
						logger.StartWrite(logmessage);
					}
					catch (LogException logerror) {
						var errorlogic = logger.ErrorBehavior;
						if (InternalLoggerErrorBehavior.None == errorlogic && null != _manager) {
							errorlogic = _manager.ErrorBehavior;
						}
						if (0 != (errorlogic & InternalLoggerErrorBehavior.AutoDisable)) {
							logger.Available = false;
						}
						if (0 != (errorlogic & InternalLoggerErrorBehavior.Log)) {
							if (_manager != null) {
								_manager.LogFailSafe(new LogMessage {Error = logerror});
							}
						}
						if (0 != (errorlogic & InternalLoggerErrorBehavior.Throw)) {
							throw;
						}
					}
				}
			}
		}

		/// <summary>
		/// 	Level of UserLog
		/// </summary>
		public LogLevel Level {
			get { return _level == LogLevel.All ? (_level = _loggers.Select(x => x.Level).Min()) : _level; }
			set { }
		}


		/// <summary>
		/// 	Вычисляет - не является ли сообщение об ошибке дублирующим предыдущее (может возникнуть в стеке ошибок)
		/// 	так как контекст вызова содержится в трейс-информации исключения, нет смысла (в обычном случае)
		/// 	отправлять на один логер все моменты журналирования одного и того же исключения
		/// 	посредством параметра <see cref="AllExceptionsInLogStack" /> зарезервирована возможность включения альтернативного
		/// 	поведения, однако в самом ядре Qorpent нет прямой поддержки включения данного режима
		/// </summary>
		/// <param name="logmessage"> </param>
		/// <returns> </returns>
		protected bool IsDuplicatedMessage(LogMessage logmessage) {
			if (null != logmessage.Error && !AllExceptionsInLogStack) {
				if (LastException != logmessage.Error) {
					LastException = logmessage.Error;
					LastExceptionLevel = logmessage.Level;
				}
				else {
					if (LastExceptionLevel > logmessage.Level) {
						LastExceptionLevel = logmessage.Level;
					}
					else {
						return true;
					}
				}
			}
			return false;
		}

		private readonly LogHelper _helper;
		private readonly IEnumerable<ILogger> _loggers;
		private readonly ILogManager _manager;
		private readonly string _name;

		/// <summary>
		/// 	Последнее обработанное исключение (хранится для контроля дублирования исключений,
		/// 	<see cref="AllExceptionsInLogStack" />, <see cref="IsDuplicatedMessage" />).
		/// </summary>
		protected Exception LastException;

		/// <summary>
		/// 	Уровень регистрации последнего исключения(хранится для контроля дублирования исключений,
		/// 	<see cref="AllExceptionsInLogStack" />, <see cref="IsDuplicatedMessage" />)
		/// </summary>
		protected LogLevel LastExceptionLevel;

		private LogLevel _level;
	}
}