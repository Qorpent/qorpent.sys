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
	///     Компилятор ObjectXml по умолчанию
	/// </summary>
	public class ObjectXmlCompiler : ObjectXmlCompilerBase {
		/// <summary>
		///     Текущий контекстный индекс
		/// </summary>
		protected ObjectXmlCompilerIndex _currentBuildIndex;

		/// <summary>
		///     Перекрыть для создания индексатора
		/// </summary>
		/// <param name="sources"></param>
		/// <returns></returns>
		protected override ObjectXmlCompilerIndex BuildIndex(IEnumerable<XElement> sources) {
			_currentBuildIndex = new ObjectXmlCompilerIndex();
			ObjectXmlClass[] baseindex = IndexizeRawClasses(sources).ToArray();
			_currentBuildIndex.RawClasses = baseindex.ToDictionary(_ => _.FullName, _ => _);
			return _currentBuildIndex;
		}

		private IEnumerable<ObjectXmlClass> IndexizeRawClasses(IEnumerable<XElement> sources) {
			foreach (XElement src in sources) {
				foreach (ObjectXmlClass e in IndexizeRawClasses(src, "")) {
					yield return e;
				}
			}
		}

		private IEnumerable<ObjectXmlClass> IndexizeRawClasses(XElement src, string ns) {
			foreach (XElement e in src.Elements()) {
				if (e.Name.LocalName == "namespace") {
					if (string.IsNullOrWhiteSpace(ns)) {
						ns = e.Attr("code");
					}
					else {
						ns = ns + "." + e.Attr("code");
					}
					foreach (ObjectXmlClass e_ in IndexizeRawClasses(e, ns)) {
						yield return e_;
					}
				}
				else {
					var def = new ObjectXmlClass();
					def.Source = e;
					def.Name = e.Attr("code");
					def.Namespace = ns;

					if (null != e.Attribute("abstract") || e.Attr("name") == "abstract") {
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
					foreach (XElement i in e.Elements("import")) {
						var import = new ObjectXmlImport {Condition = i.Value, TargetCode = i.Attr("code")};
						def.Imports.Add(import);
					}
					foreach (XElement i in e.Elements("element")) {
						var merge = new ObjectXmlMerge();
						merge.Name = i.Attr("code");
						merge.Type = ObjectXmlMergeType.Define;
						if (i.Attribute("override") != null) {
							merge.Type = ObjectXmlMergeType.Override;
							merge.TargetName = i.Attr("override");
						}
						else if (null != i.Attribute("extend")) {
							merge.Type = ObjectXmlMergeType.Extension;
							merge.TargetName = i.Attr("extend");
						}
						def.MergeDefs.Add(merge);
					}

					yield return def;
				}
			}
		}

		/// <summary>
		///     Перекрыть для создания линковщика
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		protected override void Link(IEnumerable<XElement> sources, ObjectXmlCompilerIndex index) {
			_currentBuildIndex = index;
			IEnumerable<ObjectXmlClass> _classes = CollectFinalClasses(sources, index);
			foreach (ObjectXmlClass cls in _classes) {
				BuildClass(cls, index);
			}
		}

		private void BuildClass(ObjectXmlClass cls, ObjectXmlCompilerIndex index) {
			InitializeBuildIndexes(cls);
			InterpolateFields(cls);
			BindParametersToCompiledClass(cls);
			InterpolateElements(cls);
			CleanupPrivateMembers(cls);
		}

		private void CleanupPrivateMembers(ObjectXmlClass cls) {
			foreach (
				XAttribute a in
					((IEnumerable) cls.Compiled.XPathEvaluate(".//@*[starts-with(local-name(),'_')]")).OfType<XAttribute>().ToArray()) {
				if (a.Name.LocalName != "_file" && a.Name.LocalName != "_line") {
					a.Remove();
				}
			}
		}

		private static void InitializeBuildIndexes(ObjectXmlClass cls) {
			cls.Compiled = cls.Source;
			cls.ParamSourceIndex = cls.BuildParametersConfig();
			cls.ParamIndex = new ConfigBase();
			foreach (var p in ((IDictionary<string, object>) cls.ParamSourceIndex)) {
				cls.ParamIndex.Set(p.Key, p.Value);
			}
		}

		private static void BindParametersToCompiledClass(ObjectXmlClass cls) {
			foreach (var e in ((IDictionary<string, object>) cls.ParamIndex)) {
				cls.Compiled.SetAttributeValue(e.Key, e.Value.ToStr());
			}
		}

		private void InterpolateElements(ObjectXmlClass cls) {
			if (GetConfig().UseInterpolation) {
				var xi = new XmlInterpolation();
				xi.Interpolate(cls.Compiled);
			}
		}

		private void InterpolateFields(ObjectXmlClass cls) {
			if (GetConfig().UseInterpolation) {
				var si = new StringInterpolation();

				for (int i = 0; i <= 3; i++) {
					KeyValuePair<string, object>[] _substs =
						((IDictionary<string, object>) cls.ParamIndex).Where(_ => (_.Value is string) && ((string) _.Value).Contains("${"))
						                                              .ToArray();
					if (0 == _substs.Length) {
						break;
					}
					foreach (var s in _substs) {
						cls.ParamIndex.Set(s.Key, si.Interpolate((string) s.Value, cls.ParamSourceIndex));
					}
				}
			}
		}

		private IEnumerable<ObjectXmlClass> CollectFinalClasses(IEnumerable<XElement> sources, ObjectXmlCompilerIndex index) {
			BindOrphansAndSetupFullImportName(index);
			ResolveImports(index);
			return index.Working;
		}

		private void ResolveImports(ObjectXmlCompilerIndex index) {
			foreach (ObjectXmlClass w in index.Working) {
				foreach (ObjectXmlImport i in w.Imports) {
					i.Orphaned = true;
					ObjectXmlClass import = ResolveClass(index, i.TargetCode, w.Namespace);
					if (null != import) {
						i.Orphaned = false;
						i.Target = import;
					}
				}
			}
		}

		private static void BindOrphansAndSetupFullImportName(ObjectXmlCompilerIndex index) {
			IEnumerable<ObjectXmlClass> _initiallyorphaned = index.RawClasses.Values.Where(_ => _.Orphaned);
			foreach (ObjectXmlClass o in _initiallyorphaned) {
				string code = o.DefaultImportCode;
				string ns = o.Namespace;
				ObjectXmlClass import = ResolveClass(index, code, ns);
				if (import != null) {
					o.Orphaned = false;
					o.DefaultImport = import;
				}
			}

			index.Orphans = index.RawClasses.Values.Where(_ => _.DetectIfIsOrphaned()).ToList();

			index.Abstracts = index.RawClasses.Values.Where(_ => _.Abstract && !_.DetectIfIsOrphaned()).ToList();

			index.Working = index.RawClasses.Values.Where(_ => !_.Abstract && !_.DetectIfIsOrphaned()).ToList();
		}

		private static ObjectXmlClass ResolveClass(ObjectXmlCompilerIndex index, string code, string ns) {
			ObjectXmlClass import = null;
			if (!string.IsNullOrWhiteSpace(code)) {
				if (code.Contains('.')) {
					if (index.RawClasses.ContainsKey(code)) {
						import = index.RawClasses[code];
					}
				}
				else {
					string probe = ns + "." + code;
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