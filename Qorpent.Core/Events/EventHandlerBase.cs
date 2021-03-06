﻿#region LICENSE
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
// PROJECT ORIGIN: Qorpent.Core/EventHandlerBase.cs
#endregion
namespace Qorpent.Events {
	/// <summary>
	/// 	Base event handler implementation
	/// </summary>
	/// <typeparam name="T"> </typeparam>
	public abstract class EventHandlerBase<T> : IEventHandler<T> where T : IEvent {
		/// <summary>
		/// 	process given typed event
		/// </summary>
		/// <param name="e"> </param>
		public abstract void Process(T e);

		/// <summary>
		/// 	process given event
		/// </summary>
		/// <param name="e"> </param>
		public void Process(IEvent e) {
			Process((T) e);
		}
	}
}