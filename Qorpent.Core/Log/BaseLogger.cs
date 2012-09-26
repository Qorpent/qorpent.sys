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
// Original file : BaseLogger.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;

namespace Qorpent.Log {
	/// <summary>
	/// 	Basic logger implementation
	/// </summary>
	public class BaseLogger : ServiceBase, ILogger {
		/// <summary>
		/// </summary>
		public BaseLogger() {
			WriteTimeOut = 5000;
			ErrorBehavior = InternalLoggerErrorBehavior.Log | InternalLoggerErrorBehavior.Ignore;
			Available = true;
			new Thread(InternalWrite);
		}

		/// <summary>
		/// 	Regex to be applyed to text of message
		/// </summary>
		public string Filter { get; set; }

		/// <summary>
		/// 	Regex to be apply to given context - * for all context
		/// </summary>
		public string Mask { get; set; }

		/// <summary>
		/// 	Low level writer of UserLog
		/// </summary>
		public ILogWriter[] Writers {
			get { return _writers ?? (_writers = loadFromXmlSource()); }
			set { _writers = value; }
		}

		/// <summary>
		/// 	Фильтр на пользователя
		/// </summary>
		public string UserFilter { get; set; }

		/// <summary>
		/// 	Time out of write thread - will be used by Join
		/// </summary>
		public int WriteTimeOut { get; set; }

		/// <summary>
		/// 	Uses synchronous calls to writer instead of async default process
		/// </summary>
		public bool Synchronized { get; set; }


		/// <summary>
		/// 	user friendly name of logger
		/// </summary>
		public string Name { get; set; }


		/// <summary>
		/// 	Behavior on error occured in logger - if None - Manager behavior will be used
		/// </summary>
		public InternalLoggerErrorBehavior ErrorBehavior { get; set; }

		/// <summary>
		/// 	check's if this logger is applyable to given context
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		public virtual bool IsApplyable(object context) {
			lock (this) {
				if (string.IsNullOrEmpty(Mask) || "*" == Mask) {
					return true; //logger supports all contexts
				}
				if (null == context || "".Equals(context)) {
					return false; //logger need context, but it's not supplyed
				}
				var contextstr = context.ToString();
				return Regex.IsMatch(contextstr, Mask, RegexOptions.Compiled);
				//in all other cases we checkout our regex on string representation of context
			}
		}

		/// <summary>
		/// 	Starts write UserLog message to it's target persistence  - asserted to be async
		/// </summary>
		/// <param name="message"> </param>
		public void StartWrite(LogMessage message) {
			lock (this) {
				if (InvalidMessageText(message)) {
					return;
				}
				if (InvalidUser(message)) {
					return;
				}
				while (busy) {
					Thread.Sleep(10);
				}
				busy = true;
				_start = DateTime.Now;
				if (Synchronized) {
					InternalWrite(message);
				}
				else {
					ThreadPool.QueueUserWorkItem(InternalWrite, message);
				}
			}
		}

		/// <summary>
		/// 	Synchronizes calling context to logger
		/// </summary>
		public void Join() {
			lock (this) {
				while (busy) {
					if (_start != DateTime.MinValue && busy) {
						if ((DateTime.Now - _start).TotalMilliseconds > WriteTimeOut) {
							throw new LogException("logger timeout " + Name);
						}
					}
					Thread.Sleep(20);
				}
				busy = false;

				if (_internalThreadError != null) {
					var ex = _internalThreadError;
					_internalThreadError = null;
					throw ex;
				}

				WriteTimeOut = 1000;
			}
		}

		/// <summary>
		/// 	Level of logger
		/// </summary>
		public LogLevel Level { get; set; }

		/// <summary>
		/// 	Marks logger to be used - false - disable logger
		/// </summary>
		public bool Available { get; set; }


		private ILogWriter[] loadFromXmlSource() {
			if (null == Container) {
				return null;
			}
			if (null == Component) {
				return null;
			}
			if (null == Component.Source) {
				return null;
			}
			return GetAppendersFromXml(Component.Source).ToArray();
		}

		private IEnumerable<ILogWriter> GetAppendersFromXml(XElement src) {
			var elements = src.DescendantsAndSelf("writer");
			foreach (var element in elements) {
				var result = GetWriter(element);
				if (null != result) {
					yield return result;
				}
			}
		}

		/// <summary>
		/// 	Извлекает объект аппендера из элемента
		/// </summary>
		/// <param name="element"> </param>
		/// <returns> </returns>
		protected virtual ILogWriter GetWriter(XElement element) {
			return null;
		}

		private void InternalWrite(object message) {
			lock (busylock) {
				try {
					var lm = (LogMessage) message;

					foreach (var w in Writers) {
						if (w.Level <= lm.Level) {
							w.Write((LogMessage) message);
						}
					}
					_finish = DateTime.Now;
				}
				catch (LogException ex) {
					_internalThreadError = ex;
				}
				catch (Exception ex) {
					_internalThreadError = new LogException("error in logger " + Name, ex);
				}
				finally {
					busy = false;
				}
			}
		}

		private bool InvalidUser(LogMessage message) {
			if (!string.IsNullOrEmpty(UserFilter)) {
				if (UserFilter.Contains("/")) {
//полное имя
					if (message.User.Replace("\\", "/").ToLowerInvariant() != UserFilter.ToLowerInvariant()) {
						return true;
					}
				}
				else {
					//домен
					if (message.User.Split('/', '\\')[0] != UserFilter.ToLowerInvariant()) {
						return true;
					}
				}
			}
			return false;
		}

		private bool InvalidMessageText(LogMessage message) {
			if (!string.IsNullOrEmpty(Filter)) {
				if (string.IsNullOrEmpty(message.Message)) {
					return true;
				}
				if (!Regex.IsMatch(message.Message, Filter, RegexOptions.Compiled)) {
					return true;
				}
			}
			return false;
		}

		private readonly object busylock = new object();
		internal DateTime _finish;
		private Exception _internalThreadError;

		internal DateTime _start;
		private ILogWriter[] _writers;
		internal bool busy = false;
	}
}