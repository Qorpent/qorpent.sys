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
// Original file : BxlException.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using Qorpent.Dsl;

namespace Qorpent.Bxl {
	/// <summary>
	/// 	Throws on any problems occured during Bxl processing
	/// </summary>
	[Serializable]
	public class BxlException : Exception {
		/// <summary>
		/// 	Creates new instance of exception
		/// </summary>
		/// <param name="message"> some user message </param>
		/// <param name="inner"> inner wrapped exception </param>
		/// <param name="lexinfo"> lexinfo of item caused exception </param>
		public BxlException(string message = "", LexInfo lexinfo = new LexInfo(), Exception inner = null)
			: base(message, inner) {
			LexInfo = lexinfo;
		}

		/// <summary>
		/// 	Erorr source lex info
		/// </summary>
		public readonly LexInfo LexInfo;
	}
}