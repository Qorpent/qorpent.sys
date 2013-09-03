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
// PROJECT ORIGIN: Qorpent.Core/IEvent.cs
#endregion
using System;
using System.Collections.Generic;
using Qorpent.Applications;
using Qorpent.Log;

namespace Qorpent.Events {
	/// <summary>
	/// 	Describes Application-wide event
	/// </summary>
	public interface IEvent {
		/// <summary>
		/// 	Custom logger for event
		/// </summary>
		IUserLog UserLog { get; }

		/// <summary>
		/// 	Event Application Context
		/// </summary>
		IApplication Context { get; }

		/// <summary>
		/// 	Exceptions in event
		/// </summary>
		IList<Exception> Errors { get; }

		/// <summary>
		/// 	Setup context
		/// </summary>
		/// <param name="data"> Прямое указание данных для события </param>
		/// <param name="context"> </param>
		void SetContext(IEventData data = null, IApplication context = null);

		/// <summary>
		/// 	Setup UserLog
		/// </summary>
		/// <param name="listener"> </param>
		void SetLog(IUserLog listener);

		/// <summary>
		/// 	Возвращает результат выполнения события
		/// </summary>
		/// <returns> </returns>
		IEventResult GetResult();
	}

	/// <summary>
	/// 	Generic Event with defined data and result types
	/// </summary>
	/// <typeparam name="TData"> </typeparam>
	/// <typeparam name="TResult"> </typeparam>
	public interface IEvent<out TData, out TResult> : IEvent where TData : IEventData, new()
	                                                         where TResult : IEventResult, new() {
		/// <summary>
		/// 	Data of event
		/// </summary>
		TData Data { get; }

		/// <summary>
		/// 	Result supplied by event handler
		/// </summary>
		TResult Result { get; }
	                                                         }
}