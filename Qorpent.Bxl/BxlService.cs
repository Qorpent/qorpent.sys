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
// Original file : BxlService.cs
// Project: Qorpent.Bxl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Xml.Linq;
using Qorpent.IoC;

namespace Qorpent.Bxl {
	/// <summary>
	/// 	Default BxlService implementation
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton, ServiceType = typeof (IBxlService))]
	public class BxlService : IBxlService {
		/// <summary>
		/// 	Perform Bxl parsing of given code
		/// </summary>
		/// <param name="code"> source code </param>
		/// <param name="filename"> filename for lexinfo </param>
		/// <param name="options"> BxlParsing options </param>
		/// <returns> XElement with xml equivalent </returns>
		public XElement Parse(string code, string filename = "code.bxl", BxlParserOptions options = BxlParserOptions.None) {
			return new BxlParser().Parse(code, filename, options);
		}

		/// <summary>
		/// 	Generates BXL code from XML with given settings
		/// </summary>
		/// <param name="sourcexml"> </param>
		/// <param name="options"> </param>
		/// <returns> </returns>
		public string Generate(XElement sourcexml, BxlGeneratorOptions options = null) {
			return new BxlGenerator().Convert(sourcexml, options);
		}

		/// <summary>
		/// 	Creates standalone parser instance
		/// </summary>
		/// <returns> </returns>
		public IBxlParser GetParser() {
			return new BxlParser();
		}
	}
}