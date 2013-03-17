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
// PROJECT ORIGIN: Qorpent.Core/IXsltHelper.cs
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace Qorpent.Dsl.SmartXslt {
	/// <summary>
	/// Интерфейс вспомогательного класса для SmartXSLT
	/// </summary>
	public interface IXsltHelper {
		/// <summary>
		/// Automatically loads source file and applys given transform
		/// </summary>
		void Process(string source, TextWriter output, string transformFile = null);

		/// <summary>
		/// Automatically loads source file and applys given transform
		/// </summary>
		void Process(XElement source, TextWriter output, string transformFile);

		/// <summary>
		/// Automatically loads source file and applys given transform
		/// </summary>
		void Process(XElement source, XElement transform, XsltArgumentList args, IEnumerable<XsltExtensionDefinition> extensions, Uri transformUri, TextWriter output);

		/// <summary>
		/// 	embeds advanced includes, imports, parameters and extensions into target xslt stylesheet with control of duplicates
		/// </summary>
		/// <param name="source"> The source. </param>
		/// <param name="extensions"> The extensions. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		XElement PrepareXsltStylesheet(XElement source, IEnumerable<XsltExtensionDefinition> extensions);

		/// <summary>
		/// 	embeds extensions int XsltArgs - params and extensions will be used
		/// </summary>
		/// <param name="arguments"> The arguments. </param>
		/// <param name="extensions"> The extensions. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		XsltArgumentList PrepareXsltArguments(XsltArgumentList arguments,
		                                      IEnumerable<XsltExtensionDefinition> extensions);

		/// <summary>
		/// 	extract extensions from source xml with special namespace http://qorpent/xslt/extensions
		/// </summary>
		/// <param name="source"> The source. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		IEnumerable<XsltExtensionDefinition> ExtractExtensions(XElement source);
	}
}