using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp {
	/// <summary>
	///     Компилятор BxlSharp по умолчанию
	/// </summary>
	public class BSharpCompiler : BSharpCompilerBase {
		/// <summary>
		///     Текущий контекстный индекс
		/// </summary>
		protected IBSharpContext CurrentBuildContext;

		/// <summary>
		///     Перекрыть для создания индексатора
		/// </summary>
		/// <param name="sources"></param>
		/// <returns></returns>
		protected override IBSharpContext BuildIndex(IEnumerable<XElement> sources) {
			
			var baseindex = IndexizeRawClasses(sources);
			CurrentBuildContext = new BSharpContext();
			CurrentBuildContext.Setup(baseindex);
			CurrentBuildContext.Build();
			return CurrentBuildContext;
		}

		private IEnumerable<BSharpClass> IndexizeRawClasses(IEnumerable<XElement> sources) {
			foreach (XElement src in sources) {
				foreach (BSharpClass e in IndexizeRawClasses(src, "")) {
					yield return e;
				}
			}
		}

		private IEnumerable<BSharpClass> IndexizeRawClasses(XElement src, string ns) {
			foreach (XElement e in src.Elements()) {
				var _ns = "";
				if (e.Name.LocalName == "namespace") {
					if (string.IsNullOrWhiteSpace(ns)) {
						_ns = e.Attr("code");
					}
					else {
						_ns = ns + "." + e.Attr("code");
					}
					foreach (BSharpClass e_ in IndexizeRawClasses(e, _ns)) {
						yield return e_;
					}
				}
				else {
					var def = new BSharpClass {Source = e, Name = e.Attr("code"), Namespace = ns};

					SetupInitialOrphanState(e, def);
					ParseImports(e, def);
					ParseCompoundElements(e, def);

					yield return def;
				}
			}
		}

		private static void SetupInitialOrphanState(XElement e, BSharpClass def) {
			if (null != e.Attribute("abstract") || e.Attr("name") == "abstract") {
				def.Set(BSharpClassAttributes.Abstract);
			}
			if (null != e.Attribute("static") || e.Attr("name") == "static")
			{
				def.Set(BSharpClassAttributes.Static);
			}
			if (e.Name.LocalName == "class") {
				def.Set(BSharpClassAttributes.Explicit);
			}
			else if (e.Name.LocalName == "__TILD__class") {
				def.Set(BSharpClassAttributes.Override);				
			}
			else if (e.Name.LocalName == "__PLUS__class")
			{
				def.Set(BSharpClassAttributes.Extension);			
			}
			else {
				def.Set(BSharpClassAttributes.Orphan);
				def.DefaultImportCode = e.Name.LocalName;
			}
		}

		private static void ParseImports(XElement e, BSharpClass def) {
			foreach (XElement i in e.Elements("import")) {
				var import = new BSharpImport {Condition = i.Attr("if"), TargetCode = i.Attr("code")};
				def.SelfImports.Add(import);
			}
		}

		private static void ParseCompoundElements(XElement e, BSharpClass def) {
			foreach (XElement i in e.Elements("element")) {
				var merge = new BSharpElement();
				merge.Name = i.Attr("code");
				merge.Type = BSharpElementType.Define;
				if (i.Attribute("override") != null) {
					merge.Type = BSharpElementType.Override;
					merge.TargetName = i.Attr("override");
				}
				else if (null != i.Attribute("extend")) {
					merge.Type = BSharpElementType.Extension;
					merge.TargetName = i.Attr("extend");
				}
				def.SelfElements.Add(merge);
			}
		}

		/// <summary>
		///     Перекрыть для создания линковщика
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		protected override void Link(IEnumerable<XElement> sources, IBSharpContext context) {
			
			context.Get(BSharpContextDataType.Working).AsParallel().ForAll(
				_ => {
					try {
						BSharpClassBuilder.Build(this, _, context);
					}
					catch (Exception ex) {
						_.Error = ex;
					}
				})
			;
		}
	}
}