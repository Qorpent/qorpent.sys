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
// PROJECT ORIGIN: Qorpent.Core/IMvcConnection.cs
#endregion
using System;
using System.Net;

namespace Qorpent.Mvc.Remoting {
	/// <summary>
	/// 	Interface of MvcRemoting API connection
	/// </summary>
	public interface IMvcConnection {
		/// <summary>
		/// 	Setup credential info for call
		/// </summary>
		/// <param name="credentials"> </param>
		void SetCredentials(ICredentials credentials);

		/// <summary>
		/// 	Make synchronous call to Qorpent MVC
		/// </summary>
		/// <param name="query"> </param>
		/// <returns> </returns>
		MvcResult Call(MvcQuery query);

		/// <summary>
		/// 	Make asynchronous call to Qorpent MVC
		/// </summary>
		/// <param name="query"> </param>
		/// <param name="callback"> </param>
		void Call(MvcQuery query, Action<MvcResult> callback);

		/// <summary>
		/// 	Setups url
		/// </summary>
		/// <param name="url"> </param>
		void SetUrl(string url);
	}
}