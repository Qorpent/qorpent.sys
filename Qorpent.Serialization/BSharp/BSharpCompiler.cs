using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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

		/// <summary>
		/// 
		/// </summary>
		public BSharpCompiler(){
			_config = new BSharpConfig();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static BSharpCompiler CreateDefault(){
			var cfg = new BSharpConfig{UseInterpolation = true, SingleSource = true, DoProcessRequires = true};
			var result = new BSharpCompiler();
			result.Initialize(cfg);
			return result;
		}
		/// <summary>
		/// Выполнить компиляцию исходного кода
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IBSharpContext Compile(IEnumerable<XElement> sources, IBSharpConfig config = null){
			var compiler = null == config ? CreateDefault() : new BSharpCompiler();
			if (null != config){
				compiler.Initialize(config);
			}
			return compiler.Compile(sources,(IBSharpContext)null);
		}
		/// <summary>
		/// Асинхронно выполнить компиляцию кода
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public async static Task<IBSharpContext> CompileAsync(IEnumerable<XElement> sources, IBSharpConfig config = null){
			return await Task.Run(() => Compile(sources, config));
		}
		/// <summary>
		/// Скомпилировать отдельный XElement
		/// </summary>
		/// <param name="e"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IBSharpContext Compile(XElement e, IBSharpConfig config= null){
			return Compile(new[]{e}, config);
		}
		/// <summary>
		/// Асинхронная компиляция отдельного XElement
		/// </summary>
		/// <param name="e"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public async static Task<IBSharpContext> CompileAsync(XElement e, IBSharpConfig config = null)
		{
			return await Task.Run(()=>Compile(e, config));
		}


		/// <summary>
		/// Скомпилировать код на BXL
		/// </summary>
		/// <param name="bxl"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IBSharpContext Compile(string bxl, IBSharpConfig config = null)
		{
			return Compile(new[] { new BxlParser().Parse(bxl) }, config);
		}

		/// <summary>
		/// Скомпилировать код на BXL
		/// </summary>
		/// <param name="bxl"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public async static Task<IBSharpContext> CompileAsync(string bxl, IBSharpConfig config = null)
		{
			return await Task.Run(()=>Compile(bxl, config));
		}
		

		/// <summary>
		/// Скомпилировать набор BXL
		/// </summary>
		/// <param name="bxls"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IBSharpContext Compile(IEnumerable<string> bxls, IBSharpConfig config = null){
			var bxl = new BxlParser();
			var xmls = bxls.Select((_, i) => bxl.Parse(_, "code" + i + ".bxl"));
			return Compile(xmls, config);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bxls"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public async static Task<IBSharpContext> CompileAsync(IEnumerable<string> bxls, IBSharpConfig config = null){
			return await Task.Run(() => Compile(bxls, config));
		}

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
			return new ConfigBase(_config.Conditions);
		}

	    private readonly IList<IBSharpCompilerExtension> _extensions = new List<IBSharpCompilerExtension>();

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
		    _global = _config.Global ?? new ConfigBase{UseInheritance = false};
	    }


		/// <summary>
		///     Компилирует источники в перечисление итоговых классов
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="preparedContext"></param>
		/// <returns></returns>
		public IBSharpContext Compile(IEnumerable<XElement> sources , IBSharpContext preparedContext = null){
			var result = preparedContext ?? new BSharpContext(this);
			result.Compiler = this;
			sources = ProcessRequires(sources,result);
			if (_config.SingleSource) {
				return BuildBatch(sources,result);	
			}
			foreach (var src in sources) {
				var subresult = BuildSingle(src);
				result.Merge(subresult);
			}
			return result;
		}
		/// <summary>
		/// Опция для обработки директивы require  в исходных файлах
		/// </summary>
		public bool DoProcessRequires { get { return _config.DoProcessRequires; } }

		private IEnumerable<XElement> ProcessRequires(IEnumerable<XElement> sources, IBSharpContext context){
			if (DoProcessRequires){
				return GetSourcesWithRequireProcessing(sources, context);
			}
			return GetSourcesWithRequireIgnorance(sources, context);
		}

		private IEnumerable<XElement> GetSourcesWithRequireIgnorance(IEnumerable<XElement> sources, IBSharpContext context){
			foreach (var src in sources){
				var requires = src.Elements(BSharpSyntax.Require).ToArray();
				if (requires.Length != 0){
					requires.Remove();
					var message = "requre options in " + src.Describe().File + " ignored";
					context.RegisterError(new BSharpError{
						Level = ErrorLevel.Warning,
						Phase = BSharpCompilePhase.SourceIndexing,
						Message = message,
						Xml = src
					});
					log.Warn(message);
				}
			}
			return sources;
		}

		private IEnumerable<XElement> GetSourcesWithRequireProcessing(IEnumerable<XElement> sources, IBSharpContext context){
			var filenames = sources.ToDictionary(_ => Path.GetFullPath(_.Describe().File).NormalizePath(), _ => _);
			filenames.ToArray().AsParallel().ForAll(src => ProcessRequires(src.Value, src.Key, filenames, context));
			return filenames.Values.ToArray();
		}

		readonly BxlParser _requireBxl =new BxlParser();
		private void ProcessRequires(XElement source,string filename, Dictionary<string, XElement> filenames,IBSharpContext context ){
			var requires = source.Elements(BSharpSyntax.Require).ToArray();
			if (requires.Length != 0){
				var dir = Path.GetDirectoryName(filename);
				requires.Remove();
				foreach (var require in requires){
					
					var name = require.Attr("code");
					if (filenames.ContainsKey(name)) continue;
					
					var pkgservice = ResolveService<IBSharpSourceCodeProvider>(name + ".bssrcpkg");
					if (null != pkgservice){
						filenames[name] = new XElement("stub");
						foreach (var element in pkgservice.GetSources(this,null).ToArray()){
							var fn = element.Describe().File;
							if (!filenames.ContainsKey(fn)){
								filenames[fn] = element;
							}
						}
					}
					else{

						var file = require.Attr("code") + ".bxls";

						if (!Path.IsPathRooted(file)){
							file = Path.GetFullPath(Path.Combine(dir, file)).NormalizePath();
						}
						if (filenames.ContainsKey(file)) continue;
						if (File.Exists(file)){
							var src = _requireBxl.Parse(File.ReadAllText(file), file);
							filenames[file] = src;
							ProcessRequires(src, file, filenames,context);
						}
						else{
							var message = "cannot  find required module " + require.Attr("code") + " for " + source.Describe().File;
							context.RegisterError(new BSharpError{Level = ErrorLevel.Error,Phase = BSharpCompilePhase.SourceIndexing,Message = message,Xml=require});
							this.log.Error(message);
						}
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
			var buffer = new ConcurrentBag<IBSharpClass>();
			sources.AsParallel().ForAll(src =>{
				Preprocess(src);
				foreach (IBSharpClass e in IndexizeRawClasses(src, "")){
					buffer.Add(e);
				}
			});
			return buffer;
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

		/// <summary>
		/// 
		/// </summary>
		[Inject]
		public IBSharpSqlAdapter SqlAdapter{
			get { return _sqlAdapter ?? (_sqlAdapter = new BSharpSqlAdapter()); }
			set { _sqlAdapter = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		public IConfig Global{
			get { return _global; }
		}


		LogicalExpressionEvaluator eval = new LogicalExpressionEvaluator();
		private IBSharpSqlAdapter _sqlAdapter;
		static string[] ignores = new[] { "code", "name", "_file", "_line" };
		private IConfig _global;

		private IEnumerable<IBSharpClass> IndexizeRawClasses(XElement src, string ns){
			
			var aliases = new Dictionary<string, string>();
			foreach (XElement e in src.Elements()) {
				var _ns = "";
				if (null != _config.IgnoreElements && 0 != _config.IgnoreElements.Length)
				{
					if (-1 != Array.IndexOf(_config.IgnoreElements, e.Name.LocalName)){
						continue;
						
					}
				}

				if (e.Name.LocalName == BSharpSyntax.Namespace) {
                    var ifa = e.Attr("if");
                    if (!string.IsNullOrWhiteSpace(ifa))
                    {
                        var terms = new DictionaryTermSource<object>(GetConditions());
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
				}

				else if(e.Name.LocalName==BSharpSyntax.AliasImport){
					
					foreach (var attr in e.Attributes()){
						if (-1 == Array.IndexOf(ignores, attr.Name.LocalName)){
							aliases[attr.Name.LocalName] = attr.Value;
						}
					}
				}
				else if (e.Name.LocalName == BSharpSyntax.Require)
				{
					continue;
				}
				else if (e.Name.LocalName == BSharpSyntax.Dataset)
				{
					var def = new BSharpClass(CurrentBuildContext) { Source = e, Name = BSharpSyntax.DatasetClassCodePrefix+e.Attr("code"), Namespace = ns ?? string.Empty };
					def.Set(BSharpClassAttributes.Dataset);
					yield return def;
				}else if (e.Name.LocalName == BSharpSyntax.Generator)
				{
					yield return PrepareGenerator(ns, e);
				}
				else if (e.Name.LocalName == BSharpSyntax.Connection){
					yield return PrepareConnection(ns, e);
				}
				else if (e.Name.LocalName == BSharpSyntax.Template)
				{
					yield return PrepareTemplate(ns, e);
				}
				else
				{
					var def = ReadSingleClassSource(e, ns,aliases);
					if(null!=def)yield return def;
				}
			}
		}
		

		private IBSharpClass PrepareTemplate(string ns, XElement e)
		{
			var _code = e.Attr(BSharpSyntax.ClassNameAttribute);
			var code = BSharpSyntax.GenerateTemplateClassName( _code);
			e.SetAttr(BSharpSyntax.ConnectionCodeAttribute, e.Attr("code"));
			e.SetAttr(BSharpSyntax.ClassNameAttribute, code);
			e.SetAttr(BSharpSyntax.TemplateValueAttribute, e.Value);
			var def = new BSharpClass(CurrentBuildContext) { Source = e, Name = code, Namespace = ns ?? string.Empty };
			def.Set(BSharpClassAttributes.Template);
			return def;
		}

		private IBSharpClass PrepareConnection(string ns, XElement e){
			var mode = e.Attr(BSharpSyntax.ConnectionModeAttribute, "sql");
			var _code = e.Attr(BSharpSyntax.ClassNameAttribute);
			var code = BSharpSyntax.GenerateConnectionClassName(mode, _code);
			e.SetAttr(BSharpSyntax.ConnectionCodeAttribute, e.Attr("code"));
			e.SetAttr(BSharpSyntax.ClassNameAttribute, code);
			e.SetAttr(BSharpSyntax.ConnectionStringAttribute, e.Value);
			var def = new BSharpClass(CurrentBuildContext){Source = e, Name = code, Namespace = ns ?? string.Empty};
			def.Set(BSharpClassAttributes.Connection);
			return def;
		}

		private IBSharpClass PrepareGenerator(string ns, XElement e){
			var code = "gen" + Guid.NewGuid().ToString().Replace("-", "");
			e.SetAttributeValue(BSharpSyntax.GeneratorClassCodeAttribute, e.Attr("code"));
			e.SetAttributeValue(BSharpSyntax.GeneratorClassNameAttribute, e.Attr("name"));
			e.SetAttributeValue("code", code);
			var def = new BSharpClass(CurrentBuildContext){Source = e, Name = code, Namespace = ns ?? string.Empty};
			def.Set(BSharpClassAttributes.Generator);
			return def;
		}

		/// <summary>
		/// Считать исходный 
		/// </summary>
		/// <param name="e"></param>
		/// <param name="ns"></param>
		/// <param name="aliases"></param>
		/// <returns></returns>
		public IBSharpClass ReadSingleClassSource(XElement e, string ns, IDictionary<string, string> aliases){
			bool anonym = false;
			var selfcode = e.Attr("code");
			if (string.IsNullOrWhiteSpace(selfcode)){
				var clsbody = new XElement(e);
				foreach (var e_ in clsbody.DescendantsAndSelf()){
					e_.SetAttributeValue("_file", null);
					e_.SetAttributeValue("_line", null);	
				}
				
				selfcode = "cls"+(
					BitConverter.ToUInt64(MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(clsbody.ToString())),0));
				e.SetAttributeValue("code",selfcode);
				anonym = true;
			}
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
			if (anonym){
				def.Set(BSharpClassAttributes.Anonymous);
			}
			if (!IsOverrideMatch(def)) return null;
			SetupInitialOrphanState(e, def,aliases);
			ParseImports(e, def, aliases);
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
                    var terms = new DictionaryTermSource<object>(GetConditions());
                    return eval.Eval(ifa, terms);
                }
            }
            return true;
        }

		private  void SetupInitialOrphanState(XElement e, IBSharpClass def,IDictionary<string,string> aliases ) {
			if (null != e.Attribute(BSharpSyntax.ClassAbstractModifier) || e.Attr("name") == BSharpSyntax.ClassAbstractModifier)
			{
				def.Set(BSharpClassAttributes.Abstract);
			}
			

			if (null != e.Attribute(BSharpSyntax.ClassStaticModifier) || e.Attr("name") == BSharpSyntax.ClassStaticModifier)
			{
				def.Set(BSharpClassAttributes.Static);
			}

			if (null != e.Attribute(BSharpSyntax.ClassGenericModifier) || e.Attr("name") == BSharpSyntax.ClassGenericModifier)
			{
				def.Set(BSharpClassAttributes.Generic);
				def.Set(BSharpClassAttributes.Abstract);
				def.Set(BSharpClassAttributes.Static);
			}
			if (e.Name.LocalName == BSharpSyntax.Class) {
				def.Set(BSharpClassAttributes.Explicit);
			}
			if (null != e.Attribute(BSharpSyntax.EmbedAttribute) || e.Attr("name") == BSharpSyntax.EmbedAttribute){
				def.Set(BSharpClassAttributes.Embed);
				if (e.Attr("name") == BSharpSyntax.EmbedAttribute){
					e.Attribute("name").Remove();

				}
				else{
					e.Attribute(BSharpSyntax.EmbedAttribute).Remove();
				}
			}
			if (e.Name.LocalName == BSharpSyntax.PatchClassKeyword)
			{
				def.Set(BSharpClassAttributes.Explicit);
				def.Set(BSharpClassAttributes.Patch);
				def.Set(BSharpClassAttributes.Embed);
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
				if (null!=aliases && aliases.ContainsKey(def.DefaultImportCode)){
					def.AliasImportCode = def.DefaultImportCode;
					def.DefaultImportCode = aliases[def.DefaultImportCode];
				}
			}
		}

		private static void ParseImports(XElement e, IBSharpClass def, IDictionary<string, string> aliases) {
			foreach (XElement i in e.Elements("import")) {
				var import = new BSharpImport {Condition = i.Attr("if"), TargetCode = i.Attr("code"), Source = i};
				if (null!=aliases && aliases.ContainsKey(import.TargetCode)){
					import.Alias = import.TargetCode;
					import.TargetCode = aliases[import.TargetCode];
					
				}
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
			//статические классы нужно строить до всех остальных
			context.Get(BSharpContextDataType.Statics).AsParallel().ForAll(
				_ => CompileSingle(context, _))
				;	
			context.Get(BSharpContextDataType.Working).AsParallel().ForAll(
				_ => CompileSingle(context, _))
				;	
  
		}

		private void CompileSingle(IBSharpContext context, IBSharpClass _){
			try{
				BSharpClassBuilder.Build(BuildPhase.Compile, this, _, context);
			}
			catch (Exception ex){
				_.Error = ex;
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
			var requirelink = context.RequireLinking();
			var requirepatch = context.RequirePatching();
			if (!(requirelink || requirepatch)) return;
			
				if (requirelink){
					context.Get(BSharpContextDataType.Working)
					       .Where(_ => _.Is(BSharpClassAttributes.RequireLinking))
					       .AsParallel()
					       .ForAll(
						       _ =>{
							       try{
								       BSharpClassBuilder.Build(BuildPhase.AutonomeLink, this, _, context);
							       }
							       catch (Exception ex){
								       _.Error = ex;
							       }
						       })
						;
					context.Get(BSharpContextDataType.Working)
					       .Where(_ => _.Is(BSharpClassAttributes.RequireLinking))
					       .AsParallel()
					       .ForAll(
						       _ =>{
							       try{
								       BSharpClassBuilder.Build(BuildPhase.CrossClassLink, this, _, context);
							       }
							       catch (Exception ex){
								       _.Error = ex;
							       }
						       })
						;
				}
				if (requirepatch){
					context.Get(BSharpContextDataType.Working).Where(_ => _.Is(BSharpClassAttributes.Patch)).OrderBy(_=>_.Priority).Select(
						_ =>{
							try{
								BSharpClassBuilder.Build(BuildPhase.ApplyPatch, this, _, context);
							}
							catch (Exception ex){
								_.Error = ex;
							}
							return "";
						}).ToArray()
						;
				}
			
		}
	}
}