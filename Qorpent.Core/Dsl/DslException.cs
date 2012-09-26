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
// Original file : DslException.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Dsl {
	/// <summary>
	/// 	General DSL exception
	/// </summary>
	[Serializable]
	public class DslException : Exception {
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		/// <summary>
		/// 	Initializes a new instance of the <see cref="DslException" /> class.
		/// </summary>
		/// <param name="dslMessage"> The DSL message. </param>
		/// <remarks>
		/// </remarks>
		public DslException(DslMessage dslMessage = null) {
			DslMessage = dslMessage;
		}

		/// <summary>
		/// 	Initializes a new instance of the <see cref="DslException" /> class.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <param name="dslMessage"> The DSL message. </param>
		/// <remarks>
		/// </remarks>
		public DslException(string message, DslMessage dslMessage = null)
			: base(message) {
			DslMessage = dslMessage;
		}

		/// <summary>
		/// 	Initializes a new instance of the <see cref="DslException" /> class.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <param name="inner"> The inner. </param>
		/// <param name="dslMessage"> The DSL message. </param>
		/// <remarks>
		/// </remarks>
		public DslException(string message, Exception inner, DslMessage dslMessage = null) : base(message, inner) {
			DslMessage = dslMessage;
		}

		/// <summary>
		/// </summary>
		public DslMessage DslMessage { get; private set; }
	}
}