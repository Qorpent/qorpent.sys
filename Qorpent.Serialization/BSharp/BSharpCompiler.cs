using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.Config;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.LogicalExpressions;
using Qorpent.Serialization;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.LogicalExpressions;

namespace Qorpent.BSharp{
	/// <summary>
	///     Абстракция компилятора, опирается на некий набор первичных компиляторов
	/// </summary>
	[ContainerComponent(ServiceType = typeof (IBSharpCompiler))]
	public class BSharpCompiler : ServiceBase, IBSharpCompiler{
		private static readonly string[] ignores = new[]{"code", "name", "_file", "_line"};
		private readonly IDictionary<string, string> _baseglobals = new Dictionary<string, string>();
		private readonly IList<IBSharpCompilerExtension> _extensions = new List<IBSharpCompilerExtension>();
		private readonly IDictionary<string, string> _extlobals = new Dictionary<string, string>();
		private readonly IDictionary<string, string> _overlobals = new Dictionary<string, string>();
		private readonly BxlParser _requireBxl = new BxlParser();
		private readonly LogicalExpressionEvaluator eval = new LogicalExpressionEvaluator();

		/// <summary>
		///     Текущий контекстный индекс
		/// </summary>
		protected IBSharpContext CurrentBuildContext;

		private IBSharpConfig _config;
		private IConfig _global;
		private IBSharpSqlAdapter _sqlAdapter;

		/// <summary>
		/// </summary>
		public BSharpCompiler(){
			_config = new BSharpConfig();
		}

		private IUserLog log{
			get { return GetConfig().Log; }
		}

		/// <summary>
		///     Коллекция расширений
		/// </summary>
		public IList<IBSharpCompilerExtension> Extensions{
			get { return _extensions; }
		}

		/// <summary>
		///     Опция для обработки директивы require  в исходных файлах
		/// </summary>
		public bool DoProcessRequires{
			get { return _config.DoProcessRequires; }
		}

		/// <summary>
		///     Возвращает конфигурацию компилятора
		/// </summary>
		/// <returns></returns>
		public IBSharpConfig GetConfig(){
			if (null == _config){
				_config = new BSharpConfig();
			}
			return _config;
		}

		/// <summary>
		///     Возвращает условия компиляции
		/// </summary>
		/// <returns></returns>
		public IConfig GetConditions(){
			return new ConfigBase(_config.Conditions);
		}

