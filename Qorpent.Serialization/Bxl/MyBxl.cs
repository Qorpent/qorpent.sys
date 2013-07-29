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
// PROJECT ORIGIN: Qorpent.Bxl/MyBxl.cs
#endregion
using System.IO;
using System.Xml.Linq;

namespace Qorpent.Bxl {
	/// <summary>
	/// 	Simple user-friendly gate for quick acess Bxl functionalyty
	/// </summary>
	public static class MyBxl {
		/// <summary>
		/// 	Parses BXL file into XElement
		/// </summary>
		/// <param name="filename"> source filename (default file-resolver will used) </param>
		/// <param name="nolexdata"> true if no lexinfo attributes needed </param>
		/// <param name="safeattributes"> true if safe attribute names for id,code,name needed </param>
		/// <seealso cref="BxlParserOptions" />
		/// <returns> Xml representation of given file </returns>
		public static XElement ParseFile(string filename, bool nolexdata = false, bool safeattributes = false) {
			return
				new BxlParser().Parse(
					File.ReadAllText(filename),
					Path.GetFileName(filename),
					GetOptions(nolexdata, safeattributes)
					);
		}

		/// <summary>
		/// 	Generates valid options from flags
		/// </summary>
		/// <param name="nolexdata"> </param>
		/// <param name="safeattributes"> </param>
		/// <param name="onlycode"> </param>
		/// <param name="onlyid"> </param>
		/// <returns> </returns>
		public static BxlParserOptions GetOptions(bool nolexdata = false, bool safeattributes = false, bool onlycode = false,
		                                          bool onlyid = false) {
			var result = BxlParserOptions.None;
			if (nolexdata) {
				result = result | BxlParserOptions.NoLexData;
			}
			if (safeattributes) {
				result = result | BxlParserOptions.SafeAttributeNames;
			}
			if (onlycode) {
				result = result | BxlParserOptions.OnlyCodeAttribute;
			}
			if (onlyid) {
				result = result | BxlParserOptions.OnlyIdAttibute;
			}
			return result;
		}

		/// <summary>
		/// 	Parses BXL code string inf XElement
		/// </summary>
		/// <param name="code"> source code string in BXL language </param>
		/// <param name="nolexdata"> true if no lexinfo attributes needed </param>
		/// <param name="safeattributes"> true if safe attribute names for id,code,name needed </param>
		/// <seealso cref="BxlParserOptions" />
		/// <returns> Xml representation of given code </returns>
		public static XElement Parse(string code, bool nolexdata = false, bool safeattributes = false) {
			return
				new BxlParser().Parse(
					code,
					"code.bxl",
					GetOptions(nolexdata, safeattributes))
				;
		}

		/// <summary>
		/// 	Converts given XElement to BXL code
		/// </summary>
		/// <param name="data"> xml to convert </param>
		/// <param name="options"> </param>
		/// <returns> BXL code representing given data </returns>
		/// <remarks>
		/// 	Uses default BXL generation settings, if you need more complex generation, use <see cref="BxlGenerator" /> direcly
		/// </remarks>
		public static string Convert(XElement data, BxlGeneratorOptions options = null) {
			return new BxlGenerator().Convert(data, options);
		}

		/// <summary>
		/// 	Converts given Xml file to BXL code
		/// </summary>
		/// <param name="filename"> xml file to convert (default FileResolver will be used) </param>
		/// <param name="options"> </param>
		/// <returns> BXL code representing given data </returns>
		/// <remarks>
		/// 	Uses default BXL generation settings, if you need more complex generation, use <see cref="BxlGenerator" /> direcly
		/// </remarks>
		public static string ConvertFile(string filename, BxlGeneratorOptions options = null) {
			return new BxlGenerator().Convert(XElement.Load(filename), options);
		}
	}
}