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
// PROJECT ORIGIN: Qorpent.Core/MvcConnectionFactory.cs
#endregion
using System.Net;
using Qorpent.IoC;

namespace Qorpent.Mvc.Remoting {
	/// <summary>
	/// 	Connection factory for Mvc remoting
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton)]
	public class MvcConnectionFactory : ServiceBase, IMvcConnectionFactory {
		/// <summary>
		/// 	Creates needed connection, based on url
		/// </summary>
		/// <param name="url"> </param>
		/// <param name="credentials"> </param>
		/// <returns> </returns>
		public IMvcConnection Create(string url, ICredentials credentials = null) {
			IMvcConnection result;
			result = ResolveService<IMvcConnection>(url == "(local)" ? "local.mvc.connection" : "remote.mvc.connection");
			result.SetUrl(url);
			result.SetCredentials(credentials);
			return result;
		}
	}
}