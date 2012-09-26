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
// Original file : AuthorizationResult.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using Qorpent.Serialization;

namespace Qorpent.Security {
	/// <summary>
	/// 	Result of authorization process
	/// </summary>
	[Serialize]
	public class AuthorizationResult {
		/// <summary>
		/// 	static constructor for quick OK answer
		/// </summary>
		[IgnoreSerialize] public static AuthorizationResult OK {
			get { return new AuthorizationResult {Authorized = true}; }
		}


		/// <summary>
		/// 	True if result is valid
		/// </summary>
		public bool Authorized { get; set; }

		/// <summary>
		/// 	Exception with authorization error
		/// </summary>
		[SerializeNotNullOnly] [Serialize] public Exception AuthorizeError { get; set; }

		/// <summary>
		/// 	Static authorization error contructor
		/// </summary>
		/// <param name="error"> </param>
		/// <returns> </returns>
		public static AuthorizationResult Error(Exception error) {
			return new AuthorizationResult {Authorized = false, AuthorizeError = error};
		}
	}
}