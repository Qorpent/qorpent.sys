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
// Original file : SimpleMvcContext.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Xml.Linq;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	basic QWeb context realization for usage in core-only scenario
	/// </summary>
	public class SimpleMvcContext : MvcContextBase {
		/// <summary>
		/// 	Retrievs xml-data parameter
		/// </summary>
		public override XElement XData { get; set; }

		/// <summary>
		/// 	Logon user - based on native HTTP context
		/// </summary>
		public override IPrincipal LogonUser { get; set; }

		/// <summary>
		/// 	Http status code
		/// </summary>
		public override int StatusCode { get; set; }

		/// <summary>
		/// 	Evaluated last modified state
		/// </summary>
		public override DateTime LastModified { get; set; }

		/// <summary>
		/// 	Evaluated etag
		/// </summary>
		public override string Etag { get; set; }

		/// <summary>
		/// 	Incoming if modiefied header for 304 state
		/// </summary>
		public override DateTime IfModifiedSince { get; set; }

		/// <summary>
		/// 	Incomiung If-None-Match header for 304 state
		/// </summary>
		public override string IfNoneMatch { get; set; }

		/// <summary>
		/// </summary>
		public override string ContentType { get; set; }

		/// <summary>
		/// 	Language of request
		/// </summary>
		public override string Language { get; set; }

		/// <summary>
		/// 	Set system/server defined execution context
		/// </summary>
		/// <param name="nativecontext"> </param>
		public override void SetNativeContext(object nativecontext) {}

		/// <summary>
		/// 	Generates parameters from underlined context
		/// </summary>
		/// <returns> </returns>
		protected override IDictionary<string, string> RetrieveParameters() {
			return new Dictionary<string, string>();
		}

		/// <summary>
		/// 	Safe method to acess parameters in context
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="def"> </param>
		/// <param name="setup"> </param>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		public override T Get<T>(string name, T def, bool setup = false) {
			if (typeof (T) != typeof (string)) {
				throw new Exception("only strings supported in simple context");
			}
			if (Parameters.ContainsKey(name)) {
				var result = Parameters[name];
				if (null == result) {
					return def;
				}
				return (T) ((object) result);
			}
			if (setup) {
				Parameters[name] = def as string;
			}
			return def;
		}

		/// <summary>
		/// 	Converts given parameter to typed array with splitters
		/// </summary>
		/// <param name="elementtype"> </param>
		/// <param name="name"> </param>
		/// <param name="splitters"> </param>
		/// <returns> </returns>
		public override Array GetArray(Type elementtype, string name, params char[] splitters) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// 	Converts given parameter to string array with splitters
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="splitters"> </param>
		/// <returns> </returns>
		public override string[] GetArray(string name, params char[] splitters) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// 	Return dicitonary representation of parameters in DictionaryForm
		/// </summary>
		/// <param name="paramname"> </param>
		/// <returns> </returns>
		public override IDictionary<string, string> GetDict(string paramname) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// 	Determine if request made from local system
		/// </summary>
		/// <returns> </returns>
		public override bool IsLocalHost() {
			return true;
		}

		/// <summary>
		/// </summary>
		/// <param name="filename"> </param>
		public override void WriteOutFile(string filename) {
			throw new NotImplementedException();
		}
	}
}