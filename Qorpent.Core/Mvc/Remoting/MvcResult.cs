﻿#region LICENSE

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
// Original file : MvcResult.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Mvc.Remoting {
	/// <summary>
	/// 	Result of remote MVC call
	/// </summary>
	public class MvcResult {
		/// <summary>
		/// 	An object with ActionResult (only for local calls)
		/// </summary>
		public object ActionResult { get; set; }

		/// <summary>
		/// 	Generated content (generated by render)
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// 	HTTP status of request (pseudo -codes for locals)
		/// </summary>
		public int Status { get; set; }

		/// <summary>
		/// 	Error, occured during call
		/// </summary>
		public Exception Error {
			get { return _error; }
			set {
				_error = value;
				ErrorString = "";
				if (null != value) {
					ErrorString = value.ToString();
				}
			}
		}

		/// <summary>
		/// 	Error string (for HTTP context)
		/// </summary>
		public string ErrorString { get; set; }

		/// <summary>
		/// 	304 last modified state (for custom caching)
		/// </summary>
		public DateTime LastModified { get; set; }

		/// <summary>
		/// 	304 etag (for custom caching)
		/// </summary>
		public string ETag { get; set; }

		private Exception _error;
	}
}