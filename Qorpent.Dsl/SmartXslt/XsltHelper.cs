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
// Original file : XsltHelper.cs
// Project: Qorpent.Dsl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Xsl;
using Qorpent.Dsl.XmlInclude;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Dsl.SmartXslt {
	/// <summary>
	/// 	some utilities for preparing xslt/xsltarguments
	/// </summary>
	/// <remarks>
	/// </remarks>
	[ContainerComponent(Lifestyle.Transient,ServiceType = typeof(IXsltHelper))]
	public class XsltHelper : ServiceBase, IXsltHelper {
		/// <summary>
		/// Automatically loads source file and applys given transform
		/// </summary>
		public void Process(string source, TextWriter output, string transformFile = null) {
			if(null==output) {
				throw new ArgumentNullException("output");
			}
			var filename = ResolveService<IFileNameResolver>().Resolve(source);
			if(null==filename) {
				throw new DslException("cannot find source file "+source);
			}
			var includereader = ResolveService<IXmlIncludeProcessor>();
			var xml = includereader.Load(filename);
			if(null==transformFile) {
				var transformElement = xml.Element("transform");
				if(transformElement!=null) {
					transformFile = transformElement.ChooseAttr("__code", "code");
				}
			}
			if(null==transformFile) {
				output.Write(xml.ToString());
				return;
			}
			Process(xml,output,transformFile);
		}

		/// <summary>
		/// Automatically loads source file and applys given transform
		/// </summary>
		public void Process(XElement source, TextWriter output, string transformFile) {
			if(null==output) {
				throw new ArgumentNullException("output");
			}
			if(transformFile.IsEmpty()) {
				throw new ArgumentNullException("transformFile");
			}
			var realTransformFileName = ResolveService<IFileNameResolver>().Resolve(transformFile + ".xslt", true,
						                                                              new[]
							                                                              {
								                                                              "usr/dsl", "usr", "mod/dsl", "mod", "sys/dsl",
								                                                              "sys", ""
							                                                              });
			if(null==realTransformFileName) {
				throw new DslException("cannot find XSLT file for "+transformFile+" dsl XSLT lang");
			}

			var transformbase = XElement.Load(realTransformFileName);
			var extensions = ExtractExtensions(source);
			var xslttransform = PrepareXsltStylesheet(transformbase, extensions);
			var args = new XsltArgumentList();
			args = PrepareXsltArguments(args, extensions);
			Process(source, xslttransform, args,extensions, new Uri("file:///"+realTransformFileName),  output);
		}

		/// <summary>
		/// Automatically loads source file and applys given transform
		/// </summary>
		public void Process(XElement source, XElement transform, XsltArgumentList args, IEnumerable<XsltExtensionDefinition> extensions, Uri transformUri, TextWriter output) {
			if (source == null) {
				throw new ArgumentNullException("source");
			}
			if (transform == null) {
				throw new ArgumentNullException("transform");
			}
			if (output == null) {
				throw new ArgumentNullException("output");
			}
			
			var dictionary = new Dictionary<string, string>();
			if(null!=extensions) {
				foreach (XsltExtensionDefinition e in extensions) {
					if ((e.Type == XsltExtenstionType.Import || e.Type == XsltExtenstionType.Include) &&
					    (e.Value != null && e.Value is string && ((string) e.Value).Contains("<xsl:"))) {
						dictionary.Add(e.Name, (string) e.Value);
					}
				}
			}
			var compiled = new XslCompiledTransform();
			compiled.Load(transform.CreateReader(),XsltSettings.TrustedXslt,new FileNameResolverXmlUrlResolver(transformUri,ResolveService<IFileNameResolver>(),dictionary));

			compiled.Transform(source.CreateReader(),args,output);

		}


		/// <summary>
		/// </summary>
		public static readonly string ImportElementName = "{" + QorpentConst.Xml.XsltNameSpace + "}import";

		/// <summary>
		/// </summary>
		public static readonly string IncludeElementName = "{" + QorpentConst.Xml.XsltNameSpace + "}include";

		/// <summary>
		/// </summary>
		public static readonly string ParamElementName = "{" + QorpentConst.Xml.XsltNameSpace + "}param";

		/// <summary>
		/// 	embeds advanced includes, imports, parameters and extensions into target xslt stylesheet with control of duplicates
		/// </summary>
		/// <param name="source"> The source. </param>
		/// <param name="extensions"> The extensions. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public XElement PrepareXsltStylesheet(XElement source, IEnumerable<XsltExtensionDefinition> extensions) {
			foreach (var extension in extensions) {
				switch (extension.Type) {
					case XsltExtenstionType.Parameter:
						EmbedParameter(source, extension, false);
						break;
					case XsltExtenstionType.ParameterSelect:
						EmbedParameter(source, extension, true);
						break;
					case XsltExtenstionType.Extension:
						EmbedExtension(source, extension);
						break;
					case XsltExtenstionType.Import:
						EmbedImport(source, extension);
						break;
					case XsltExtenstionType.Include:
						EmbedInclude(source, extension);
						break;
				}
			}
			return source;
		}

		/// <summary>
		/// 	Embeds the include.
		/// </summary>
		/// <param name="source"> The source. </param>
		/// <param name="extension"> The extension. </param>
		/// <remarks>
		/// </remarks>
		private void EmbedInclude(XElement source, XsltExtensionDefinition extension) {
			var existed = source.Elements(IncludeElementName).FirstOrDefault(x => x.Attr("href") == extension.Name);
			if (null != existed) {
				return; //element already added
			}

			var lastimport = source.Elements(ImportElementName).LastOrDefault();
			var lastinclude = source.Elements(IncludeElementName).LastOrDefault();

			var element = new XElement(IncludeElementName, new XAttribute("href", extension.Name));

			if (lastinclude != null) {
				lastinclude.AddAfterSelf(element);
			}
			else if (lastimport != null) {
				lastimport.AddAfterSelf(element);
			}
			else {
				source.AddFirst(element);
			}
		}


		/// <summary>
		/// 	Embeds the import.
		/// </summary>
		/// <param name="source"> The source. </param>
		/// <param name="extension"> The extension. </param>
		/// <remarks>
		/// </remarks>
		private void EmbedImport(XElement source, XsltExtensionDefinition extension) {
			var existed = source.Elements(ImportElementName).FirstOrDefault(x => x.Attr("href") == extension.Name);
			if (null != existed) {
				return; //element already added
			}

			var lastimport = source.Elements(ImportElementName).LastOrDefault();


			var element = new XElement(ImportElementName, new XAttribute("href", extension.Name));

			if (lastimport != null) {
				lastimport.AddAfterSelf(element);
			}
			else {
				source.AddFirst(element);
			}
		}

		/// <summary>
		/// 	Embeds the extension.
		/// </summary>
		/// <param name="source"> The source. </param>
		/// <param name="extension"> The extension. </param>
		/// <remarks>
		/// </remarks>
		private void EmbedExtension(XElement source, XsltExtensionDefinition extension) {
			var nsattrname = "{" + XNamespace.Xmlns + "}" + extension.Name;
			var existed = source.Attribute(nsattrname);
			if (null != existed) {
				if (existed.Value != extension.Namespace) {
					throw new QorpentException("try to override existed namespace");
				}
				return;
			}
			source.SetAttributeValue(nsattrname, extension.Namespace);
		}

		/// <summary>
		/// 	Embeds the parameter.
		/// </summary>
		/// <param name="source"> The source. </param>
		/// <param name="extension"> The extension. </param>
		/// <param name="asselect"> if set to <c>true</c> [asselect]. </param>
		/// <remarks>
		/// </remarks>
		private void EmbedParameter(XElement source, XsltExtensionDefinition extension, bool asselect) {
			var existed = source.Elements(ParamElementName).FirstOrDefault(x => x.Attr("name") == extension.Name);
			if (null != existed) {
				existed.Remove(); //here we must remove old declaration
			}

			var lastimport = source.Elements(ImportElementName).LastOrDefault();
			var lastinclude = source.Elements(IncludeElementName).LastOrDefault();
			var lastparam = source.Elements(ParamElementName).LastOrDefault();


			var element = new XElement(ParamElementName, new XAttribute("name", extension.Name));
			if (asselect) {
				element.Add(new XAttribute("select", extension.Value));
			}
			else {
				element.SetValue(extension.Value);
			}

			if (lastparam != null) {
				lastparam.AddAfterSelf(element);
			}
			else if (lastinclude != null) {
				lastinclude.AddAfterSelf(element);
			}
			else if (lastimport != null) {
				lastimport.AddAfterSelf(element);
			}
			else {
				source.AddFirst(element);
			}
		}

		/// <summary>
		/// 	embeds extensions int XsltArgs - params and extensions will be used
		/// </summary>
		/// <param name="arguments"> The arguments. </param>
		/// <param name="extensions"> The extensions. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public XsltArgumentList PrepareXsltArguments(XsltArgumentList arguments,
		                                             IEnumerable<XsltExtensionDefinition> extensions) {
			foreach (var extension in extensions) {
				switch (extension.Type) {
					case XsltExtenstionType.Parameter:
						arguments.AddParam(extension.Name, "", extension.Value);
						break;
					case XsltExtenstionType.Extension:
						arguments.AddExtensionObject(extension.Namespace, extension.Value);
						break;
				}
			}
			return arguments;
		}

		/// <summary>
		/// 	extract extensions from source xml with special namespace http://qorpent/xslt/extensions
		/// </summary>
		/// <param name="source"> The source. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public IEnumerable<XsltExtensionDefinition> ExtractExtensions(XElement source) {
			foreach (var ext in source.Elements().Where(x => x.Name.NamespaceName == QorpentConst.Xml.SmartXsltNamespace)) {
				if (QorpentConst.Xml.SmartXsltIncludeElementName == ext.Name) {
					yield return XsltExtensionDefinition.Include(ext.Describe().Code);
				}
				else if (QorpentConst.Xml.SmartXsltImportElementName == ext.Name) {
					yield return XsltExtensionDefinition.Import(ext.Describe().Code);
				}
				else if (QorpentConst.Xml.SmartXsltParamElementName == ext.Name) {
					if (null != ext.Attribute("select")) {
						yield return XsltExtensionDefinition.ParameterSelect(ext.Describe().Name, ext.Attr("select"));
					}
					else {
						yield return XsltExtensionDefinition.Parameter(ext.Describe().Code, ext.Describe().Name);
					}
				}
				else if (QorpentConst.Xml.SmartXsltExtensionElementName == ext.Name) {
					var name = ext.Describe().Code;
					var ns = ext.ChooseAttr("name", "__name");
					var typename = ext.Value;
					if (typename.IsEmpty()) {
						throw new QorpentException("type not defined for extension");
					}
					var type = Type.GetType(typename);
					var e = Activator.CreateInstance(type);
					yield return XsltExtensionDefinition.Extension(e, name, ns);
				}
			}
		}
	}
}