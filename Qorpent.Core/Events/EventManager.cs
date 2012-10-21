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
// Original file : EventManager.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Qorpent.Applications;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Security;

namespace Qorpent.Events {
	/// <summary>
	/// 	Default implementation of IEventManager, functional, support default event behaviour
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton)]
	public class EventManager : ServiceBase, IEventManager {
		/// <summary>
		/// 	Подготваливает класс для вызова события - позволяет вызывающей стороне обеспечить собственный процесс обработки и блокировки события
		/// </summary>
		/// <returns> </returns>
		public IEventInvoker Prepare(Type eventType, IPrincipal user = null, bool syslock = true) {
			lock (Sync) {
				return new Invoker(
					eventType,
					Application,
					GetHandlers(eventType, user).ToArray(),
					EnvironmentInfo.Sync,
					Log
					);
			}
		}

		/// <summary>
		/// 	Производит полный цикл обработки события
		/// </summary>
		/// <param name="data"> Данные для события </param>
		/// <param name="user"> </param>
		/// <param name="syslock"> True (по умолчанию) - если событие требует межпоточной синхронизации </param>
		/// <typeparam name="TEvent"> Тип события </typeparam>
		/// <typeparam name="TData"> Тип данных события </typeparam>
		/// <typeparam name="TResult"> Тип результата события </typeparam>
		/// <returns> </returns>
		public TResult Call<TEvent, TData, TResult>(TData data, IPrincipal user = null, bool syslock = true)
			where TEvent : IEvent<TData, TResult> where TData : IEventData, new() where TResult : IEventResult, new() {
			lock (Sync) {
				var invoker = Prepare(typeof (TEvent), user, syslock);
				var result = invoker.Invoke(data);
				return (TResult) result;
			}
		}

		/// <summary>
		/// 	Вызов события по требуемому результату, должен автоматически диспетчироваться
		/// </summary>
		/// <param name="data"> Данные для события </param>
		/// <param name="user"> </param>
		/// <param name="syslock"> True (по умолчанию) - если событие требует межпоточной синхронизации </param>
		/// <typeparam name="TResult"> </typeparam>
		/// <returns> </returns>
		public TResult Call<TResult>(IEventData data, IPrincipal user = null, bool syslock = true)
			where TResult : IEventResult, new() {
			lock (Sync) {
				//находим среди всех поддерживаемых сообщений первое, которое возвращает запрошенный TResult
				var eventType =
					_handlers.Keys.Where(type => type.BaseType != null && type.BaseType.IsGenericType)
						.FirstOrDefault(
							type => type.BaseType != null && typeof (TResult).IsAssignableFrom(type.BaseType.GetGenericArguments()[1]));
				if (null == eventType) {
					return new TResult();
				}
				var invoker = Prepare(eventType, user, syslock);
				var result = invoker.Invoke(data);
				return (TResult) result;
			}
		}

		/// <summary>
		/// 	register event handler for event class
		/// </summary>
		/// <param name="handler"> </param>
		/// <param name="userLog"> </param>
		/// <typeparam name="TEvent"> </typeparam>
		public void Add<TEvent>(IEventHandler<TEvent> handler, IUserLog userLog = null)
			where TEvent : IEvent {
			lock (Sync) {
				if (!_handlers.ContainsKey(typeof (TEvent))) {
					_handlers[typeof (TEvent)] = new List<IEventHandler>();
				}
				_handlers[typeof (TEvent)].Add(handler);
				Log.Debug("event handler " + handler + " added for " + typeof (TEvent).Name + " event ");
			}
		}

		/// <summary>
		/// 	Removes given event handler
		/// </summary>
		/// <typeparam name="TEvent"> </typeparam>
		/// <param name="handler"> </param>
		/// <param name="userLog"> </param>
		public void Remove<TEvent>(IEventHandler<TEvent> handler, IUserLog userLog = null)
			where TEvent : IEvent {
			lock (Sync) {
				if (!_handlers.ContainsKey(typeof (TEvent))) {
					return;
				}
				_handlers[typeof (TEvent)].Remove(handler);
				Log.Debug("event handler " + handler + " removed from " + typeof (TEvent).Name + " event ");
			}
		}

		/// <summary>
		/// 	Collect all event handlers for given event type
		/// </summary>
		/// <param name="eventType"> Тип события </param>
		/// <param name="user"> </param>
		/// <returns> </returns>
		public IEventHandler[] GetHandlers(Type eventType, IPrincipal user = null) {
			var result = _handlers.ContainsKey(eventType)
				             ? _handlers[eventType].ToArray()
				             : new IEventHandler[] {};
			if (user != null) {
				result = result.Where(x => Application.Access.IsAccessible(x, AccessRole.Execute, user)).ToArray();
			}
			return result;
		}


		/// <summary>
		/// 	EventHandler не настраивается сам на себя
		/// </summary>
		protected override void SetupResetReaction() {}

		#region Nested type: Invoker

		private class Invoker : IEventInvoker {
			public Invoker(Type eventType,
			               IApplication context, IEventHandler[] handlers, object locker, IUserLog userLog) {
				_hs = handlers;
				_userLog = userLog;
				_locker = locker ?? new object();
				_context = context;
				_eventType = eventType;
			}


			public IEventResult Invoke(IEventData eventData = null) {
				lock (_locker) {
					if (_hs.Length == 0) {
						return null;
					}
					var e = (IEvent) Activator.CreateInstance(_eventType);
					e.SetContext(eventData, _context);
					foreach (var h in _hs) {
						_userLog.Debug("start handler " + h, e.Context);
						try {
							h.Process(e);
							_userLog.Debug("end handler " + h, e.Context);
						}
						catch (Exception ex) {
							_userLog.Error("error handler " + h + " " + ex, e.Context);
						}
					}
					return e.GetResult();
				}
			}

			private readonly IApplication _context;
			private readonly Type _eventType;
			private readonly IEventHandler[] _hs;
			private readonly object _locker;
			private readonly IUserLog _userLog;
		}

		#endregion

		private readonly IDictionary<Type, IList<IEventHandler>> _handlers = new Dictionary<Type, IList<IEventHandler>>();
	}
}