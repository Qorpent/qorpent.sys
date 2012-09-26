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
// Original file : ConnectionBase.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Net;
using System.Threading;

namespace Qorpent.Mvc.Remoting {
	/// <summary>
	/// 	Abstract Mvc connection - just make wrap over call
	/// </summary>
	public abstract class ConnectionBase : IMvcConnection {
		/// <summary>
		/// 	Setup credential info for call
		/// </summary>
		/// <param name="credentials"> </param>
		public void SetCredentials(ICredentials credentials) {
			this.credentials = credentials;
		}

		/// <summary>
		/// 	Make synchronous call to Qorpent MVC
		/// </summary>
		/// <param name="query"> </param>
		/// <returns> </returns>
		public MvcResult Call(MvcQuery query) {
			lock (this) {
				return internalCall(query);
			}
		}

		/// <summary>
		/// 	Make asynchronous call to Qorpent MVC
		/// </summary>
		/// <param name="query"> </param>
		/// <param name="callback"> </param>
		public void Call(MvcQuery query, Action<MvcResult> callback) {
			ThreadPool.QueueUserWorkItem(s =>
				{
					var ac = (asynccall) s;
					var result = Call(ac.query);
					ac.callback(result);
				}, new asynccall {query = query, callback = callback});
		}

		/// <summary>
		/// 	Setups url
		/// </summary>
		/// <param name="url"> </param>
		public void SetUrl(string url) {
			this.url = url;
		}


		/// <summary>
		/// 	implement for real calling to mvc
		/// </summary>
		/// <param name="query"> </param>
		/// <returns> </returns>
		protected abstract MvcResult internalCall(MvcQuery query);

		#region Nested type: asynccall

		private struct asynccall {
			public Action<MvcResult> callback;
			public MvcQuery query;
		}

		#endregion

		/// <summary>
		/// 	credentials for calling
		/// </summary>
		protected ICredentials credentials;

		/// <summary>
		/// 	request base url
		/// </summary>
		protected string url;
	}
}