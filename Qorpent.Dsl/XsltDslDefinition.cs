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
// PROJECT ORIGIN: Qorpent.Dsl/XsltDslDefinition.cs
#endregion
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.Dsl.SmartXslt;
using Qorpent.IO;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Dsl {
	/// <summary>
	/// 	Definition of XSLT bound dsl
	/// </summary>
	public class XsltDslDefinition {
		/// <summary>
		/// 	File to be loaded
		/// </summary>
		public string MainXsltFileName { get; set; }

		/// <summary>
		/// 	Extensions to be used
		/// </summary>
		public IEnumerable<XsltExtensionDefinition> Extensions { get; set; }

		/// <summary>
		/// 	Creates the transform.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public SmartXsltTransform CreateTransform() {
			var result = new SmartXsltTransform();
			result.Resolver = resolver;
			result.Setup(MainXsltFileName, MainXsltFileName, Extensions);
			return result;
		}

		/// <summary>
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="resolvedDslDirectory"> </param>
		/// <param name="resolver"> </param>
		/// <param name="extensions"> </param>
		/// <exception cref="DslException"></exception>
		public void Load(string name, string resolvedDslDirectory = null, IFileNameResolver resolver = null,
		                 IEnumerable<XsltExtensionDefinition> extensions = null) {
			this.resolver = resolver;
			Extensions = new[]
				{
					XsltExtensionDefinition.Extension(new DslCSharpXsltExtensions(), "cs", QorpentConst.Xml.CSharpDslExtensionNameSpace)
					,
					XsltExtensionDefinition.Extension(new DslSqlXsltExtensions(), "sql", QorpentConst.Xml.SqlDslExtensionNameSpace)
				};
			var _resolver = resolver ?? Application.Current.Files;
			string xsltfile = null;

			if (resolvedDslDirectory.IsEmpty()) {
				xsltfile = GetXsltFileWithFileNameResolver(name, _resolver);
			}
			else {
				xsltfile = GetXsltFileFromDirectlyDefinedDirectory(name, resolvedDslDirectory);
			}
			MainXsltFileName = xsltfile;
			extensions = extensions ?? new XsltExtensionDefinition[] {};
			Extensions = Extensions
				.Union(new XsltHelper().ExtractExtensions(XElement.Load(xsltfile)))
				.Union(extensions)
				.ToList();
		}

		private string GetXsltFileFromDirectlyDefinedDirectory(string name, string resolvedDslDirectory) {
			string definitionfile;
			string xsltfile;
			var root = Path.GetFullPath(resolvedDslDirectory);
			definitionfile = Path.Combine(root, name + "bxl");
			if (!File.Exists(definitionfile)) {
				//no certain Dsl definition file, try to load XSLT directly
				xsltfile = Path.Combine(root, name, "default.xslt");

				if (!File.Exists(xsltfile)) {
					xsltfile = Path.Combine(root, name, "default.bxl");
				}
				if (!File.Exists(xsltfile)) {
					xsltfile = Path.Combine(root, "default.xslt");
				}
				if (!File.Exists(xsltfile)) {
					xsltfile = Path.Combine(root, "default.bxl");
				}
				if (null == xsltfile) {
					throw new DslException(new DslMessage
						{
							ErrorLevel = ErrorLevel.Error,
							Message =
								"cannot find dsl with name " + name + " in root " + root
						});
				}
			}
			else {
				var defxml = XElement.Load(definitionfile);
				xsltfile = defxml.Element("xslt").Describe().Code;
				Extensions = Extensions.Union(new XsltHelper().ExtractExtensions(defxml));
			}
			return xsltfile;
		}

		private string GetXsltFileWithFileNameResolver(string name, IFileNameResolver _resolver) {
			string definitionfile;
			string xsltfile;
			definitionfile = _resolver.Resolve("dsl/" + name + ".bxl");
			if (null == definitionfile) {
				//no certain Dsl definition file, try to load XSLT directly
				xsltfile = _resolver.Resolve("dsl/" + name + "/default.xslt");
				if (null == xsltfile) {
					xsltfile = _resolver.Resolve("dsl/" + name + "/default.bxl");
				}
				if (null == xsltfile) {
					throw new DslException(new DslMessage
						{
							ErrorLevel = ErrorLevel.Error,
							Message =
								"cannot find dsl with name " + name + " no dsl/" + name + ".bxl or dsl/" + name +
								"/default.xslt files found"
						});
				}
			}
			else {
				var defxml = XElement.Load(definitionfile);
				xsltfile = defxml.Element("xslt").Describe().Code;
				Extensions = Extensions.Union(new XsltHelper().ExtractExtensions(defxml));
			}
			return xsltfile;
		}

		private IFileNameResolver resolver;
	}
}