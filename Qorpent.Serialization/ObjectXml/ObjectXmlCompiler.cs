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
					var def = new ObjectXmlClass {Source = e, Name = e.Attr("code"), Namespace = ns};

					SetupInitialOrphanState(e, def);
					ParseImports(e, def);
					ParseCompoundElements(e, def);

					yield return def;
				}
			}
		}

		private static void SetupInitialOrphanState(XElement e, ObjectXmlClass def) {
			if (null != e.Attribute("abstract") || e.Attr("name") == "abstract") {
				def.Abstract = true;
			}
			if (null != e.Attribute("static") || e.Attr("name") == "static")
			{
				def.Static = true;
			}
			if (e.Name.LocalName != "class") {
				def.Orphaned = true;
				def.DefaultImportCode = e.Name.LocalName;
			}
			else {
				def.Orphaned = false;
				def.ExplicitClass = true;
			}
		}

		private static void ParseImports(XElement e, ObjectXmlClass def) {
			foreach (XElement i in e.Elements("import")) {
				var import = new ObjectXmlImport {Condition = i.Attr("if"), TargetCode = i.Attr("code")};
				def.Imports.Add(import);
			}
		}

		private static void ParseCompoundElements(XElement e, ObjectXmlClass def) {
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
		}

		/// <summary>
		///     Перекрыть для создания линковщика
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		protected override void Link(IEnumerable<XElement> sources, ObjectXmlCompilerIndex index) {
			_currentBuildIndex = index;
			CollectClassGroups(sources, index);
			foreach (ObjectXmlClass cls in index.Static) {
				BuildClass(cls, index);
			}
			foreach (ObjectXmlClass cls in index.Working.Where(_=>!_.Static))
			{
				BuildClass(cls, index);
			}
		}

		private void BuildClass(ObjectXmlClass cls, ObjectXmlCompilerIndex index) {
			if (cls.IsBuilt) return;
			InitializeBuildIndexes(cls,index);
			IntializeMergeIndexes(cls,index);
			MergeSimpleInternalsNonStatic(cls);
			InterpolateFields(cls);
			BindParametersToCompiledClass(cls);
			InterpolateElements(cls);
			MergeSimpleInternalsStatic(cls);
			CleanupPrivateMembers(cls);
			cls.IsBuilt = true;
		}

		private void IntializeMergeIndexes(ObjectXmlClass cls, ObjectXmlCompilerIndex index) {
			cls.AllMergeDefs = cls.CollectMerges().ToList();
		}

		private void MergeSimpleInternalsStatic(ObjectXmlClass cls) {
			foreach (var e in cls.CollectImports().Where(_=>_.Static)) {
				cls.Compiled.Add(e.Compiled.Elements());
			}
		}

		private void MergeSimpleInternalsNonStatic(ObjectXmlClass cls) {
			foreach (var e in cls.CollectImports().Where(_ => !_.Static))
			{
				cls.Compiled.Add(e.Source.Elements());
			}
		}

		private void CleanupPrivateMembers(ObjectXmlClass cls) {
			((IEnumerable) cls.Compiled.XPathEvaluate(".//@*[starts-with(local-name(),'_')]")).OfType<XAttribute>().Remove();
			if (cls.Compiled.Attr("name") == "abstract") {
				cls.Compiled.Attribute("name").Remove();
			}
			cls.Compiled.Elements("import").Remove();
			cls.Compiled.Elements("element").Remove();
		}

		private  void InitializeBuildIndexes(ObjectXmlClass cls,ObjectXmlCompilerIndex index) {
			cls.Compiled = new XElement( cls.Source);
			cls.ParamSourceIndex = BuildParametersConfig(cls,index);
			cls.ParamIndex = new ConfigBase();
			foreach (var p in cls.ParamSourceIndex) {
				cls.ParamIndex.Set(p.Key, p.Value);
			}

		}

		/// <summary>
		///     Возвращает XML для резолюции атрибутов
		/// </summary>
		/// <returns></returns>
		public  IConfig BuildParametersConfig(ObjectXmlClass cls,ObjectXmlCompilerIndex index)
		{

			var result = new ConfigBase();
			ConfigBase current = result;
			foreach (var i in cls.CollectImports().Union(new[] { cls }))
			{
				var selfconfig = new ConfigBase();
				selfconfig.Set("_class_", cls.FullName);
				selfconfig.SetParent(current);
				current = selfconfig;
				if (i.Static && cls!=i)
				{
					if (!i.IsBuilt) {
						BuildClass(i,index);
					}
					foreach (var p in  i.ParamIndex) {
						current.Set(p.Key,p.Value);
					}
				}
				else
				{
					foreach (XAttribute a in i.Source.Attributes())
					{
						current.Set(a.Name.LocalName, a.Value);
					}
				}
			}
			return current;
		}

		private static void BindParametersToCompiledClass(ObjectXmlClass cls) {
			foreach (var e in cls.ParamIndex) {
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
						cls.ParamIndex.Where(_ => (_.Value is string) && ((string) _.Value).Contains("${"))
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

		private void CollectClassGroups(IEnumerable<XElement> sources, ObjectXmlCompilerIndex index) {
			BindOrphansAndSetupFullImportName(index);
			ResolveImports(index);
		}

		private void ResolveImports(ObjectXmlCompilerIndex index) {
			foreach (var w in index.Working.Union(index.Abstracts)) {
				foreach (ObjectXmlImport i in w.Imports) {
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
			IEnumerable<ObjectXmlClass> _initiallyorphaned = index.RawClasses.Values.Where(_ => _.Orphaned);
			foreach (var o in _initiallyorphaned) {
				string code = o.DefaultImportCode;
				string ns = o.Namespace;
				var import = ResolveClass(index, code, ns);
				if (import != null) {
					o.Orphaned = false;
					o.DefaultImport = import;
				}
			}

			index.Orphans = index.RawClasses.Values.Where(_ => _.DetectIfIsOrphaned()).ToList();

			index.Abstracts = index.RawClasses.Values.Where(_ => _.Abstract && !_.DetectIfIsOrphaned()).ToList();

			index.Working = index.RawClasses.Values.Where(_ => !_.Abstract && !_.DetectIfIsOrphaned()).ToList();
			index.Static = index.RawClasses.Values.Where(_ => _.Static && !_.DetectIfIsOrphaned()).ToList();
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