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
// Original file : IEventManager.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Security.Principal;
using Qorpent.Log;

namespace Qorpent.Events {
	/// <summary>
	/// 	Event manager for calling and processing events and handlers
	/// </summary>
	public interface IEventManager {
		/// <summary>
		/// 	Производит полный цикл обработки события
		/// </summary>
		/// <param name="data"> Данные для события </param>
		/// <param name="user"> Если не null, то к хэндлерам будет применяться критерий соответствия прав у переданного пользователя, при null система работает без учета контекста безопасноти (в серверном режиме) </param>
		/// <param name="syslock"> True (по умолчанию) - если событие требует межпоточной синхронизации </param>
		/// <typeparam name="TEvent"> Тип события </typeparam>
		/// <typeparam name="TData"> Тип данных события </typeparam>
		/// <typeparam name="TResult"> Тип результата события </typeparam>
		/// <returns> </returns>
		TResult Call<TEvent, TData, TResult>(TData data, IPrincipal user = null, bool syslock = true)
			where TEvent : IEvent<TData, TResult>
			where TData : IEventData, new() where TResult : IEventResult, new();

		/// <summary>
		/// 	Вызов события по требуемому результату, должен автоматически диспетчироваться
		/// </summary>
		/// <param name="data"> Данные для события </param>
		/// <param name="user"> </param>
		/// <param name="syslock"> True (по умолчанию) - если событие требует межпоточной синхронизации </param>
		/// <typeparam name="TResult"> </typeparam>
		/// <returns> </returns>
		TResult Call<TResult>(IEventData data, IPrincipal user, bool syslock = true) where TResult : IEventResult, new();

		/// <summary>
		/// 	register event handler for event class
		/// </summary>
		/// <param name="handler"> </param>
		/// <param name="userLog"> </param>
		/// <typeparam name="TEvent"> </typeparam>
		void Add<TEvent>(IEventHandler<TEvent> handler, IUserLog userLog = null)
			where TEvent : IEvent;

		/// <summary>
		/// 	Removes given event handler
		/// </summary>
		/// <typeparam name="TEvent"> </typeparam>
		/// <param name="handler"> </param>
		/// <param name="userLog"> </param>
		void Remove<TEvent>(IEventHandler<TEvent> handler, IUserLog userLog = null)
			where TEvent : IEvent;

		///<summary>
		///	Collect all event handlers for given event type
		///</summary>
		///<param name="eventType"> Тип события </param>
		///<param name="user"> </param>
		///<returns> </returns>
		IEventHandler[] GetHandlers(Type eventType, IPrincipal user = null);

		/// <summary>
		/// 	Подготваливает класс для вызова события
		/// </summary>
		/// <param name="eventType"> Тип события </param>
		/// <param name="user"> </param>
		/// <param name="syslock"> True (по умолчанию) - если событие требует межпоточной синхронизации </param>
		/// <returns> </returns>
		IEventInvoker Prepare(Type eventType, IPrincipal user = null, bool syslock = true);
	}
}