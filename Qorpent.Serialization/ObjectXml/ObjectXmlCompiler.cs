using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.ObjectXml {
	/// <summary>
	/// Компилятор ObjectXml по умолчанию
	/// </summary>
	public class ObjectXmlCompiler : ObjectXmlCompilerBase {
		/// <summary>
		/// Текущий контекстный индекс
		/// </summary>
		protected ObjectXmlCompilerIndex _currentBuildIndex;

		/// <summary>
		/// Перекрыть для создания индексатора
		/// </summary>
		/// <param name="sources"></param>
		/// <returns></returns>
		protected override ObjectXmlCompilerIndex BuildIndex(IEnumerable<XElement> sources) {
			_currentBuildIndex = new ObjectXmlCompilerIndex();
			var baseindex = IndexizeRawClasses(sources).ToArray();
			_currentBuildIndex.RawClasses =baseindex.ToDictionary(_=>_.FullName,_=>_);
			return _currentBuildIndex;
		}

		private IEnumerable<XmlObjectClassDefinition> IndexizeRawClasses(IEnumerable<XElement> sources)
		{
			foreach (var src in sources) {
				foreach (var e in IndexizeRawClasses(src,"")) {
					yield return e;
				}
			}
		}

		private IEnumerable<XmlObjectClassDefinition> IndexizeRawClasses(XElement src, string ns) {
			foreach (var e in src.Elements()) {
				if (e.Name.LocalName == "namespace") {
					if (string.IsNullOrWhiteSpace(ns)) {
						ns = e.Attr("code");
					}
					else {
						ns = ns + "." + e.Attr("code");
					}
					foreach (var e_ in IndexizeRawClasses(e, ns)) {
						yield return e_;
					}
				}
				else {
					var def = new XmlObjectClassDefinition();
					def.Source = e;
					def.Name = e.Attr("code");
					def.Namespace = ns;
					
					if (null != e.Attribute("abstract") || e.Attr("name")=="abstract") {
						def.Abstract = true;
					}
					if (e.Name.LocalName != "class") {
						def.Orphaned = true;
						def.DefaultImportCode = e.Name.LocalName;
					}
					else {
						def.Orphaned = false;
						def.ExplicitClass = true;
					}
					foreach (var i in e.Elements("import")) {
						var import = new XmlObjectImportDescription {Condition = i.Value, TargetCode = i.Attr("code")};
						def.Imports.Add(import);
					}
					foreach (var i in e.Elements("element")) {
						var merge = new XmlObjectMergeDefinition();
						merge.Name = i.Attr("code");
						merge.Type = XmlObjectMergeType.Define;
						if (i.Attribute("override") != null) {
							merge.Type = XmlObjectMergeType.Override;
							merge.TargetName = i.Attr("override");
						}else if (null != i.Attribute("extend")) {
							merge.Type = XmlObjectMergeType.Extension;
							merge.TargetName = i.Attr("extend");
						}
						def.MergeDefs.Add(merge);

					}

					yield return def;
				}
			}
		}

		/// <summary>
		/// Перекрыть для создания линковщика
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		protected override void Link(IEnumerable<XElement> sources, ObjectXmlCompilerIndex index) {
			_currentBuildIndex = index;
			var _classes = CollectFinalClasses(sources, index);
			foreach (var cls in _classes) {
				BuildClass(cls, index);
			}
		}
		XmlInterpolation xi = new XmlInterpolation();
		private void BuildClass(XmlObjectClassDefinition cls, ObjectXmlCompilerIndex index) {
			cls.Compiled = cls.Source;
			cls.ParamIndex = cls.BuildParameterOverrideXml();
			cls.SrcParamIndex = new XElement(cls.ParamIndex);
			if (GetConfig().UseInterpolation) {
				cls.ParamIndex = xi.Interpolate(cls.ParamIndex);
				var lastelement = cls.ParamIndex.Descendants().Last();
				//force cross-attribute resulution
				for (var i = 0; i < 3; i++) {
					if (lastelement.Attributes().Any(_ => _.Value.Contains("${"))) {
						xi.Interpolate(cls.ParamIndex);
					}
					else {
						break;
					}
				}
			}
			foreach (var a in cls.Compiled.Attributes().ToArray()) {
				a.Remove();
			}
			foreach (var e in cls.ParamIndex.DescendantsAndSelf()) {
				foreach (var a in e.Attributes()) {
					cls.Compiled.SetAttributeValue(a.Name, a.Value);
				}
			}
		}

		private IEnumerable<XmlObjectClassDefinition> CollectFinalClasses(IEnumerable<XElement> sources, ObjectXmlCompilerIndex index) {
			BindOrphansAndSetupFullImportName(index);
			ResolveImports(index);
			return index.Working;
		}

		private void ResolveImports(ObjectXmlCompilerIndex index) {
			foreach (var w in index.Working) {
				foreach (var i in w.Imports) {
					i.Orphaned = true;
					var import = ResolveClass(index, i.TargetCode, w.Namespace);
					if (null != import) {
						i.Orphaned = false;
						i.Target = import;
					}
				}
			}
		}

		private static void BindOrphansAndSetupFullImportName(ObjectXmlCompilerIndex index) {
			var _initiallyorphaned = index.RawClasses.Values.Where(_ => _.Orphaned);
			foreach (var o in _initiallyorphaned) {
				var code = o.DefaultImportCode;
				var ns = o.Namespace;
				var import = ResolveClass(index, code, ns);
				if (import != null) {
					o.Orphaned = false;
					o.DefaultImport = import;
				}
			}

			index.Orphans = index.RawClasses.Values.Where(_ => _.DetectIfIsOrphaned()).ToList();

			index.Abstracts = index.RawClasses.Values.Where(_ => _.Abstract && !_.DetectIfIsOrphaned()).ToList();

			index.Working = index.RawClasses.Values.Where(_ => !_.Abstract && !_.DetectIfIsOrphaned()).ToList();
		}

		private static XmlObjectClassDefinition ResolveClass(ObjectXmlCompilerIndex index, string code, string ns) {
			XmlObjectClassDefinition import = null;
			if (!string.IsNullOrWhiteSpace(code)) {
				if (code.Contains('.')) {
					if (index.RawClasses.ContainsKey(code)) {
						import = index.RawClasses[code];
					}
				}
				else {
					var probe = ns + "." + code;
					if (index.RawClasses.ContainsKey(probe)) {
						import = index.RawClasses[probe];
					}
					else {
						probe = code;
						if (index.RawClasses.ContainsKey(probe)) {
							import = index.RawClasses[probe];
						}
					}
				}
			}
			return import;
		}
	}
}