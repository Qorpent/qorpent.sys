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
// PROJECT ORIGIN: Qorpent.Core/IEventHandler.cs
#endregion
namespace Qorpent.Events {
	/// <summary>
	/// 	describe basic event handler
	/// </summary>
	public interface IEventHandler {
		/// <summary>
		/// 	process given event
		/// </summary>
		/// <param name="e"> </param>
		void Process(IEvent e);
	}

	/// <summary>
	/// 	typed event handler
	/// </summary>
	/// <typeparam name="T"> </typeparam>
	public interface IEventHandler<in T> : IEventHandler where T : IEvent {
		/// <summary>
		/// 	process given typed event
		/// </summary>
		/// <param name="e"> </param>
		void Process(T e);
	}
}