using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp {
	/// <summary>
	///     Абстракция компилятора, опирается на некий набор первичных компиляторов
	/// </summary>
	[ContainerComponent(ServiceType = typeof(IBSharpCompiler))]
	public  class BSharpCompiler :  ServiceBase,IBSharpCompiler {
		private IBSharpConfig _config;

		/// <summary>
		///     Текущий контекстный индекс
		/// </summary>
		protected IBSharpContext CurrentBuildContext;

		/// <summary>
		///     Возвращает конфигурацию компилятора
		/// </summary>
		/// <returns></returns>
		public IBSharpConfig GetConfig() {
			if (null == _config) {
				_config = new BSharpConfig();
			}
			return _config;
		}

		/// <summary>
		/// Возвращает условия компиляции
		/// </summary>
		/// <returns></returns>
		public IConfig GetConditions() {
			return new ConfigBase(GetConfig().Conditions);
		}

		/// <summary>
		/// </summary>
		/// <param name="compilerConfig"></param>
		public void Initialize(IBSharpConfig compilerConfig) {
			_config = compilerConfig;
		}


		/// <summary>
		///     Компилирует источники в перечисление итоговых классов
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="preparedContext"></param>
		/// <returns></returns>
		public IBSharpContext Compile(IEnumerable<XElement> sources , IBSharpContext preparedContext = null) {
			var cfg = GetConfig();
			if (cfg.SingleSource) {
				return BuildBatch(sources);
			}
			IBSharpContext result = preparedContext ?? new BSharpContext(this);
			if (null == result.Compiler) {
				result.Compiler = this;
			}
			
			foreach (XElement src in sources) {
				var subresult = BuildSingle(src);
				result.Merge(subresult);
			}
			return result;
		}

		private IBSharpContext BuildSingle(XElement source) {
			var batch = new[] {source};
			IBSharpContext context = BuildIndex(batch);
			Link(batch, context);
			return context;
		}

		private IBSharpContext BuildBatch(IEnumerable<XElement> sources) {
			XElement[] batch = sources.ToArray();
			var context = BuildIndex(batch);
			Link(batch, context);
			return context;
		}

		/// <summary>
		///     Перекрыть для создания индексатора
		/// </summary>
		/// <param name="sources"></param>
		/// <returns></returns>
		protected virtual IBSharpContext BuildIndex(IEnumerable<XElement> sources) {
			CurrentBuildContext = new BSharpContext(this);
			var baseindex = IndexizeRawClasses(sources);
			
			CurrentBuildContext.Setup(baseindex);
			CurrentBuildContext.Build();
			return CurrentBuildContext;
		}

		private IEnumerable<IBSharpClass> IndexizeRawClasses(IEnumerable<XElement> sources) {
			foreach (XElement src in sources) {
				foreach (IBSharpClass e in IndexizeRawClasses(src, "")) {
					yield return e;
				}
			}
		}

		private IEnumerable<IBSharpClass> IndexizeRawClasses(XElement src, string ns) {
			foreach (XElement e in src.Elements()) {
				var _ns = "";
				if (e.Name.LocalName == "namespace") {
					if (string.IsNullOrWhiteSpace(ns)) {
						_ns = e.Attr("code");
					}
					else {
						_ns = ns + "." + e.Attr("code");
					}
					foreach (IBSharpClass e_ in IndexizeRawClasses(e, _ns)) {
						yield return e_;
					}
				}
				else {
					var def = new BSharpClass(CurrentBuildContext) {Source = e, Name = e.Attr("code"), Namespace = ns};

					SetupInitialOrphanState(e, def);
					ParseImports(e, def);
					ParseCompoundElements(e, def);

					yield return def;
				}
			}
		}

		private static void SetupInitialOrphanState(XElement e, IBSharpClass def) {
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

		private static void ParseImports(XElement e, IBSharpClass def) {
			foreach (XElement i in e.Elements("import")) {
				var import = new BSharpImport {Condition = i.Attr("if"), TargetCode = i.Attr("code"), Source = i};
				def.SelfImports.Add(import);
			}
		}

		private static void ParseCompoundElements(XElement e, IBSharpClass def) {
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
		protected virtual void Link(IEnumerable<XElement> sources, IBSharpContext context) {
			
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