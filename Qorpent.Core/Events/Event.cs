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
// PROJECT ORIGIN: Qorpent.Core/Event.cs
#endregion
using System;
using System.Collections.Generic;
using Qorpent.Applications;
using Qorpent.Log;

namespace Qorpent.Events {
	/// <summary>
	/// 	Base event type, can used for custom events and for override
	/// </summary>
	/// <typeparam name="TData"> </typeparam>
	/// <typeparam name="TResult"> </typeparam>
	public class Event<TData, TResult> : IEvent<TData, TResult>
		where TData : IEventData, new()
		where TResult : IEventResult, new() {
		/// <summary>
		/// 	Setup context
		/// </summary>
		/// <param name="data"> прямое указание данных для события </param>
		/// <param name="context"> </param>
		public void SetContext(IEventData data = null, IApplication context = null) {
			_context = context;
			if (null != data) {
				_data = (TData) data;
			}
			if (Equals(null, _data)) {
				_data = new TData();
				if (null != context) {
					_data.Build(context);
				}
			}
			_result = new TResult();
		}

		/// <summary>
		/// 	Setup UserLog
		/// </summary>
		/// <param name="listener"> </param>
		public void SetLog(IUserLog listener) {
			_userLog = listener;
		}

		/// <summary>
		/// 	Возвращает результат выполнения события
		/// </summary>
		/// <returns> </returns>
		public IEventResult GetResult() {
			return Result;
		}

		/// <summary>
		/// 	Custom logger for event
		/// </summary>
		public IUserLog UserLog {
			get { return _userLog; }
		}

		/// <summary>
		/// 	Event Application Context
		/// </summary>
		public IApplication Context {
			get { return _context; }
		}

		/// <summary>
		/// 	Exceptions in event
		/// </summary>
		public IList<Exception> Errors {
			get { return _errors; }
		}

		/// <summary>
		/// 	Data of event
		/// </summary>
		public TData Data {
			get { return _data; }
			set { _data = value; }
		}

		/// <summary>
		/// 	Result supplied by event handler
		/// </summary>
		public TResult Result {
			get { return _result; }
		}

		private readonly IList<Exception> _errors = new List<Exception>();
		private IApplication _context;
		private TData _data;
		private TResult _result;
		private IUserLog _userLog;
		}
}