using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.Config;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Utils.Extensions;
using Qorpent.LogicalExpressions;
using Qorpent.Utils.LogicalExpressions;

namespace Qorpent.BSharp {
	/// <summary>
	///     Абстракция компилятора, опирается на некий набор первичных компиляторов
	/// </summary>
	[ContainerComponent(ServiceType = typeof(IBSharpCompiler))]
	public  class BSharpCompiler :  ServiceBase,IBSharpCompiler {
		private IBSharpConfig _config;
		IUserLog log {
			get { return GetConfig().Log; }
		}
        /// <summary>
        /// Коллекция расширений
        /// </summary>
	    public IList<IBSharpCompilerExtension> Extensions {
	        get { return _extensions; }
	    }

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

	    private IList<IBSharpCompilerExtension> _extensions = new List<IBSharpCompilerExtension>();

	    /// <summary>
	    /// Выполняет расширения
	    /// </summary>
	    /// <param name="cls"></param>
	    /// <param name="context"></param>
	    /// <param name="phase"></param>
	    public void CallExtensions(IBSharpClass cls, IBSharpContext context, BSharpCompilePhase phase) {
	        if(0==Extensions.Count)return;
            foreach (var extension in Extensions) {
                extension.Execute(this,context,cls,phase);
            }
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
		public IBSharpContext Compile(IEnumerable<XElement> sources , IBSharpContext preparedContext = null){
			//if (this.DoProcessRequires){
				sources = ProcessRequires(sources);
			//}
			var cfg = GetConfig();
			if (cfg.SingleSource) {
				return BuildBatch(sources,preparedContext);
				
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
		/// <summary>
		/// Опция для обработки директивы require  в исходных файлах
		/// </summary>
		public bool DoProcessRequires { get; set; }

		private IEnumerable<XElement> ProcessRequires(IEnumerable<XElement> sources){
			if (DoProcessRequires){
				var filenames = sources.ToDictionary(_ => Path.GetFullPath(_.Describe().File).NormalizePath(), _ => _);
				foreach (var src in filenames){
					ProcessRequires(src.Value, src.Key, filenames);
				}
				return filenames.Values.ToArray();
			}
			else{
				foreach (var src in sources){
					var requires = src.Elements("requre").ToArray();
					if (requires.Length != 0){
						requires.Remove();
						log.Warn("requre options in "+src.Describe().File+" ignored");
					}
				}
				return sources;
			}
			
		}
		BxlParser requireBxl =new BxlParser();

		private void ProcessRequires(XElement source,string filename, Dictionary<string, XElement> filenames){
			var requires = source.Elements("requre").ToArray();
			if (requires.Length != 0){
				var dir = Path.GetDirectoryName(filename);
				requires.Remove();
				foreach (var require in requires){
					var file = require.Attr("code")+".bxls";
					
					if (!Path.IsPathRooted(file)){
						file = Path.GetFullPath(Path.Combine(dir, file)).NormalizePath();
					} 
					if(filenames.ContainsKey(file))continue;
					if (File.Exists(file)){
						var src = requireBxl.Parse(File.ReadAllText(file), file);
						filenames[file] = src;
						ProcessRequires(src, file, filenames);
					}
					else{
						this.log.Error("cannot  find required module "+require.Attr("code")+" for "+source.Describe().File);
					}
				}
			}
		}

		private IBSharpContext BuildSingle(XElement source) {
			var batch = new[] {source};
			var context = Build(batch);
		    return context;
		}

	    private IBSharpContext Build(XElement[] batch) {
	        IBSharpContext context = BuildIndex(batch);
	        CompileClasses(batch, context);
	        LinkClasses(batch, context);
	        return context;
	    }

	    private IBSharpContext BuildBatch(IEnumerable<XElement> sources, IBSharpContext preparedContext) {
			XElement[] batch = sources.ToArray();
            var context = Build(batch);
			if (null != preparedContext) {
				preparedContext.Merge(context);
				return preparedContext;
			}
			return context;
		}

		/// <summary>
		///     Перекрыть для создания индексатора
		/// </summary>
		/// <param name="sources"></param>
		/// <returns></returns>
		protected virtual IBSharpContext BuildIndex(IEnumerable<XElement> sources) {
			CurrentBuildContext = new BSharpContext(this);
			var baseindex = IndexizeRawClasses(sources).ToArray();
			CurrentBuildContext.Setup(baseindex);
			CurrentBuildContext.ExecuteGenerators();
			CurrentBuildContext.Build();
			return CurrentBuildContext;
		}

		private IEnumerable<IBSharpClass> IndexizeRawClasses(IEnumerable<XElement> sources) {
			foreach (XElement src in sources) {
			    Preprocess(src);
				foreach (IBSharpClass e in IndexizeRawClasses(src, "")) {
					yield return e;
				}
			}
		}

        private void Preprocess(XElement src)
        {
           var sets = src.Descendants("set").Reverse().ToArray();

            foreach (var s in sets)
            {
                var subelements = s.Elements().ToArray();
                foreach (var a in s.Attributes())
                {
                    foreach (var sb in subelements)
                    {
                        if (null == sb.Attribute(a.Name))
                        {
                            sb.SetAttributeValue(a.Name, a.Value);
                        }
                    }
                }
                s.ReplaceWith(subelements);
            }
            
        }


        LogicalExpressionEvaluator eval = new LogicalExpressionEvaluator();

		private IEnumerable<IBSharpClass> IndexizeRawClasses(XElement src, string ns) {
			foreach (XElement e in src.Elements()) {
				var _ns = "";
				if (e.Name.LocalName == BSharpSyntax.Namespace) {
                    var ifa = e.Attr("if");
                    if (!string.IsNullOrWhiteSpace(ifa))
                    {
                        var terms = new DictionaryTermSource(this.GetConditions());
                        if (!eval.Eval(ifa, terms))
                        {
                            continue;
                        }
                    }

					if (string.IsNullOrWhiteSpace(ns)) {
						_ns = e.Attr("code");
					}
					else {
						_ns = ns + "." + e.Attr("code");
					}
					foreach (IBSharpClass e_ in IndexizeRawClasses(e, _ns)) {
						yield return e_;
					}
				}else if (e.Name.LocalName == BSharpSyntax.Dataset)
				{
					var def = new BSharpClass(CurrentBuildContext) { Source = e, Name = BSharpSyntax.DatasetClassCodePrefix+e.Attr("code"), Namespace = ns ?? string.Empty };
					def.Set(BSharpClassAttributes.Dataset);
					yield return def;
				}else if (e.Name.LocalName == BSharpSyntax.Generator)
				{
					var code = "gen" + Guid.NewGuid().ToString().Replace("-", "");
					e.SetAttributeValue("classCode",e.Attr("code"));
					e.SetAttributeValue("className",e.Attr("name"));
					e.SetAttributeValue("code",code);
					var def = new BSharpClass(CurrentBuildContext) { Source = e, Name = code, Namespace = ns ?? string.Empty };
					def.Set(BSharpClassAttributes.Generator);
					yield return def;
				}
				else
				{
					var def = ReadSingleClassSource(e, ns);
					if(null!=def)yield return def;
				}
			}
		}

		/// <summary>
		/// Считать исходный 
		/// </summary>
		/// <param name="e"></param>
		/// <param name="ns"></param>
		/// <returns></returns>
		public IBSharpClass ReadSingleClassSource(XElement e, string ns)
		{
			var selfcode = e.Attr("code");
			var __ns = ns ?? string.Empty;
			if (selfcode.Contains("."))
			{
				var lastdot = selfcode.LastIndexOf('.');
				var _nsadd = selfcode.Substring(0, lastdot);
				selfcode = selfcode.Substring(lastdot + 1);
				if (string.IsNullOrWhiteSpace(__ns))
				{
					__ns = _nsadd;
				}
				else
				{
					__ns = __ns + "." + _nsadd;
				}
			}
			var def = new BSharpClass(CurrentBuildContext) {Source = e, Name = selfcode, Namespace = __ns};
			if (null != def.Source.Attribute("_file"))
			{
				def.Source.SetAttributeValue("_dir", Path.GetDirectoryName(def.Source.Attr("_file")).Replace("\\", "/"));
			}
			if (!IsOverrideMatch(def)) return null;
			SetupInitialOrphanState(e, def);
			ParseImports(e, def);
			ParseCompoundElements(e, def);
			return def;
		}

		private bool IsOverrideMatch(BSharpClass def)
        {
            if (def.Source.Name.LocalName == BSharpSyntax.ClassOverrideKeyword || def.Source.Name.LocalName == BSharpSyntax.ClassExtensionKeyword)
            {
                var ifa = def.Source.Attr("if");
                if (!string.IsNullOrWhiteSpace(ifa))
                {
                    def.Source.Attribute("if").Remove();
                    var terms = new DictionaryTermSource(GetConditions());
                    return eval.Eval(ifa, terms);
                }
            }
            return true;
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
            else if (e.Name.LocalName == BSharpSyntax.ClassOverrideKeyword)
            {
				def.Set(BSharpClassAttributes.Override);				
			}
            else if (e.Name.LocalName == BSharpSyntax.ClassExtensionKeyword)
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
		protected virtual void CompileClasses(IEnumerable<XElement> sources, IBSharpContext context) {
			if (Debugger.IsAttached) {
				foreach (var c in context.Get(BSharpContextDataType.Working)) {
					try
					{
						BSharpClassBuilder.Build(BuildPhase.Compile,  this, c, context);
					}
					catch (Exception ex)
					{
						c.Error = ex;
					}
				}
				context.ClearBuildTasks();
                
			}
			else {
				context.Get(BSharpContextDataType.Working).AsParallel().ForAll(
					_ =>
					{
						try
						{
							BSharpClassBuilder.Build(BuildPhase.Compile,  this, _, context);
						}
						catch (Exception ex)
						{
							_.Error = ex;
						}
					})
					;	
  
			}
		}

		/// <summary>
		///     Перекрыть для создания линковщика
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		protected virtual void LinkClasses(IEnumerable<XElement> sources, IBSharpContext context) {
			context.BuildLinkingIndex();
			if (!context.RequireLinking()) return;
			if (Debugger.IsAttached)
			{
				foreach (var c in context.Get(BSharpContextDataType.Working).Where(_=>_.Is(BSharpClassAttributes.RequireLinking)))
				{
					try
					{
						BSharpClassBuilder.Build(BuildPhase.AutonomeLink, this, c, context);
					}
					catch (Exception ex)
					{
						c.Error = ex;
					}
				}
				context.ClearBuildTasks();
                foreach (var c in context.Get(BSharpContextDataType.Working).Where(_ => _.Is(BSharpClassAttributes.RequireLinking)))
                {
                    try
                    {
                        BSharpClassBuilder.Build(BuildPhase.CrossClassLink, this, c, context);
                    }
                    catch (Exception ex)
                    {
                        c.Error = ex;
                    }
                }
                context.ClearBuildTasks();
			}
			else
			{
				context.Get(BSharpContextDataType.Working).Where(_=>_.Is(BSharpClassAttributes.RequireLinking)).AsParallel().ForAll(
					_ =>
					{
						try
						{
							BSharpClassBuilder.Build(BuildPhase.AutonomeLink, this, _, context);
						}
						catch (Exception ex)
						{
							_.Error = ex;
						}
					})
					;
                context.Get(BSharpContextDataType.Working).Where(_ => _.Is(BSharpClassAttributes.RequireLinking)).AsParallel().ForAll(
                    _ =>
                    {
                        try
                        {
                            BSharpClassBuilder.Build(BuildPhase.CrossClassLink, this, _, context);
                        }
                        catch (Exception ex)
                        {
                            _.Error = ex;
                        }
                    })
                    ;
			}
		}
	}
}