		/// <summary>
		///     Выполняет расширения
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="context"></param>
		/// <param name="phase"></param>
		public void CallExtensions(IBSharpClass cls, IBSharpContext context, BSharpCompilePhase phase){
			if (0 == Extensions.Count) return;
			foreach (IBSharpCompilerExtension extension in Extensions){
				extension.Execute(this, context, cls, phase);
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="compilerConfig"></param>
		public void Initialize(IBSharpConfig compilerConfig){
			_config = compilerConfig;
			_global = _config.Global ?? new ConfigBase{UseInheritance = false};
			if (null != compilerConfig.Conditions){
				foreach (var cond in compilerConfig.Conditions){
					_global[cond.Key] = cond.Value;
				}
			}
		}


		/// <summary>
		///     Компилирует источники в перечисление итоговых классов
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="preparedContext"></param>
		/// <returns></returns>
		public IBSharpContext Compile(IEnumerable<XElement> sources, IBSharpContext preparedContext = null){
			IBSharpContext result = preparedContext ?? new BSharpContext(this);
			result.Compiler = this;
			sources = ProcessRequires(sources, result);
			Log.Info("Total Sources after require "+sources.Count());
			if (_config.SingleSource){
				return BuildBatch(sources, result);
			}
			foreach (XElement src in sources){
				IBSharpContext subresult = BuildSingle(src);
				result.Merge(subresult);
			}
			return result;
		}

		/// <summary>
		/// </summary>
		[Inject]
		public IBSharpSqlAdapter SqlAdapter{
			get { return _sqlAdapter ?? (_sqlAdapter = new BSharpSqlAdapter()); }
			set { _sqlAdapter = value; }
		}

		/// <summary>
		/// </summary>
		public IConfig Global{
			get { return _global; }
		}

		/// <summary>
		///     Считать исходный
		/// </summary>
		/// <param name="e"></param>
		/// <param name="ns"></param>
		/// <param name="aliases"></param>
		/// <returns></returns>
		public IBSharpClass ReadSingleClassSource(XElement e, string ns, IDictionary<string, string> aliases){
			bool anonym = false;
			string selfcode = e.Attr("code");
			if (string.IsNullOrWhiteSpace(selfcode)){
				var clsbody = new XElement(e);
				foreach (XElement e_ in clsbody.DescendantsAndSelf()){
					e_.SetAttributeValue("_file", null);
					e_.SetAttributeValue("_line", null);
				}

				selfcode = "cls_" + e.NoEvidenceCopy().ToString().MD5BasedDigitHash();
				e.SetAttributeValue("code", selfcode);
				anonym = true;
			}
			string __ns = ns ?? string.Empty;
			if (selfcode.Contains(".")){
				int lastdot = selfcode.LastIndexOf('.');
				string _nsadd = selfcode.Substring(0, lastdot);
				selfcode = selfcode.Substring(lastdot + 1);
				e.SetAttributeValue("code", selfcode);
				e.SetAttributeValue("id", selfcode);
				if (string.IsNullOrWhiteSpace(__ns)){
					__ns = _nsadd;
				}
				else{
					__ns = __ns + "." + _nsadd;
				}
			}
			var def = new BSharpClass(CurrentBuildContext){Source = e, Name = selfcode, Namespace = __ns};
			if (null != def.Source.Attribute("_file")){
				def.Source.SetAttributeValue("_dir", Path.GetDirectoryName(def.Source.Attr("_file")).Replace("\\", "/"));
			}
			if (anonym){
				def.Set(BSharpClassAttributes.Anonymous);
			}
			if (def.Source.GetSmartValue(BSharpSyntax.ExplicitClassMarker).ToBool()){
				def.Set(BSharpClassAttributes.ExplicitElements);
			}
			if (def.Source.GetSmartValue(BSharpSyntax.PartialClass).ToBool()){
				def.Set(BSharpClassAttributes.Partial);
			}
			if (!IsOverrideMatch(def)) return null;

			SetupInitialOrphanState(e, def, aliases);
			ParseImports(e, def, aliases);
			ParseCompoundElements(e, def);
			ParseImplicitElements(e, def);
			ParseDefinitions(e, def);
			return def;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public static BSharpCompiler CreateDefault(){
			var cfg = new BSharpConfig{UseInterpolation = true, SingleSource = true, DoProcessRequires = true};
			var result = new BSharpCompiler();
			result.Initialize(cfg);
			return result;
		}


		/// <summary>
		///     Выполнить компиляцию исходного кода
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IBSharpContext Compile(IEnumerable<XElement> sources, IBSharpConfig config = null){
			BSharpCompiler compiler = null == config ? CreateDefault() : new BSharpCompiler();
			if (null != config){
				compiler.Initialize(config);
			}
			return compiler.Compile(sources, (IBSharpContext) null);
		}

		/// <summary>
		///     Асинхронно выполнить компиляцию кода
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static async Task<IBSharpContext> CompileAsync(IEnumerable<XElement> sources, IBSharpConfig config = null){
			return await Task.Run(() => Compile(sources, config));
		}

		/// <summary>
		///     Скомпилировать отдельный XElement
		/// </summary>
		/// <param name="e"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IBSharpContext Compile(XElement e, IBSharpConfig config = null){
			return Compile(new[]{e}, config);
		}

		/// <summary>
		///     Компилирует директорию
		/// </summary>
		/// <param name="dirname"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IBSharpContext CompileDirectory(string dirname, IBSharpConfig config = null){
			var parser = new BxlParser();
			IEnumerable<XElement> sources =
				Directory.GetFiles(dirname, "*.bxls", SearchOption.AllDirectories).Select(_ => parser.Parse(File.ReadAllText(_), _));
			return Compile(sources, config);
		}

		/// <summary>
		///     Асинхронная компиляция отдельного XElement
		/// </summary>
		/// <param name="e"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static async Task<IBSharpContext> CompileAsync(XElement e, IBSharpConfig config = null){
			return await Task.Run(() => Compile(e, config));
		}


