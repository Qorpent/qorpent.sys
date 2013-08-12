using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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

		protected override ObjectXmlCompilerIndex BuildIndex(IEnumerable<XElement> sources) {
			_currentBuildIndex = new ObjectXmlCompilerIndex();
			_currentBuildIndex.RawClasses = IndexizeRawClasses(sources).ToDictionary(_=>_.FullName,_=>_);
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
					ns = ns + "." + e.Attr("code");
					foreach (var e_ in IndexizeRawClasses(e, ns)) {
						yield return e_;
					}
				}
				else {
					var def = new XmlObjectClassDefinition();
					def.Source = e;
					def.Name = e.Attr("code");
					def.Namespace = ns;
					if (null != e.Attribute("abstract")) {
						def.Abstract = true;
					}
					if (e.Name.LocalName != "class") {
						def.Orphaned = true;
						def.DefaultImportCode = e.Name.LocalName;
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

		protected override IEnumerable<XElement> Link(IEnumerable<XElement> sources, ObjectXmlCompilerIndex index) {
			_currentBuildIndex = index;
			var _classes = CollectFinalClasses(sources, index);
			foreach (var cls in _classes) {
				BuildClass(cls, index);
			}
			return _classes.Select(_ => _.Compiled);
		}

		private void BuildClass(XmlObjectClassDefinition cls, ObjectXmlCompilerIndex index) {
			cls.Compiled = cls.Source;
		}

		private IEnumerable<XmlObjectClassDefinition> CollectFinalClasses(IEnumerable<XElement> sources, ObjectXmlCompilerIndex index) {
			BindOrphansAndSetupFullImportName(index);
			return index.Working;
		}

		private static void BindOrphansAndSetupFullImportName(ObjectXmlCompilerIndex index) {
			var _initiallyorphaned = index.RawClasses.Values.Where(_ => _.Orphaned);
			foreach (var o in _initiallyorphaned) {
				if (!string.IsNullOrWhiteSpace(o.DefaultImportCode)) {
					if (o.DefaultImportCode.Contains('.')) {
						if (index.RawClasses.ContainsKey(o.DefaultImportCode)) {
							o.DefaultImport = index.RawClasses[o.DefaultImportCode];
							o.Orphaned = false;
						}
					}
					else {
						var probe = o.Namespace + "." + o.DefaultImportCode;
						if (index.RawClasses.ContainsKey(probe)) {
							o.DefaultImport = index.RawClasses[probe];
							o.Orphaned = false;
						}
						else {
							probe = o.DefaultImportCode;
							if (index.RawClasses.ContainsKey(probe))
							{
								o.DefaultImport = index.RawClasses[probe];
								o.Orphaned = false;
							}
						}
					}
				}
			}

			index.Orphaned = index.RawClasses.Values.Where(_ => _.DetectIfIsOrphaned()).ToList();

			index.Abstracts = index.RawClasses.Values.Where(_ => _.Abstract && !_.DetectIfIsOrphaned()).ToList();

			index.Working = index.RawClasses.Values.Where(_ => !_.Abstract && !_.DetectIfIsOrphaned()).ToList();
		}
	}
}