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
// PROJECT ORIGIN: Qorpent.Dsl/XmlIncludeProcessor.cs
#endregion

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.Bxl;
using Qorpent.Dsl.XmlInclude;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Qxi {
	/// <summary>
	/// 	Uses file resolver from container or application or directly setted to perform XML include logic
	/// </summary>
	/// <remarks>
	/// </remarks>
	[ContainerComponent(Lifestyle.Transient)]
	public class XmlIncludeProcessor : ServiceBase, IXmlIncludeProcessor {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="XmlIncludeProcessor" /> class. - for tests
		/// </summary>
		/// <param name="resolver"> The resolver. </param>
		/// <remarks>
		/// </remarks>
		public XmlIncludeProcessor(IFileNameResolver resolver) : this() {
			_resolver = resolver;
		}

		/// <summary>
		/// 	Initializes a new instance of the <see cref="XmlIncludeProcessor" /> class. - for recursive includes
		/// </summary>
		/// <param name="parent"> The parent. </param>
		/// <remarks>
		/// </remarks>
		protected internal XmlIncludeProcessor(XmlIncludeProcessor parent) {
			_parent = parent;
			DirectImports = _parent.DirectImports;
		}

		/// <summary>
		/// 	Initializes a new instance of the <see cref="XmlIncludeProcessor" /> class. - for default/ioc
		/// </summary>
		/// <remarks>
		/// </remarks>
		public XmlIncludeProcessor() {
			DirectImports = new Dictionary<string, XElement>();
		}

		/// <summary>
		/// 	Direcly defined imports (will be resolved with special schema direct//)
		/// </summary>
		/// <value> The direct imports. </value>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, XElement> DirectImports { get; protected set; }

		/// <summary>
		/// 	Gets the resolver.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected IFileNameResolver Resolver {
			get {
				if (null != _resolver) {
					return _resolver;
				}
				if (null != _parent) {
					return _resolver = _parent.Resolver;
				}
				return _resolver = ResolveService<IFileNameResolver>();
			}
			set { _resolver = value; }
		}

		/// <summary>
		/// 	Gets the resolver.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IBxlParser Bxl {
			get {
				if (null != _bxl) {
					return _bxl;
				}
				if (null != _parent) {
					return _bxl = _parent.Bxl;
				}
				return ResolveService<IBxlParser>();
			}
			set { _bxl = value; }
		}


		/// <summary>
		/// 	reads file form disk (both XML or BXL) and processed includes
		/// </summary>
		/// <param name="path"> </param>
		/// <param name="applyDelayed"> </param>
		/// <param name="options"> </param>
		/// <returns> </returns>
		public XElement Load(string path, bool applyDelayed = true, BxlParserOptions options = BxlParserOptions.None) {
			var ext = Path.GetExtension(path) ?? "bxl";
			var fullpath = Resolver.Resolve(path);
			XElement result;
			if (ext.EndsWith("bxl") || ext.EndsWith("hql") /*hack to match old zeta usage */) {
				var bxl = File.ReadAllText(fullpath);
				result = Bxl.Parse(bxl, fullpath, options);
			}
			else {
				result = XElement.Load(fullpath);
			}
			return Include(result, fullpath, applyDelayed, options);
		}

		/// <summary>
		/// 	Includes the specified document.
		/// </summary>
		/// <param name="document"> The document. </param>
		/// <param name="codebase"> The codebase. </param>
		/// <param name="applyDelayed"> if set to <c>true</c> [with late includes]. </param>
		/// <param name="options"> </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public XElement Include(XElement document, string codebase, bool applyDelayed = true,
		                        BxlParserOptions options = BxlParserOptions.None) {
			var processable = GetProcessableElements(document, applyDelayed).ToArray();
			while (processable.Any()) {
				ApplyIncludes(document, codebase, options, processable);
				ApplyImports(document, codebase, options, processable);
				ApplySafeImports(document, codebase, options, processable);
				ApplyReplaces(document, codebase, options, processable);
				ApplyTemplates(document, codebase, options, processable);
				processable = GetProcessableElements(document, applyDelayed).ToArray();
			}
			return document;
		}


		/// <summary>
		/// 	Примеяет элементы IMPORT
		/// </summary>
		/// <param name="document"> </param>
		/// <param name="codebase"> </param>
		/// <param name="options"> </param>
		/// <param name="processable"> </param>
		protected virtual void ApplyImports(XElement document, string codebase, BxlParserOptions options,
		                                    IEnumerable<XElement> processable) {
			foreach (var i in processable.Where(x => x.Name == QorpentConst.Xml.XmlIncludeImportElementName)) {
				var importElement = GetImportSource(document, i, codebase, options);
				if (null == importElement) {
					i.Remove();
				}
				else {
					foreach (var xAttribute in importElement.Attributes()) {
						if (NoImportedAttributes.Any(x => x == xAttribute.Name.LocalName)) {
							continue;
						}
						if(null!=i.Parent) {
							i.Parent.SetAttributeValue(xAttribute.Name, xAttribute.Value);
						}
					}
					var parent = i.Parent;
					i.ReplaceWith(importElement.Elements());
					if (!string.IsNullOrWhiteSpace(i.Attr("replace"))) {
						parent.ReplaceWith(ApplyReplacesToImportedData(i, new[] {parent}));
					}
				}
			}
		}

		/// <summary>
		/// 	Примеяет элементы IMPORT
		/// </summary>
		/// <param name="document"> </param>
		/// <param name="codebase"> </param>
		/// <param name="options"> </param>
		/// <param name="processable"> </param>
		protected virtual void ApplySafeImports(XElement document, string codebase, BxlParserOptions options,
		                                        IEnumerable<XElement> processable) {
			foreach (var i in processable.Where(x => x.Name == QorpentConst.Xml.XmlIncludeSafeImportElementName)) {
				var importElement = GetImportSource(document, i, codebase, options);
				if (null == importElement) {
					i.Remove();
				}
				else {
					foreach (var xAttribute in importElement.Attributes()) {
						if (NoImportedAttributes.Any(x => x == xAttribute.Name.LocalName)) {
							continue;
						}

						Debug.Assert(i.Parent != null, "i.Parent != null");
						if (null == i.Parent.Attribute(xAttribute.Name)) {
							i.Parent.SetAttributeValue(xAttribute.Name, xAttribute.Value);
						}
					}
					var parent = i.Parent;
					i.ReplaceWith(importElement.Elements());
					if (!string.IsNullOrWhiteSpace(i.Attr("replace")))
					{
						parent.ReplaceWith(ApplyReplacesToImportedData(i, new[] { parent }));
					}
				}
			}
		}

		/// <summary>
		/// 	Применяет элементы INCLUDE
		/// </summary>
		/// <param name="document"> </param>
		/// <param name="codebase"> </param>
		/// <param name="options"> </param>
		/// <param name="processable"> </param>
		protected virtual void ApplyIncludes(XElement document, string codebase, BxlParserOptions options,
		                                     IEnumerable<XElement> processable) {
			foreach (var i in processable.Where(x => x.Name == QorpentConst.Xml.XmlIncludeIncludeElementName)) {
				var includeElements = GetIncludeSource(document, i, codebase, options);
                includeElements = ApplyReplacesToImportedData(i, includeElements);
			    i.ReplaceWith(includeElements);
			}
		}

	    private static IEnumerable<XElement> ApplyReplacesToImportedData(XElement i, IEnumerable<XElement> includeElements) {
	        var replacer = i.Attr("replace");
	        if (!string.IsNullOrWhiteSpace(replacer)) {
	            includeElements = includeElements.Select(_ => new XElement(_)).ToArray();
	            var replaces = replacer.SmartSplit().Select(_ => new {from = _.Split('=')[0], to = _.Split('=')[1]}).ToArray();
	            foreach (var ie in includeElements) {
	                var texts = ie.DescendantNodes().OfType<XText>().ToArray();
		            var attributes = ie.DescendantsAndSelf().SelectMany(_ => _.Attributes()).ToArray();
	                foreach (var a in attributes) {
	                    foreach (var r in replaces) {
	                        a.Value = a.Value.Replace(r.@from, r.to);
	                    }
	                }
	                foreach (var a in texts) {
	                    foreach (var r in replaces) {
	                        a.Value = a.Value.Replace(r.@from, r.to);
	                    }
	                }
	            }
	        }
	        return includeElements;
	    }


	    /// <summary>
		/// 	Применяет элементы TEMPLATE
		/// </summary>
		/// <param name="document"> </param>
		/// <param name="codebase"> </param>
		/// <param name="options"> </param>
		/// <param name="processable"> </param>
		protected virtual void ApplyTemplates(XElement document, string codebase, BxlParserOptions options,
		                                      IEnumerable<XElement> processable) {
			IDictionary<string, XElement> templates = (from x in processable
			                                           where x.Name == QorpentConst.Xml.XmlIncludeTemplateElementName
			                                           select
				                                           new
					                                           {
						                                           code = x.ChooseAttr("__code", "code"),
						                                           template = x,
					                                           })
				.ToDictionary(x => x.code, x => x.template);
			foreach (var templateElement in templates.Values.ToArray()) {
				templateElement.Remove(); //убираем шаблоны из исходников
			}

			foreach (var t in templates) {
				var code = t.Key;
				var template = t.Value.Elements().First(); //само содержимое шаблона
				var targets = document.DescendantsAndSelf(code);
				foreach (var target in targets.ToArray()) {
					var result = new XElement(template);
					foreach (var attr in target.Attributes()) {
						result.SetAttributeValue(attr.Name, attr.Value);
					}
					if (result.SelfValue().IsEmpty()) {
						result.Value = target.SelfValue();
					}
					foreach (var e in target.Elements()) {
						result.Add(e);
					}
					target.ReplaceWith(result);
				}
			}
		}


		/// <summary>
		/// 	Применяет элементы REPLACE
		/// </summary>
		/// <param name="document"> </param>
		/// <param name="codebase"> </param>
		/// <param name="options"> </param>
		/// <param name="processable"> </param>
		protected virtual void ApplyReplaces(XElement document, string codebase, BxlParserOptions options,
		                                     IEnumerable<XElement> processable) {
			var replacers = new List<ReplaceDescriptor>();
			foreach (
				var replaceElement in processable.Where(x => x.Name == QorpentConst.Xml.XmlIncludeReplaceElementName).ToArray()
				) {
				var replacer = new ReplaceDescriptor
					{Pattern = replaceElement.ChooseAttr("__code", "code"), Replacer = replaceElement.SelfValue()};
				replacers.Add(replacer);
				replaceElement.Remove();
			}
			ApplyReplaces(document, replacers);
		}

		private void ApplyReplaces(XElement element, List<ReplaceDescriptor> replacers) {
			if (element.Name.Namespace == QorpentConst.Xml.XmlIncludeNamespace) {
				return;
			}
			foreach (var a in element.Attributes()) {
				if (a.Name.Namespace == XNamespace.Xml) {
					continue;
				}
				if (a.Name.Namespace == XNamespace.Xmlns) {
					continue;
				}
				a.Value = ApplyReplaces(a.Value, replacers);
			}
			foreach (var t in element.Nodes().OfType<XText>()) {
				t.Value = ApplyReplaces(t.Value, replacers);
			}
			foreach (var xElement in element.Elements()) {
				ApplyReplaces(xElement, replacers);
			}
		}

		private string ApplyReplaces(string str, IEnumerable<ReplaceDescriptor> replacers) {
			return replacers.Aggregate(str, (current, replaceDescriptor) => replaceDescriptor.Execute(current));
		}

		/// <summary>
		/// 	Gets the import.
		/// </summary>
		/// <param name="document"> </param>
		/// <param name="xElement"> The x element. </param>
		/// <param name="codebase"> The codebase. </param>
		/// <param name="options"> </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private XElement GetImportSource(XElement document, XElement xElement, string codebase, BxlParserOptions options) {
			var importDocument = GetImportedDocument(document, xElement, codebase, options);
			if (null == importDocument) {
				return null;
			}
			var single = importDocument;
			var query = xElement.Attr(QorpentConst.Xml.XmlIncludeQueryAttributeName);
			if (query.IsEmpty()) {
				query = xElement.Value;
			}
			if (query.IsNotEmpty()) {
				single = importDocument.XPathSelectElement(query);
			}
			return single;
		}

		/// <summary>
		/// 	Gets the imported document.
		/// </summary>
		/// <param name="document"> </param>
		/// <param name="xElement"> The x element. </param>
		/// <param name="codebase"> The codebase. </param>
		/// <param name="options"> </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		protected virtual XElement GetImportedDocument(XElement document, XElement xElement, string codebase,
		                                               BxlParserOptions options) {
			var href = xElement.ChooseAttr(QorpentConst.Xml.XmlIncludeHrefAttributeName.ToString(), "__code", "code");
			if (href.IsEmpty()) {
				throw new QorpentException("href is empty");
			}
			string docpath;
			if (href == "self") {
				return new XElement(document);
			}
			if (href.StartsWith("direct//")) {
				var directcode = href.Substring(8);
				if (!DirectImports.ContainsKey(directcode)) {
					return null;
				}
				return DirectImports[directcode];
			}
			if (href.StartsWith("~")) {
				docpath = Resolver.Resolve(href);
			}
			else if (Path.IsPathRooted(href)) {
				docpath = href;
			}
			else {
				var relative = href;
				var resolvedcodebase = Resolver.Resolve(codebase);
				var dir = Path.GetDirectoryName(resolvedcodebase);
				while (relative.StartsWith("..")) {
					dir = Path.GetDirectoryName(dir);
					relative = relative.Substring(3);
				}
				Debug.Assert(dir != null, "dir != null");
				docpath = Path.Combine(dir, relative);
				if (!File.Exists(docpath)) {
					if (!href.Contains("..")) {
						docpath = Resolver.Resolve(new[] {href}, true, FileNameResolverExtensions.DEFAULT_USRFILE_RESOLVE_PROBE_PATHS);
					}
				}
			}
			if (!File.Exists(docpath)) {
				if (xElement.Attr("optional", "false").ToBool()) {
					return null;
				}
				throw new QorpentException("cannot find " + docpath + " resolved from href: " + href + " on codebase: " + codebase);
			}
			var result = docpath.EndsWith(".xml") ? XElement.Load(docpath) : Bxl.Parse(File.ReadAllText(docpath), docpath);

			//recursively call to includes (not delayed)
			result = new XmlIncludeProcessor(this).Include(result, docpath, false, options);
			return result;
		}

		/// <summary>
		/// 	Gets the include.
		/// </summary>
		/// <param name="document"> </param>
		/// <param name="xElement"> The x element. </param>
		/// <param name="codebase"> The codebase. </param>
		/// <param name="options"> </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private IEnumerable<XElement> GetIncludeSource(XElement document, XElement xElement, string codebase,
		                                         BxlParserOptions options) {
			var importDocument = GetImportedDocument(document, xElement, codebase, options);
			if (null == importDocument) {
				return new XElement[] {};
			}
			var query = xElement.Attr(QorpentConst.Xml.XmlIncludeQueryAttributeName);
			if (query.IsEmpty()) {
				query = xElement.Value;
			}
			if (query.IsNotEmpty()) {
				return importDocument.XPathSelectElements(query);
			}
			if (importDocument.Name.LocalName == "root") {
//bxl adaptation
				return importDocument.Elements();
			}
			return new[] {importDocument};
		}

		/// <summary>
		/// 	Gets the processable.
		/// </summary>
		/// <param name="document"> The document. </param>
		/// <param name="applyDelayed"> if set to <c>true</c> [apply delayed]. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private IEnumerable<XElement> GetProcessableElements(XElement document, bool applyDelayed) {
			var normalIi = new List<XElement>(); // список элементов импорта/инклуда обычного порядка
			normalIi.AddRange(
				document.DescendantsAndSelf().Where(x => IsProcessAble(x, applyDelayed, false)).Reverse());
			var selfIi = new List<XElement>(); // список элементов импорта/инклуда типа self
			if (applyDelayed) {
				normalIi.AddRange(
					document.DescendantsAndSelf().Where(x => IsProcessAble(x, true, true)).Reverse());
			}
			var templates = new List<XElement>();
			if (applyDelayed) {
				templates.AddRange(
					document.DescendantsAndSelf().Where(x => IsProcessAble(x, true, false, "template,replace")).Reverse());
			}
			return normalIi.Union(selfIi).Union(templates).ToArray();
		}

		/// <summary>
		/// 	Determines whether [is process able] [the specified x element].
		/// </summary>
		/// <param name="xElement"> The x element. </param>
		/// <param name="applyDelayed"> if set to <c>true</c> [apply delayed]. </param>
		/// <param name="selfs"> true - поиск ведется среди qxi::import/include self </param>
		/// <param name="elementNameFilter"> ростой строчный фильтр имен элментов </param>
		/// <returns> <c>true</c> if [is process able] [the specified x element]; otherwise, <c>false</c> . </returns>
		/// <remarks>
		/// </remarks>
		protected virtual bool IsProcessAble(XElement xElement, bool applyDelayed, bool selfs,
		                                     string elementNameFilter = "include,import,safeimport") {
			if (!applyDelayed && selfs) {
				return false;
			}
			if (xElement.Name.Namespace != QorpentConst.Xml.XmlIncludeNamespace) {
				return false;
			}
			if ((selfs || !applyDelayed) &&
			    (xElement.Name == QorpentConst.Xml.XmlIncludeTemplateElementName ||
			     xElement.Name == QorpentConst.Xml.XmlIncludeReplaceElementName)) {
				return false;
			}
			if (selfs && xElement.ChooseAttr("__code", "code") != "self") {
				return false;
			}
			if (!selfs && xElement.ChooseAttr("__code", "code") == "self") {
				return false;
			}
			if (elementNameFilter.IsNotEmpty() && !elementNameFilter.Contains(xElement.Name.LocalName)) {
				return false;
			}
			if (applyDelayed) {
				return true;
			}
			return !xElement.ChooseAttr(QorpentConst.Xml.XmlIncludeDelayAttributeName.ToString(), "__name", "name").ToBool();
		}

		/// <summary>
		/// </summary>
		private readonly XmlIncludeProcessor _parent;

		/// <summary>
		/// </summary>
		public string[] NoImportedAttributes = new[] {"__id", "id", "__code", "code", "__name", "name"};

		private IBxlParser _bxl;

		/// <summary>
		/// </summary>
		private IFileNameResolver _resolver;
	}
}