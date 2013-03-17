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
// PROJECT ORIGIN: Qorpent.Core/IXmlIncludeProcessor.cs
#endregion
using System.Xml.Linq;
using Qorpent.Bxl;

namespace Qorpent.Dsl.XmlInclude {
	/// <summary>
	/// 	Process file includes in given XElement, uses XmlIncludeNamespace
	/// </summary>
	public interface IXmlIncludeProcessor {
		/// <summary>
		/// 	Process given document with recursive including with support of delayed include (for extensions),
		/// 	supports include, import, both delayed and with XPath support.
		/// 	Additionaly in-document replaces supported with qxi::import "self" (always goes last)
		/// 	At last qxi::template and qxi::replace support must be provided (processed after all include/import)
		/// </summary>
		/// <param name="document"> </param>
		/// <param name="codebase"> </param>
		/// <param name="applyDelayed"> </param>
		/// <param name="options"> </param>
		/// <remarks>
		/// 	Шаблоны, замены и import self расцениваются как delay элементы то есть они обрабатывабтся только в итоговом
		/// 	целвом документе
		/// </remarks>
		/// <returns> </returns>
		XElement Include(XElement document, string codebase, bool applyDelayed = true,
		                 BxlParserOptions options = BxlParserOptions.None);

		/// <summary>
		/// 	reads file form disk (both XML or BXL) and processed includes
		/// </summary>
		/// <param name="path"> </param>
		/// <param name="applyDelayed"> </param>
		/// <param name="options"> </param>
		/// <returns> </returns>
		XElement Load(string path, bool applyDelayed = true, BxlParserOptions options = BxlParserOptions.None);
	}
}