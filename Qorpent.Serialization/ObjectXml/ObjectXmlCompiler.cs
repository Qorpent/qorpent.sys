using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.Config;
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

		private void BuildClass(XmlObjectClassDefinition cls, ObjectXmlCompilerIndex index) {
			InitializeBuildIndexes(cls);
			InterpolateFields(cls);
			BindParametersToCompiledClass(cls);
			InterpolateElements(cls);
			CleanupPrivateMembers(cls);

		}

		private void CleanupPrivateMembers(XmlObjectClassDefinition cls) {
			foreach (
				var a in ((IEnumerable) cls.Compiled.XPathEvaluate(".//@*[starts-with(local-name(),'_')]")).OfType<XAttribute>().ToArray()) {
				if (a.Name.LocalName != "_file" && a.Name.LocalName != "_line") {

					a.Remove();
				}
			}
		}

		private static void InitializeBuildIndexes(XmlObjectClassDefinition cls) {
			cls.Compiled = cls.Source;
			cls.ParamSourceIndex = cls.BuildParametersConfig();
			cls.ParamIndex = new ConfigBase();
			foreach (var p in ((IDictionary<string, object>) cls.ParamSourceIndex)) {
				cls.ParamIndex.Set(p.Key, p.Value);
			}
		}

		private static void BindParametersToCompiledClass(XmlObjectClassDefinition cls) {
			foreach (var e in ((IDictionary<string,object>)cls.ParamIndex)) {
					cls.Compiled.SetAttributeValue(e.Key, e.Value.ToStr());
			}
		}

		private void InterpolateElements(XmlObjectClassDefinition cls) {
			if (GetConfig().UseInterpolation) {
				var xi = new XmlInterpolation();
				xi.Interpolate(cls.Compiled);
			}
		}

		private void InterpolateFields(XmlObjectClassDefinition cls) {
			if (GetConfig().UseInterpolation) {
				var si = new StringInterpolation();

				for (var i = 0; i <= 3; i++) {
					var _substs = ((IDictionary<string,object>)cls.ParamIndex).Where(_ => (_.Value is string) && ((string) _.Value).Contains("${")).ToArray();
					if (0 == _substs.Length) {
						break;
					}
					foreach (var s in _substs) {
						cls.ParamIndex.Set(s.Key, si.Interpolate((string) s.Value, cls.ParamSourceIndex));
					}
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