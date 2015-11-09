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
// PROJECT ORIGIN: Qorpent.Core/BaseLogger.cs
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;

namespace Qorpent.Log {
	/// <summary>
	/// 	Базовая реализация логгера
	/// </summary>
	public class BaseLogger : ServiceBase, ILogger {
        /// <summary>
        ///     Объект для локинга
        /// </summary>
        private readonly object _busylock = new object();
        /// <summary>
        ///     Признак «занятости» логгера — производится запись
        /// </summary>
        internal bool Busy = false;
        /// <summary>
        ///     Время окончания записи логгером
        /// </summary>
        internal DateTime Finish;
        /// <summary>
        ///     Время начала записи логгером
        /// </summary>
        internal DateTime Start;
        /// <summary>
        ///     Экземпляр исключения, произошедшего внутри потока
        /// </summary>
        private Exception _internalThreadError;
        /// <summary>
        ///     Внутреннее хранилище массива райтеров
        /// </summary>
        private IList<ILogWriter> _writers;
        /// <summary>
        /// 	Регулярное выражение, которое будет применено к тексту сообщения
        /// </summary>
        public string Filter { get; set; }
        /// <summary>
        /// 	Регулярное выражение, которое будет применимо к выданному контексту («*» — любой контекст)
        /// </summary>
        public string Mask { get; set; }
        /// <summary>
        /// 	Массив низкоуровневых менеджеров записи лога
        /// </summary>
        public IList<ILogWriter> Writers {
            get { return _writers ?? (_writers = LoadFromXmlSource().ToList()); }
            set { _writers = value.ToList(); }
        }
        /// <summary>
        /// 	Фильтр на пользователя
        /// </summary>
        public string UserFilter { get; set; }
        /// <summary>
        ///     Таймаут потока записи (используется в методе <see cref="Qorpent.Log.BaseLogger.Join"/>)
        /// </summary>
        public int WriteTimeOut { get; set; }
        /// <summary>
        /// 	Использует синхронные вызовы к райтерам вместо асинхронных процессов по умолчанию
        /// </summary>
        public bool Synchronized { get; set; }
        /// <summary>
        /// 	Человеко-понятное название логгера
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///     Методика поведения в ситуации сбоя логгера. Если значение не установлено, то будет
        ///     использовано значение менеджера по умолчанию
        /// </summary>
        public InternalLoggerErrorBehavior ErrorBehavior { get; set; }
        /// <summary>
        /// 	Уровень логгирования
        /// </summary>
        public LogLevel Level { get; set; }
        /// <summary>
        /// 	Указывает, что логгер может использоваться. В противном случае — false
        /// </summary>
        public bool Available { get; set; }
        /// <summary>
        /// 	Базовая реализация логгера
        /// </summary>
		public BaseLogger() {
			WriteTimeOut = 15000;
			ErrorBehavior = InternalLoggerErrorBehavior.Log | InternalLoggerErrorBehavior.Ignore;
			Available = true;
		}
        /// <summary>
        ///     Проверяет, что данный логгер применим к переданному контексту
        /// </summary>
        /// <param name="context">Контекст</param>
        /// <returns>True, если применим</returns>
		public virtual bool IsApplyable(object context) {
            if (!Active) return false;
			lock (Sync) {
				if (string.IsNullOrEmpty(Mask) || "*" == Mask) {
					return true; // логгер поддерживает весь контекст
				}
				if (null == context || "".Equals(context)) {
					return false; // логгер требует контекст, который не был передан
				}
				var contextstr = context.ToString();
				return Regex.IsMatch(contextstr, Mask, RegexOptions.Compiled);
				//in all other cases we checkout our regex on string representation of context
			}
		}
		/// <summary>
		///     Начинает запись сообщения UserLog в его цели, асинхронно
		/// </summary>
		/// <param name="message"> </param>
		public void StartWrite(LogMessage message) {
		    if (!Active) return;
			lock (Sync) {
				if (InvalidMessageText(message)) {
					return;
				}

				if (InvalidUser(message)) {
					return;
				}

				while (Busy) {
					Thread.Sleep(10);
				}

				Busy = true;
				Start = DateTime.Now;
				if (Synchronized) {
					InternalWrite(message);
				} else {
					ThreadPool.QueueUserWorkItem(InternalWrite, message);
				}
			}
		}
		/// <summary>
		///     Синхронизирует вызов контекста к логгеру
		/// </summary>
		public void Join() {
			lock (Sync) {
				while (Busy) {
					if (Start != DateTime.MinValue && Busy) {
						if ((DateTime.Now - Start).TotalMilliseconds > WriteTimeOut) {
							throw new LogException("logger timeout " + Name);
						}
					}
					Thread.Sleep(20);
				}
				Busy = false;

				if (_internalThreadError != null) {
					var ex = _internalThreadError;
					_internalThreadError = null;
					throw ex;
				}

				WriteTimeOut = 15000;
			}
		}

	    public bool Active { get; set; } = true;

	    /// <summary>
        ///     Загрузка из XML-представления
        /// </summary>
        /// <returns></returns>
		private ILogWriter[] LoadFromXmlSource() {
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
        /// <summary>
        ///     Загружает перечисление описаний райтеров из XML
        /// </summary>
        /// <param name="src">Исходный XML-элемент</param>
        /// <returns>Перечисление менеджеров записи <see cref="ILogWriter"/></returns>
		private IEnumerable<ILogWriter> GetAppendersFromXml(XElement src) {
			var elements = src.DescendantsAndSelf("writer");
			return elements.Select(GetWriter).Where(result => null != result);
		}
        /// <summary>
		/// 	Извлекает объект аппендера из элемента
		/// </summary>
		/// <param name="element"> </param>
		/// <returns> </returns>
		protected virtual ILogWriter GetWriter(XElement element) {
			return null;
		}
        /// <summary>
        ///     Прокатывание внутренней операции записи в лог
        /// </summary>
        /// <param name="message"></param>
		private void InternalWrite(object message) {
			lock (_busylock) {
				try {
					var lm = (LogMessage) message;

					foreach (var w in Writers) {
						if (w.Level <= lm.Level) {
							w.Write((LogMessage) message);
						}
					}
					Finish = DateTime.Now;
				}
				catch (LogException ex) {
					_internalThreadError = ex;
				}
				catch (Exception ex) {
					_internalThreadError = new LogException("error in logger " + Name, ex);
				}
				finally {
					Busy = false;
				}
			}
		}
        /// <summary>
        ///     Указывает на то, что имя пользователя, переданное в <see cref="LogMessage"/> некорректное
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
		private bool InvalidUser(LogMessage message) {
			if (!string.IsNullOrEmpty(UserFilter)) {
				if (UserFilter.Contains("/")) {
                    // полное имя
					if (message.User.Replace("\\", "/").ToLowerInvariant() != UserFilter.ToLowerInvariant()) {
						return true;
					}
				} else {
					// домен
					if (message.User.Split('/', '\\')[0] != UserFilter.ToLowerInvariant()) {
						return true;
					}
				}
			}
			return false;
		}
        /// <summary>
        ///     Указывает на то, что текст, переданный в <see cref="LogMessage"/> некорректный
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
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
	}
}