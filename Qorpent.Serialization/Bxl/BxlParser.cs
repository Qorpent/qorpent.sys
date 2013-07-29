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
// PROJECT ORIGIN: Qorpent.Bxl/BxlParser.cs
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Qorpent.Dsl;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Bxl {
	/// <summary>
	/// 	Main gate to complexy access to Bxl parsing modes
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, ServiceType = typeof (IBxlParser))]
	[ContainerComponent(Lifestyle.Transient, ServiceType = typeof (ISpecialXmlParser), Name = "bxl.xml.parser")]
	public class BxlParser : IBxlParser, ISpecialXmlParser {
		/// <summary>
		/// 	Parses source code into Xml
		/// </summary>
		/// <param name="code"> </param>
		/// <param name="filename"> </param>
		/// <param name="options"> </param>
		/// <returns> </returns>
// ReSharper disable MethodOverloadWithOptionalParameter (other is implicit interface)
		public XElement Parse(string code = null, string filename = "code.bxl",BxlParserOptions options = BxlParserOptions.None) {
// ReSharper restore MethodOverloadWithOptionalParameter
			if (filename == null) {
				throw new ArgumentNullException("filename");
			}
			if (code.IsEmpty()) {
				if(string.IsNullOrWhiteSpace(filename))return new XElement("root");
				code = File.ReadAllText(filename);
			}
			if(string.IsNullOrWhiteSpace(filename)) filename = "code.bxl";
			var tokens = ParseTokens(code, filename, options);
			var generator = new BxlXmlGenerator();
#if !SQL2008
			var result = generator.Generate(tokens, filename,
			                                !(options.HasFlag(BxlParserOptions.NoLexData)),
			                                options.HasFlag(BxlParserOptions.SafeAttributeNames),
			                                options.HasFlag(BxlParserOptions.OnlyCodeAttribute),
			                                options.HasFlag(BxlParserOptions.ExtractSingle)
				);
#else
			var result = generator.Generate(tokens, filename,
											0==(options&BxlParserOptions.NoLexData),
											0!=(options&BxlParserOptions.SafeAttributeNames),
											0!=(options&BxlParserOptions.OnlyCodeAttribute)
				);
#endif
			return result;
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


		XElement ISpecialXmlParser.ParseXml(string srccode) {
			return Parse(srccode, "isxp");
		}


		/// <summary>
		/// 	Parses source code int BxlToken list
		/// </summary>
		/// <param name="code"> </param>
		/// <param name="filename"> </param>
		/// <param name="options"> </param>
		/// <returns> </returns>
		public BxlToken[] ParseTokens(string code, string filename = "code.bxl",
		                              BxlParserOptions options = BxlParserOptions.None) {
			var tokenizer = new BxlTokenizer();
#if !SQL2008
			if (options.HasFlag(BxlParserOptions.NoElements)) {
				tokenizer = tokenizer.NoElements;
			}
#else
			if (0!=(options &BxlParserOptions.NoElements)) tokenizer = tokenizer.NoElements;
#endif
			return tokenizer.Tokenize(code, filename);
		}


		/// <summary>
		/// 	Parses source into bxl token tree (XPath compatible)
		/// </summary>
		/// <param name="code"> </param>
		/// <param name="filename"> </param>
		/// <param name="options"> </param>
		/// <returns> </returns>
		public BxlTree ParseTree(string code, string filename = "code.bxl", BxlParserOptions options = BxlParserOptions.None) {
			var tokens = ParseTokens(code, filename, options);
			return new BxlTree(tokens);
		}


		/// <summary>
		/// 	Parses source in Line-by-Line mode and returns by-line token arrays (for colorizer tools)
		/// </summary>
		/// <param name="code"> </param>
		/// <param name="filename"> </param>
		/// <param name="options"> </param>
		/// <returns> </returns>
		public BxlToken[][] ParseByLine(string code, string filename, BxlParserOptions options) {
			var result = new List<BxlToken[]>();
			var reader = new StringReader(code);
			string line;
			var state = 1;
			var tokenizer = new BxlTokenizer();
			var linenumber = 0;
			while (null != (line = reader.ReadLine())) {
				tokenizer.SetInitialState(state);
				try {
					result[linenumber] = tokenizer.Tokenize(line, filename);
					state = tokenizer.GetFinishState();
				}
				catch (Exception ex) {
					result[linenumber] = new[] {new BxlToken(BxlTokenType.Error) {Value = ex.ToString()}};
					state = 1;
				}
				linenumber++;
			}
			return result.ToArray();
		}
	}
}