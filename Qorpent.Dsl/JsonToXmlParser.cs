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
// PROJECT ORIGIN: Qorpent.Dsl/JsonToXmlParser.cs
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Qorpent.Dsl.Json;
using Qorpent.IoC;

namespace Qorpent.Dsl {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	[ContainerComponent(Lifestyle.Transient, Name = "json.xml.parser")]
	public class JsonToXmlParser : ISpecialXmlParser {
		/// <summary>
		/// Конвертирует исходный текст в Json используя штатный пармер
		/// </summary>
		/// <param name="srccode"></param>
		/// <returns></returns>
		public XElement Parse(string srccode) {
			return new JsonParser().Parse(srccode).WriteToXml();
		}
		/// <summary>
		/// 
		/// </summary>
		public class JsonParserException:Exception {
			/// <summary>
			/// 
			/// </summary>
			/// <param name="message"></param>
			public JsonParserException(string message):base(message){}
		}
	}
}