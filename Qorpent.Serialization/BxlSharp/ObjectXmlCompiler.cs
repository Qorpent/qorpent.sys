using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.BxlSharp {
	/// <summary>
	///     Компилятор BxlSharp по умолчанию
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
			var baseindex = IndexizeRawClasses(sources).ToArray();
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
				var _ns = "";
				if (e.Name.LocalName == "namespace") {
					if (string.IsNullOrWhiteSpace(ns)) {
						_ns = e.Attr("code");
					}
					else {
						_ns = ns + "." + e.Attr("code");
					}
					foreach (ObjectXmlClass e_ in IndexizeRawClasses(e, _ns)) {
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
			if (e.Name.LocalName == "class") {
				def.Orphaned = false;
				def.ExplicitClass = true;
			}
			else if (e.Name.LocalName == "__TILD__class") {
				def.IsClassOverride = true;
				def.TargetClassName = def.Name;
				def.Name = Guid.NewGuid().ToString();
				def.Orphaned = false;
				def.ExplicitClass = true;
			}
			else if (e.Name.LocalName == "__PLUS__class")
			{
				def.IsClassExtension = true;
				def.TargetClassName = def.Name;
				def.Name = Guid.NewGuid().ToString();
				def.Orphaned = false;
				def.ExplicitClass = true;
			}
			else {
				def.Orphaned = true;
				def.DefaultImportCode = e.Name.LocalName;
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
			index.Build();
			index.Working.AsParallel().ForAll(
				_ => {
					try {
						ObjectXmlClassBuilder.Build(this, _, index);
					}
					catch (Exception ex) {
						_.Error = ex;
					}
				})
			;
		}
	}
}