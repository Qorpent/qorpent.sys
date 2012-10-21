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
// Original file : ISmartXsltTransform.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.IO;

namespace Qorpent.Dsl.SmartXslt {
	/// <summary>
	/// 	Xslt transform with addition preprocessing ability
	/// </summary>
	/// <remarks>
	/// </remarks>
	public interface ISmartXsltTransform {
		/// <summary>
		/// 	Setups the specified xsltsource.
		/// </summary>
		/// <param name="xsltsource"> The xsltsource. </param>
		/// <param name="codebase"> logical/phisical uri for url resolver </param>
		/// <param name="extensions"> The extensions. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		ISmartXsltTransform Setup(object xsltsource, string codebase, IEnumerable<XsltExtensionDefinition> extensions = null);

		/// <summary>
		/// 	Processes the specified contentsource into textwrite.
		/// </summary>
		/// <param name="contentsource"> The contentsource. </param>
		/// <param name="output"> The output. </param>
		/// <param name="additionParameters"> The addition parameters. </param>
		/// <remarks>
		/// </remarks>
		void Process(object contentsource, TextWriter output, IDictionary<string, object> additionParameters = null);

		/// <summary>
		/// 	Processes the specified contentsource and returns as string.
		/// </summary>
		/// <param name="contentsource"> The contentsource. </param>
		/// <param name="additionParameters"> The addition parameters. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		string Process(object contentsource, IDictionary<string, object> additionParameters = null);
	}
}