		/// <summary>
		///     Скомпилировать код на BXL
		/// </summary>
		/// <param name="bxl"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IBSharpContext Compile(string bxl, IBSharpConfig config = null){
			return Compile(new[]{GetXml(bxl, 0)}, config);
		}

		/// <summary>
		///     Скомпилировать код на BXL
		/// </summary>
		/// <param name="bxl"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static async Task<IBSharpContext> CompileAsync(string bxl, IBSharpConfig config = null){
			return await Task.Run(() => Compile(bxl, config));
		}


		/// <summary>
		///     Скомпилировать набор BXL
		/// </summary>
		/// <param name="bxls"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IBSharpContext Compile(IEnumerable<string> bxls, IBSharpConfig config = null){
			IEnumerable<XElement> xmls = bxls.Select(GetXml);
			return Compile(xmls, config);
		}

		private static XElement GetXml(string _, int i){
			var bxl = new BxlParser();
			if (_.Length <= 255){
				IList<string> lines = _.SmartSplit(false, true, '\r', '\n');
				if (lines.Count == 1){
					try{
						if (File.Exists(_)){
							string fullpath = Path.GetFullPath(_);
							return bxl.Parse(File.ReadAllText(fullpath), fullpath);
						}
					}
					catch{
					}
				}
			}
			return bxl.Parse(_, "code" + i + ".bxl");
		}

		/// <summary>
		/// </summary>
		/// <param name="bxls"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static async Task<IBSharpContext> CompileAsync(IEnumerable<string> bxls, IBSharpConfig config = null){
			return await Task.Run(() => Compile(bxls, config));
		}

		private IEnumerable<XElement> ProcessRequires(IEnumerable<XElement> sources, IBSharpContext context){
			if (DoProcessRequires){
				return GetSourcesWithRequireProcessing(sources, context);
			}
			return GetSourcesWithRequireIgnorance(sources, context);
		}

		private IEnumerable<XElement> GetSourcesWithRequireIgnorance(IEnumerable<XElement> sources, IBSharpContext context){
			foreach (XElement src in sources){
				XElement[] requires = src.Elements(BSharpSyntax.Require).ToArray();
				if (requires.Length != 0){
					requires.Remove();
					string message = "requre options in " + src.Describe().File + " ignored";
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
			Dictionary<string, XElement> filenames =
				sources.ToDictionary(_ => Path.GetFullPath(_.Describe().File).NormalizePath(), _ => _);
		    foreach (var basefiles in filenames.ToArray()) {
		        ProcessRequires(basefiles.Value, basefiles.Key, filenames, context);
		    }
			return filenames.Values.ToArray();
		}

		private void ProcessRequires(XElement source, string filename, Dictionary<string, XElement> filenames,
		                             IBSharpContext context){
			XElement[] requires = source.Elements(BSharpSyntax.Require).ToArray();
			foreach (XElement require in requires){
				string name = require.GetCode();
				if (filenames.ContainsKey(name)) continue;
				IBSharpSourceCodeProvider pkgservice = ResolvePackage(name);
				if (null != pkgservice){
					ProcessRequiresWithSourcePackage(filenames, name, pkgservice);
				}
				else{
					ProcessRequiresWithFileReference(source, filenames, context, require, filename);
				}
			}
			requires.Remove();
		}

		private void ProcessRequiresWithFileReference(XElement source, Dictionary<string, XElement> filenames,
		                                              IBSharpContext context, XElement require, string filename){
			
			if (require.Attr("code").EndsWith("/") || require.Attr("code").Contains("*")){
				ProcessRequiresDirectory(source, filenames, context, require, filename);
				return;
			}
         
			string dir = Path.GetDirectoryName(filename);
		    string file = require.Attr("code");

			if (!Path.IsPathRooted(file)){
			    if (file.Contains("@")) {
			        file = EnvironmentInfo.ResolvePath(file);
			    }
			    else {
			        file = Path.GetFullPath(Path.Combine(dir, file)).NormalizePath();
			    }
			}
		    if (!File.Exists(file)) {
                file +=".bxls";
		    }
           

			if (filenames.ContainsKey(file)) return;
			if (File.Exists(file)){
				XElement src = _requireBxl.Parse(File.ReadAllText(file), file);
				filenames[file] = src;
				ProcessRequires(src, file, filenames, context);
			}
			else{
				string message = "cannot  find required module " + require.Attr("code") + " for " + source.Describe().File;
				context.RegisterError(new BSharpError{
					Level = ErrorLevel.Error,
					Phase = BSharpCompilePhase.SourceIndexing,
					Message = message,
					Xml = require
				});
				log.Error(message);
			}
		}

		private void ProcessRequiresDirectory(XElement source, Dictionary<string, XElement> filenames, IBSharpContext context, XElement require, string filename){
			string curdir = Path.GetDirectoryName(filename);
			string otherdir = require.Attr("code");
		    var mask = "*.bxls";
		    if (otherdir.Contains("*")) {
		        mask = Path.GetFileName(otherdir);
		        otherdir = Path.GetDirectoryName(otherdir);
		    }
		    if (otherdir.Contains("@")) {
		        otherdir = EnvironmentInfo.ResolvePath(otherdir);
		    }
			if (!Path.IsPathRooted(otherdir)){
				otherdir = Path.GetFullPath(Path.Combine(curdir, otherdir)).NormalizePath();
			}
			if (Directory.Exists(otherdir)) {
				foreach (var file in Directory.GetFiles(otherdir,mask)){
					if (filenames.ContainsKey(file)) continue;
	
						XElement src = _requireBxl.Parse(File.ReadAllText(file), file);
						filenames[file] = src;
						ProcessRequires(src, file, filenames, context);
				}
			}
			else{
				string message = "cannot  find required directory " + require.Attr("code") + " for " + source.Describe().File;
				context.RegisterError(new BSharpError
				{
					Level = ErrorLevel.Error,
					Phase = BSharpCompilePhase.SourceIndexing,
					Message = message,
					Xml = require
				});
				log.Error(message);
			}
		}

		private void ProcessRequiresWithSourcePackage(Dictionary<string, XElement> filenames, string name,
		                                              IBSharpSourceCodeProvider pkgservice){
			filenames[name] = new XElement("stub");
			foreach (XElement element in pkgservice.GetSources(this, null).ToArray()){
				string fn = element.Describe().File;
				if (!filenames.ContainsKey(fn)){
					filenames[fn] = element;
				}
			}
		}

		private IBSharpSourceCodeProvider ResolvePackage(string name){
			IBSharpSourceCodeProvider pkgservice = null;
			//some well knowns
			if (name == "data"){
				pkgservice =
					Type.GetType("Qorpent.Scaffolding.Model.Compiler.DataObjectsSourcePackageForBShart, Qorpent.Scaffolding", false)
					    .Create<IBSharpSourceCodeProvider>();
			}
			else if (name == "app"){
				pkgservice =
					Type.GetType("Qorpent.Scaffolding.Application.AppSpecificationBSharpSource, Qorpent.Scaffolding", false)
					    .Create<IBSharpSourceCodeProvider>();
			}

			else if (name == "preprocessor"){
				pkgservice =
					Type.GetType("Qorpent.BSharp.Preprocessor.PreprocessorSourcePackageForBSharp, Qorpent.Serialization", false)
					    .Create<IBSharpSourceCodeProvider>();
			}
			if (null == pkgservice){
				//fallback to IoC resolution of pkg
				pkgservice = ResolveService<IBSharpSourceCodeProvider>(name + ".bssrcpkg");
			}
			return pkgservice;
		}

		private IBSharpContext BuildSingle(XElement source){
			var batch = new[]{source};
			IBSharpContext context = Build(batch);
			return context;
		}

		private IBSharpContext Build(XElement[] batch){
			IBSharpContext context = BuildIndex(batch);
			CompileClasses(batch, context);
			LinkClasses(batch, context);
			ExecuteDefinitions(batch, context);
			return context;
		}

		private void ExecuteDefinitions(XElement[] batch, IBSharpContext context){
			context.Get(BSharpContextDataType.Working).Where(_ => 0 != _.AllEvaluations.Count()).AsParallel().ForAll(_ =>{
				try{
					BSharpClassBuilder.Build(BuildPhase.Evaluate, this, _, context);
				}
				catch (Exception ex){
					_.Error = ex;
				}
			});
		}

		private IBSharpContext BuildBatch(IEnumerable<XElement> sources, IBSharpContext preparedContext){
			XElement[] batch = sources.ToArray();
			IBSharpContext context = Build(batch);
			if (null != preparedContext){
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
		protected virtual IBSharpContext BuildIndex(IEnumerable<XElement> sources){
			CurrentBuildContext = new BSharpContext(this);
			var baseindex = IndexizeRawClasses(sources).ToArray();
		    SetupDefaultNamespace(baseindex);
		    SetupGlobals();
			CurrentBuildContext.Setup(baseindex);
			CurrentBuildContext.ExecuteGenerators();
			CurrentBuildContext.Build();
			return CurrentBuildContext;
		}

	    private void SetupDefaultNamespace(IBSharpClass[] baseindex) {
	        var defaultNamespace = GetConfig().DefaultNamespace ?? "";
	        foreach (var cls in baseindex) {
	            if (string.IsNullOrWhiteSpace(cls.Namespace) && !string.IsNullOrWhiteSpace(defaultNamespace)) {
	                cls.Namespace = defaultNamespace;
	            }
	            else if (cls.Namespace.StartsWith(".")) {
	                if (string.IsNullOrWhiteSpace(defaultNamespace)) {
	                    cls.Namespace = cls.Namespace.Substring(1);
	                }
	                else {
	                    cls.Namespace = defaultNamespace + cls.Namespace;
	                }
	            }
	        }
	    }

	    private void SetupGlobals(){
			bool requireInterpolation = false;
			foreach (var baseglobal in _overlobals){
				if (!_global.ContainsKey(baseglobal.Key)){
					requireInterpolation = requireInterpolation || baseglobal.Value.Contains("~{");
					_global[baseglobal.Key] = baseglobal.Value;
				}
			}
			foreach (var baseglobal in _baseglobals){
				if (!_global.ContainsKey(baseglobal.Key)){
					requireInterpolation = requireInterpolation || baseglobal.Value.Contains("~{");
					_global[baseglobal.Key] = baseglobal.Value;
				}
			}

			foreach (var baseglobal in _extlobals){
				if (!_global.ContainsKey(baseglobal.Key)){
					requireInterpolation = requireInterpolation || baseglobal.Value.Contains("~{");
					_global[baseglobal.Key] = baseglobal.Value;
				}
			}
			if (requireInterpolation){
				var si = new StringInterpolation{AncorSymbol = '~'};
				bool haschanges = true;
				while (haschanges){
					haschanges = false;
					foreach (var current in _global.Where(_ => _.Value is string && ((string) _.Value).Contains("~{")).ToArray()){
						string newval = si.Interpolate((string) current.Value, _global);
						if (newval != (string) current.Value){
							_global[current.Key] = newval;
							haschanges = true;
						}
					}
				}
			}
		}

		private IEnumerable<IBSharpClass> IndexizeRawClasses(IEnumerable<XElement> sources){
			var buffer = new ConcurrentBag<IBSharpClass>();
		    sources.AsParallel().ForAll(src =>{
				Preprocess(src);
				foreach (IBSharpClass e in IndexizeRawClasses(src, "")){
					buffer.Add(e);
				}
			});
			return buffer;
		}

		private void Preprocess(XElement src){
			XElement[] sets = src.Descendants("set").Reverse().ToArray();

			foreach (XElement s in sets){
				XElement[] subelements = s.Elements().ToArray();
				foreach (XAttribute a in s.Attributes()){
					foreach (XElement sb in subelements){
						if (null == sb.Attribute(a.Name)){
							sb.SetAttributeValue(a.Name, a.Value);
						}
					}
				}
				s.ReplaceWith(subelements);
			}
		}

		private IEnumerable<IBSharpClass> IndexizeRawClasses(XElement src, string ns){
			var aliases = new Dictionary<string, string>();
			string rns = ns;
			foreach (XElement e in src.Elements()){
				string _ns = "";
				if (null != _config.IgnoreElements && 0 != _config.IgnoreElements.Length){
					if (-1 != Array.IndexOf(_config.IgnoreElements, e.Name.LocalName)){
						continue;
					}
				}


				if (e.Name.LocalName == BSharpSyntax.Namespace){
					string ifa = e.Attr("if");
					if (!string.IsNullOrWhiteSpace(ifa)){
						var terms = new DictionaryTermSource<object>(GetConditions());
						if (!eval.Eval(ifa, terms)){
							continue;
						}
					}
					if (null == e.Parent.Parent && !e.Elements().Any()){
						//plain namespace definition
						rns = e.Attr("code");
					}
					else{
						rns = ns;
					}
					if (string.IsNullOrWhiteSpace(rns)){
						_ns = e.Attr("code");
					}
					else{
						_ns = rns + "." + e.Attr("code");
					}
					foreach (IBSharpClass e_ in IndexizeRawClasses(e, _ns)){
						yield return e_;
					}
				}

				else if (e.Name.LocalName == BSharpSyntax.AliasImport){
					foreach (XAttribute attr in e.Attributes()){
						if (-1 == Array.IndexOf(ignores, attr.Name.LocalName)){
							aliases[attr.Name.LocalName] = attr.Value;
						}
					}
				}
				else if (e.Name.LocalName == BSharpSyntax.ConstantDefinition ||
				         e.Name.LocalName == BSharpSyntax.ConstantOverrideDefinition ||
				         e.Name.LocalName == BSharpSyntax.ConstantDefaultDefinition){
					PrepareGlobals(e);
				}

				else if (e.Name.LocalName == BSharpSyntax.Require){
					continue;
				}
				else if (e.Name.LocalName == BSharpSyntax.Dataset){
					var def = new BSharpClass(CurrentBuildContext){
						Source = e,
						Name = BSharpSyntax.DatasetClassCodePrefix + e.Attr("code"),
						Namespace = rns ?? string.Empty
					};
					def.Set(BSharpClassAttributes.Dataset);
					yield return def;
				}
				else if (e.Name.LocalName == BSharpSyntax.Generator){
					yield return PrepareGenerator(rns, e);
				}
				else if (e.Name.LocalName == BSharpSyntax.Connection){
					yield return PrepareConnection(rns, e);
				}
				else if (e.Name.LocalName == BSharpSyntax.Template){
					yield return PrepareTemplate(rns, e);
				}
				else{
					IBSharpClass def = ReadSingleClassSource(e, rns, aliases);
					if (null != def) yield return def;
				}
			}
		}

		private void PrepareGlobals(XElement src){
			IDictionary<string, string> target = _baseglobals;
			if (src.Name.LocalName.StartsWith(XmlName.Commons['~'])){
				target = _overlobals;
			}
			else if (src.Name.LocalName.StartsWith(XmlName.Commons['+'])){
				target = _extlobals;
			}
			foreach (XAttribute a in src.Attributes()){
				if (a.Name.LocalName == "code") continue;
				if (a.Name.LocalName == "id") continue;
				if (a.Name.LocalName == "name") continue;
				if (a.Name.LocalName == "_line") continue;
				if (a.Name.LocalName == "_file") continue;
				if (target == _baseglobals){
					if (target.ContainsKey(a.Name.LocalName)){
						CurrentBuildContext.RegisterError(BSharpErrors.DoubleConstantDefinition(src, a));
						continue;
					}
				}
				target[a.Name.LocalName] = a.Value;
			}
		}


		private IBSharpClass PrepareTemplate(string ns, XElement e){
			string _code = e.Attr(BSharpSyntax.ClassNameAttribute);
			string code = BSharpSyntax.GenerateTemplateClassName(_code);
			e.SetAttr(BSharpSyntax.ConnectionCodeAttribute, e.Attr("code"));
			e.SetAttr(BSharpSyntax.ClassNameAttribute, code);
			e.SetAttr(BSharpSyntax.TemplateValueAttribute, e.Value);
			var def = new BSharpClass(CurrentBuildContext){Source = e, Name = code, Namespace = ns ?? string.Empty};
			def.Set(BSharpClassAttributes.Template);
			return def;
		}

		private IBSharpClass PrepareConnection(string ns, XElement e){
			string mode = e.Attr(BSharpSyntax.ConnectionModeAttribute, "sql");
			string _code = e.Attr(BSharpSyntax.ClassNameAttribute);
			string code = BSharpSyntax.GenerateConnectionClassName(mode, _code);
			e.SetAttr(BSharpSyntax.ConnectionCodeAttribute, e.Attr("code"));
			e.SetAttr(BSharpSyntax.ClassNameAttribute, code);
			e.SetAttr(BSharpSyntax.ConnectionStringAttribute, e.Value);
			var def = new BSharpClass(CurrentBuildContext){Source = e, Name = code, Namespace = ns ?? string.Empty};
			def.Set(BSharpClassAttributes.Connection);
			return def;
		}

		private IBSharpClass PrepareGenerator(string ns, XElement e){
			string code = "generator_" + e.NoEvidenceCopy().ToString().MD5BasedDigitHash();
			e.SetAttributeValue(BSharpSyntax.GeneratorClassCodeAttribute, e.Attr("code"));
			e.SetAttributeValue(BSharpSyntax.GeneratorClassNameAttribute, e.Attr("name"));
			e.SetAttributeValue("code", code);
			var def = new BSharpClass(CurrentBuildContext){Source = e, Name = code, Namespace = ns ?? string.Empty};
			def.Set(BSharpClassAttributes.Generator);
			return def;
		}

		private void ParseImplicitElements(XElement el, BSharpClass def){
			if (def.Is(BSharpClassAttributes.ExplicitElements)) return;
			var candidates = new Dictionary<string, IList<string>>();
			List<string> exists =
				def.SelfElements.Select(_ => _.TargetName).Union(def.SelfElements.Select(_ => _.Name)).Distinct().ToList();
			foreach (XElement e in el.Elements()){
				string n = e.Name.LocalName;
				bool over = false;
				bool ext = false;
				if (n.StartsWith("__PLUS__") || n.StartsWith("__TILD__")){
					if (n.StartsWith("__PLUS__")){
						over = true;
					}
					else{
						ext = true;
					}
					n = n.Substring(8);
				}
				if (n == BSharpSyntax.ClassElementDefinition) continue;
				if (n == BSharpSyntax.ClassImportDefinition) continue;
				if (!over && !ext){
					if (exists.Contains(n)) continue;
				}
				if (!candidates.ContainsKey(n)){
					candidates[n] = new List<string>();
				}
				if (null == candidates[n]) continue;
				string code = e.GetCode();
				if (string.IsNullOrWhiteSpace(code)){
					candidates[n] = null;
					continue;
				}
				if (!(over || ext)){
					if (candidates[n].Contains(code)){
						candidates[n] = null;
						continue;
					}
				}

				candidates[n].Add(code);
			}
			foreach (string candidate in candidates.Where(_ => _.Value != null).Select(_ => _.Key).ToArray()){
				var edef = new BSharpElement();
				edef.Name = candidate;
				edef.TargetName = candidate;
				edef.Type = BSharpElementType.Define;
				edef.Implicit = true;
				def.SelfElements.Add(edef);
			}
		}

		private void ParseDefinitions(XElement e, BSharpClass def){
			foreach (XElement i in e.Elements(BSharpSyntax.ClassEvaluateDefinition)){
				def.SelfEvaluations.Add(new BSharpEvaluation{Source = i});
			}
		}

		private bool IsOverrideMatch(BSharpClass def){
			if (def.Source.Name.LocalName == BSharpSyntax.ClassOverrideKeyword ||
			    def.Source.Name.LocalName == BSharpSyntax.ClassExtensionKeyword){
				string ifa = def.Source.Attr("if");
				if (!string.IsNullOrWhiteSpace(ifa)){
					def.Source.Attribute("if").Remove();
					var terms = new DictionaryTermSource<object>(GetConditions());
					return eval.Eval(ifa, terms);
				}
			}
			return true;
		}

		private void SetupInitialOrphanState(XElement e, IBSharpClass def, IDictionary<string, string> aliases){
			if (null != e.Attribute(BSharpSyntax.ClassAbstractModifier) || e.Attr("name") == BSharpSyntax.ClassAbstractModifier){
				def.Set(BSharpClassAttributes.Abstract);
			}


			if (null != e.Attribute(BSharpSyntax.ClassStaticModifier) || e.Attr("name") == BSharpSyntax.ClassStaticModifier){
				def.Set(BSharpClassAttributes.Static);
			}

			if (null != e.Attribute(BSharpSyntax.ClassGenericModifier) || e.Attr("name") == BSharpSyntax.ClassGenericModifier){
				def.Set(BSharpClassAttributes.Generic);
				def.Set(BSharpClassAttributes.Abstract);
				def.Set(BSharpClassAttributes.Static);
			}
			if (e.Name.LocalName == BSharpSyntax.Class){
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
			if (e.Name.LocalName == BSharpSyntax.PatchClassKeyword){
				def.Set(BSharpClassAttributes.Explicit);
				def.Set(BSharpClassAttributes.Patch);
				def.Set(BSharpClassAttributes.Embed);
			}
			else if (e.Name.LocalName == BSharpSyntax.ClassOverrideKeyword){
				def.Set(BSharpClassAttributes.Override);
			}
			else if (e.Name.LocalName == BSharpSyntax.ClassExtensionKeyword){
				def.Set(BSharpClassAttributes.Extension);
			}
			else{
				def.Set(BSharpClassAttributes.Orphan);
				def.DefaultImportCode = e.Name.LocalName;
				if (null != aliases && aliases.ContainsKey(def.DefaultImportCode)){
					def.AliasImportCode = def.DefaultImportCode;
					def.DefaultImportCode = aliases[def.DefaultImportCode];
				}
			}
		}

		private static void ParseImports(XElement e, IBSharpClass def, IDictionary<string, string> aliases){
			foreach (XElement i in e.Elements("import")){
				var import = new BSharpImport{Condition = i.Attr("if"), TargetCode = i.Attr("code"), Source = i};
				if (null != aliases && aliases.ContainsKey(import.TargetCode)){
					import.Alias = import.TargetCode;
					import.TargetCode = aliases[import.TargetCode];
				}
				def.SelfImports.Add(import);
			}
		}

		private static void ParseCompoundElements(XElement e, IBSharpClass def){
			foreach (XElement i in e.Elements(BSharpSyntax.ClassElementDefinition)){
				var merge = new BSharpElement();
				merge.Name = i.Attr("code");
				merge.TargetName = i.Attr("code");
				merge.Type = BSharpElementType.Define;
				if (i.Attribute("override") != null){
					merge.Type = BSharpElementType.Override;
					merge.TargetName = i.Attr("override");
				}
				else if (null != i.Attribute("extend")){
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
		protected virtual void CompileClasses(IEnumerable<XElement> sources, IBSharpContext context){
			foreach (var cls in context.RawClasses){
				cls.Value.PrepareForCompilation();
			}
			context.MetaClasses.Values.Where(_ => _.Is(BSharpClassAttributes.Patch) && _.PatchPhase == BSharpPatchPhase.Before)
			       .OrderBy(_ => _.Priority)
			       .Select(
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
				context.RegisterError(BSharpErrors.Generic(ex, ErrorLevel.Error));
				_.Error = ex;
			}
		}

		/// <summary>
		///     Перекрыть для создания линковщика
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		protected virtual void LinkClasses(IEnumerable<XElement> sources, IBSharpContext context){
			context.BuildLinkingIndex();
			bool requirelink = context.RequireLinking();
			bool requirepostprocess = context.RequrePostProcess();
			context.MetaClasses.Values.Where(
				_ => _.Is(BSharpClassAttributes.Patch) && _.PatchPhase == BSharpPatchPhase.AfterBuild)
			       .OrderBy(_ => _.Priority)
			       .Select(
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

			context.MetaClasses.Values.Where(_ => _.Is(BSharpClassAttributes.Patch) && _.PatchPhase == BSharpPatchPhase.After)
			       .OrderBy(_ => _.Priority)
			       .Select(
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


			if (requirepostprocess){
				context.Get(BSharpContextDataType.Working)
				       .AsParallel()
				       .ForAll(_ => BSharpClassBuilder.Build(BuildPhase.PostProcess, this, _, context));
			}
		}
	}
}