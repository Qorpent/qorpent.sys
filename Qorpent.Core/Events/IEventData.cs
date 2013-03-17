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
// PROJECT ORIGIN: Qorpent.Core/IEventData.cs
#endregion
using Qorpent.Applications;

namespace Qorpent.Events {
	/// <summary>
	/// 	Application bound event data, must support some logic to build from context
	/// </summary>
	public interface IEventData {
		/// <summary>
		/// 	setup initial state from given context
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		IEventData Build(IApplication context);
	}
}