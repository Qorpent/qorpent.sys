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
// PROJECT ORIGIN: Qorpent.Core/IMvcConnectionFactory.cs
#endregion
using System.Net;

namespace Qorpent.Mvc.Remoting {
	/// <summary>
	/// 	Factory for remote Qorpent.Mvc connectios
	/// </summary>
	public interface IMvcConnectionFactory {
		/// <summary>
		/// 	Creates needed connection, based on url
		/// </summary>
		/// <param name="url"> </param>
		/// <param name="credentials"> </param>
		/// <returns> </returns>
		IMvcConnection Create(string url, ICredentials credentials = null);
	}
}