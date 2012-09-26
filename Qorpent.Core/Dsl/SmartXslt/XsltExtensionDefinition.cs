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
// Original file : XsltExtensionDefinition.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Utils {
	/// <summary>
	/// 	Describes extension or parameter from Xslt
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class XsltExtensionDefinition {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="XsltExtensionDefinition" /> class.
		/// </summary>
		/// <param name="type"> The type. </param>
		/// <param name="ns"> The ns. </param>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <remarks>
		/// </remarks>
		private XsltExtensionDefinition(XsltExtenstionType type, string ns, string name, object value) {
			Type = type;
			Namespace = ns;
			Name = name;
			Value = value;
		}

		/// <summary>
		/// 	type of extension
		/// </summary>
		/// <value> The type. </value>
		/// <remarks>
		/// </remarks>
		public XsltExtenstionType Type { get; set; }

		/// <summary>
		/// 	namespace for parameter/extensions "" for parametetrs by default, FullName for Extensions
		/// </summary>
		/// <value> The namespace. </value>
		/// <remarks>
		/// </remarks>
		public string Namespace { get; set; }

		/// <summary>
		/// 	name for parameter/prefix for extensions, must setted up for param, TypeName.lower() for Extensions , filename for import/include
		/// </summary>
		/// <value> The name. </value>
		/// <remarks>
		/// </remarks>
		public string Name { get; set; }

		/// <summary>
		/// 	parameter, extensions value
		/// </summary>
		/// <value> The value. </value>
		/// <remarks>
		/// </remarks>
		public object Value { get; set; }

		/// <summary>
		/// 	Creates the extension.
		/// </summary>
		/// <param name="extension"> The extension. </param>
		/// <param name="name"> The name. </param>
		/// <param name="ns"> The ns. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public static XsltExtensionDefinition Extension(object extension, string name = "", string ns = "") {
			if (null == extension) {
				throw new ArgumentException("extension");
			}
			var _name = name;
			if (string.IsNullOrEmpty(name)) {
				_name = extension.GetType().Name.ToLower();
			}
			var _ns = ns;
			if (string.IsNullOrEmpty(ns)) {
				_ns = extension.GetType().FullName;
			}
			return new XsltExtensionDefinition(XsltExtenstionType.Extension, _ns, _name, extension);
		}

		/// <summary>
		/// 	Creates the parameter.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public static XsltExtensionDefinition Parameter(string name, object value = null) {
			if (string.IsNullOrEmpty(name)) {
				throw new ArgumentException("name is null or empty");
			}
			var _value = value ?? "";
			return new XsltExtensionDefinition(XsltExtenstionType.Parameter, "", name, _value);
		}

		/// <summary>
		/// 	Creates the parameter.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public static XsltExtensionDefinition ParameterSelect(string name, object value = null) {
			if (string.IsNullOrEmpty(name)) {
				throw new ArgumentException("name is null or empty");
			}
			var _value = value ?? "";
			return new XsltExtensionDefinition(XsltExtenstionType.ParameterSelect, "", name, _value);
		}


		/// <summary>
		/// 	Includes the specified filename.
		/// </summary>
		/// <param name="filename"> The filename. </param>
		/// <param name="directvalue"> </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public static XsltExtensionDefinition Include(string filename, string directvalue = null) {
			if (string.IsNullOrEmpty(filename)) {
				throw new ArgumentException("filename is empty or null");
			}
			return new XsltExtensionDefinition(XsltExtenstionType.Include, "", filename, directvalue);
		}

		/// <summary>
		/// 	Imports the specified filename.
		/// </summary>
		/// <param name="filename"> The filename. </param>
		/// <param name="directvalue"> </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public static XsltExtensionDefinition Import(string filename, string directvalue = null) {
			if (string.IsNullOrEmpty(filename)) {
				throw new ArgumentException("filename is empty or null");
			}
			return new XsltExtensionDefinition(XsltExtenstionType.Import, "", filename, directvalue);
		}
	}
}