using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.LogicalExpressions;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.LogicalExpressions;

namespace Qorpent.BSharp{
	/// <summary>
	///     Внутренний индекс компилятора
	/// </summary>
	public class BSharpContext : Scope, IBSharpContext{
		private const string RAWCLASSES = "rawclasses";
		private const string METACLASSES = "metaclasses";
		private const string ORPHANED = "orphaned";
		private const string ABSTRACTS = "abstracts";
		private const string WORKING = "working";
		private const string STATIC = "static";
		private const string OVERRIDES = "overrides";
		private const string EXTENSIONS = "extensions";
		private const string IGNORED = "ignored";
		private const string ERRORS = "errors";
		private const string EXPORTS = "exports";
		private const string PROTOTYPE_MAP = "prototypemap";
		private static int autogenIndex;

		private readonly Dictionary<string, IList<IBSharpClass>> PartialsRegistry =
			new Dictionary<string, IList<IBSharpClass>>();

		private readonly IDictionary<string, string> _resolveDictCache = new Dictionary<string, string>();
		private readonly IDictionary<string, IBSharpClass> _resolveclassCache = new Dictionary<string, IBSharpClass>();
		private readonly XmlInterpolation genInt = new XmlInterpolation();
		private readonly StringInterpolation templater = new StringInterpolation();
		private bool _built;

		/// <summary>
		/// </summary>
		public BSharpContext(IBSharpCompiler compiler = null){
			Compiler = compiler;
			ExtendedData = new Dictionary<string, object>();
		}


		/// <summary>
		///     Классы с непроинициализированным наследованием
		/// </summary>
		public List<IBSharpClass> Orphans{
			get { return Get<List<IBSharpClass>>(ORPHANED); }
			set { Set(ORPHANED, value); }
		}

		/// <summary>
		///     Классы с непроинициализированным наследованием
		/// </summary>
		public List<IBSharpClass> Abstracts{
			get { return Get<List<IBSharpClass>>(ABSTRACTS); }
			set { Set(ABSTRACTS, value); }
		}

		/// <summary>
		///     Классы с непроинициализированным наследованием
		/// </summary>
		public List<IBSharpClass> Working{
			get { return Get<List<IBSharpClass>>(WORKING); }
			set { Set(WORKING, value); }
		}

		/// <summary>
		///     Классы со статической компиляцией
		/// </summary>
		public List<IBSharpClass> Static{
			get { return Get<List<IBSharpClass>>(STATIC); }
			set { Set(STATIC, value); }
		}

		/// <summary>
		///     Реестр перезагрузок классов
		/// </summary>
		public List<IBSharpClass> Overrides{
			get { return Get<List<IBSharpClass>>(OVERRIDES); }
			set { Set(OVERRIDES, value); }
		}

		/// <summary>
		///     Реестр перезагрузок классов
		/// </summary>
		public List<IBSharpClass> Extensions{
			get { return Get<List<IBSharpClass>>(EXTENSIONS); }
			set { Set(EXTENSIONS, value); }
		}

		/// <summary>
		///     Реестр перезагрузок классов
		/// </summary>
		public List<BSharpError> Errors{
			get { return Get<List<BSharpError>>(ERRORS); }
			set { Set(ERRORS, value); }
		}

		/// <summary>
		///     Реестр перезагрузок классов
		/// </summary>
		public IDictionary<string, IBSharpClass[]> PrototypeMap{
			get { return Get<IDictionary<string, IBSharpClass[]>>(PROTOTYPE_MAP); }
			set { Set(PROTOTYPE_MAP, value); }
		}

		/// <summary>
		/// </summary>
		public IDictionary<string, IList<ExportRecord>> Dictionaries{
			get { return Get<IDictionary<string, IList<ExportRecord>>>(EXPORTS); }
			set { Set(EXPORTS, value); }
		}

		/// <summary>
		///     Исходные сырые определения классов
		/// </summary>
		public IDictionary<string, IBSharpClass> RawClasses{
			get{
				var result = Get<IDictionary<string, IBSharpClass>>(RAWCLASSES);
				if (null == result){
					Set(RAWCLASSES, result = new Dictionary<string, IBSharpClass>());
				}
				return result;
			}
			set { Set(RAWCLASSES, value); }
		}


		/// <summary>
		///     Определения  для псеавдоклассов
		/// </summary>
		public IDictionary<string, IBSharpClass> MetaClasses{
			get{
				var result = Get<IDictionary<string, IBSharpClass>>(METACLASSES);
				if (null == result){
					Set(METACLASSES, result = new Dictionary<string, IBSharpClass>());
				}
				return result;
			}
			set { Set(METACLASSES, value); }
		}

		/// <summary>
		/// </summary>
		public IDictionary<string, object> ExtendedData { get; private set; }

		/// <summary>
		///     Обертка для поиска классов
		/// </summary>
		/// <returns></returns>
		public IBSharpClass this[string name, string ns = null]{
			get { return Get(name, ns: ns); }
		}

		/// <summary>
		///     Реестр перезагрузок классов
		/// </summary>
		public List<IBSharpClass> Ignored{
			get { return Get<List<IBSharpClass>>(IGNORED); }
			set { Set(IGNORED, value); }
		}

		/// <summary>
		///     Строит индекс словарей
		/// </summary>
		public void BuildLinkingIndex(){
			PrototypeMap = Working.Where(_ => !string.IsNullOrWhiteSpace(_.Prototype))
			                      .GroupBy(_ => _.Prototype).ToDictionary(_ => _.Key, _ => _.ToArray());
			BuildDictionaryIndex();
		}

        /// <summary>
        /// Выполняет загрузку индексов при ручной комплектации контекста
        /// </summary>
	    public void BuildIndexes() {
            PrototypeMap = Working.GroupBy(_ => _.Prototype).ToDictionary(_ => _.Key, _=>_.ToArray());
        }

		/// <summary>
		///     Специальная индексация для модификатора all
		/// </summary>
		/// <param name="query"></param>
		/// <param name="ns"></param>
		/// <param name="usemeta"></param>
		/// <returns></returns>
		public IEnumerable<IBSharpClass> ResolveAll(string query, string ns = null, bool usemeta = false){
			if (query.StartsWith("^")){
				query = query.Substring(1);
			}
			if (query.Contains(";")){
				IList<string> subqueries = query.SmartSplit(false, true, ';');
				return subqueries.SelectMany(_ => ResolveAll(_, ns)).Distinct();
			}
			if (query.StartsWith("attr:")){
				string attrname = query.Substring(5);
				return Working.Where(_ => _.Compiled.GetSmartValue(attrname).ToBool());
			}

			if (string.IsNullOrWhiteSpace(query)){
				return Working;
			}
            
			if (query.EndsWith(".*")){
				return Working.Where(_ => _.Namespace.StartsWith(query.Substring(0, query.Length - 2)));
			}
			if (null != PrototypeMap && query.SmartSplit(false, true, '|').Any(_ => PrototypeMap.ContainsKey(_))) {
				return query.SmartSplit(false, true, '|').SelectMany(_ => {
				    if (PrototypeMap.ContainsKey(_)) {
				        return PrototypeMap[_];
				    }
				    return new BSharpClass[] {};
				}).ToArray();
			}
			if (null != Dictionaries && Dictionaries.ContainsKey(query)){
				return Dictionaries[query].Select(_ => _.cls);
			}


			IBSharpClass basetype = Get(query, ns: ns);

			if (null != basetype){
				List<IBSharpClass> result = Working.Where(_ => _.AllImports.Contains(basetype)).ToList();
				if (!basetype.Is(BSharpClassAttributes.Abstract)){
					result.Add(basetype);
				}
				return result.ToArray();
			}

			if (PrototypeMap == null){
				return query.SmartSplit(false, true, '|').SelectMany(_ => RawClasses.Values.Where(__ => __.Source.Attr("prototype") == _)).ToArray();
			}
			else{
				return new IBSharpClass[]{};
			}
		}

		/// <summary>
		///     Выполняет генераторы, формируя дополнительные классы
		/// </summary>
		public void ExecuteGenerators(){
			IBSharpClass[] generators = MetaClasses.Values.Where(_ => _.Is(BSharpClassAttributes.Generator)).ToArray();
			foreach (IBSharpClass generator in generators){
				ExecuteGenerator(generator);
			}
		}

		/// <summary>
		///     Разрешает элементы в словаре
		/// </summary>
		/// <returns></returns>
		public string ResolveDictionary(string code, string element){
			if (null == Dictionaries) return null;
			string key = code + BSharpSyntax.ClassPathDelimiter + element;
			if (_resolveDictCache.ContainsKey(key)){
				return _resolveDictCache[key];
			}
			lock (this){
				string result = null;
				if (Dictionaries.ContainsKey(code)){
					foreach (ExportRecord e in Dictionaries[code]){
						result = e.Resolve(element);
						if (null != result) break;
					}
				}
				_resolveDictCache[key] = result;
				return result;
			}
		}

		/// <summary>
		/// </summary>
		public void ClearBuildTasks(){
			foreach (IBSharpClass c in RawClasses.Values){
				c.BuildTask = null;
			}
		}

		/// <summary>
		///     Компилятор
		/// </summary>
		public IBSharpCompiler Compiler { get; set; }

		/// <summary>
		///     Загружает исходные определения классов
		/// </summary>
		/// <param name="rawclasses"></param>
		public void Setup(IEnumerable<IBSharpClass> rawclasses){
			if (null == RawClasses){
				RawClasses = new Dictionary<string, IBSharpClass>();
			}
			if (null == Errors){
				Errors = new List<BSharpError>();
			}
			foreach (IBSharpClass cls in rawclasses){
				if (cls.Is(BSharpClassAttributes.Partial)){
					if (!PartialsRegistry.ContainsKey(cls.FullName)){
						PartialsRegistry[cls.FullName] = new List<IBSharpClass>();
					}
					PartialsRegistry[cls.FullName].Add(cls);
				}
				else{
					RegisterClassInIndex(cls);
				}
			}
			RegisterPartials();
		}

		/// <summary>
		///     Присоединяет и склеивается с другим результатом
		/// </summary>
		/// <param name="othercontext"></param>
		public void Merge(IBSharpContext othercontext){
			lock (this){
				var subresult = othercontext as BSharpContext;


				foreach (var p in subresult.RawClasses){
					RawClasses[p.Key] = p.Value;
				}
				foreach (var p in subresult.MetaClasses){
					MetaClasses[p.Key] = p.Value;
				}

				if (null != subresult.Abstracts){
					if (null == Abstracts){
						Abstracts = new List<IBSharpClass>();
					}
					foreach (IBSharpClass a in subresult.Abstracts){
						Abstracts.Add(a);
					}
				}
				if (null != subresult.Orphans){
					if (null == Orphans){
						Orphans = new List<IBSharpClass>();
					}
					foreach (IBSharpClass a in subresult.Orphans){
						Orphans.Add(a);
					}
				}
				if (null != subresult.Working){
					if (null == Working){
						Working = new List<IBSharpClass>();
					}
					foreach (IBSharpClass a in subresult.Working){
						Working.Add(a);
					}
				}

				if (null != subresult.Overrides){
					if (null == Overrides){
						Overrides = new List<IBSharpClass>();
					}
					foreach (IBSharpClass a in subresult.Overrides){
						Overrides.Add(a);
					}
				}

				if (null != subresult.Extensions){
					if (null == Extensions){
						Extensions = new List<IBSharpClass>();
					}
					foreach (IBSharpClass a in subresult.Extensions){
						Extensions.Add(a);
					}
				}

				if (null != subresult.Errors){
					if (null == Errors){
						Errors = new List<BSharpError>();
					}
					foreach (BSharpError a in subresult.Errors){
						Errors.Add(a);
					}
				}

				if (null != subresult.Dictionaries){
					Dictionaries = Dictionaries ?? new Dictionary<string, IList<ExportRecord>>();
					foreach (var p in subresult.Dictionaries){
						if (!Dictionaries.ContainsKey(p.Key)){
							Dictionaries[p.Key] = new List<ExportRecord>();
						}

						IList<ExportRecord> t = Dictionaries[p.Key];
						foreach (ExportRecord d in p.Value){
							if (!t.Contains(d)){
								t.Add(d);
							}
						}
					}
				}
				if (null != subresult.PrototypeMap){
					if (null == PrototypeMap){
						PrototypeMap = new Dictionary<string, IBSharpClass[]>();
					}
					foreach (var p in subresult.PrototypeMap){
						if (!PrototypeMap.ContainsKey(p.Key)){
							PrototypeMap[p.Key] = p.Value;
						}
						else{
							PrototypeMap[p.Key] = PrototypeMap[p.Key].Union(p.Value).Distinct().ToArray();
						}
					}
				}
			}
		}

		/// <summary>
		///     Разрешает класс по коду и заявленному пространству имен
		/// </summary>
		/// <param name="code"></param>
		/// <param name="ns"></param>
		/// <param name="usemeta"></param>
		/// <returns></returns>
		public IBSharpClass Get(string code, string ns = null, bool usemeta = false){
			if (code.StartsWith("^")){
				code = code.Substring(1);
			}
			IDictionary<string, IBSharpClass> trg = usemeta ? MetaClasses : RawClasses;
			if (null == ns){
				IBSharpClass full = Working.FirstOrDefault(_ => _.FullName == code);
				if (null != full) return full;
				return Working.FirstOrDefault(_ => _.Name == code);
			}
			string key = ns + BSharpSyntax.ClassPathDelimiter + code;
			if (_resolveclassCache.ContainsKey(key)){
				return _resolveclassCache[key];
			}
			IBSharpClass result = null;
			if (!String.IsNullOrWhiteSpace(code)){
				if (code.Contains(BSharpSyntax.ClassPathDelimiter)){
					if (trg.ContainsKey(code)){
						result = trg[code];
					}
				}
				else if (ns.Contains(BSharpSyntax.ClassPathDelimiter)){
					IList<string> nsparts = ns.SmartSplit(false, true, BSharpSyntax.ClassPathDelimiter);
					for (int i = nsparts.Count - 1; i >= -1; i--){
						if (i == -1){
							string probe = code;
							if (trg.ContainsKey(probe)){
								result = trg[probe];
								break;
							}
						}
						else{
							string probe = "";
							for (int j = 0; j <= i; j++){
								probe += nsparts[j] + BSharpSyntax.ClassPathDelimiter;
							}
							probe += code;
							if (trg.ContainsKey(probe)){
								result = trg[probe];
								break;
							}
						}
					}
				}
				else if (!String.IsNullOrWhiteSpace(ns)){
					string probe = ns + "." + code;
					if (trg.ContainsKey(probe)){
						result = trg[probe];
					}
				}
			}
			if (null == result){
				if (null != trg && trg.ContainsKey(code)){
					result = trg[code];
				}
			}

			if (null == result){
				IEnumerable<IBSharpClass> variants = trg.Values.Where(_ => ("." + _.FullName).EndsWith("." + code));
				if (variants.Count() == 1){
					result = variants.First();
				}
				else{
					variants = variants.Where(_ => _.Namespace.Contains(ns));
					if (variants.Count() == 1){
						result = variants.First();
					}
				}
			}

			if (null != result){
				_resolveclassCache[key] = result;
			}
			return result;
		}

		/// <summary>
		///     Возвращает коллекцию классов по типу классов
		/// </summary>
		/// <param name="datatype"></param>
		/// <returns></returns>
		public IEnumerable<IBSharpClass> Get(BSharpContextDataType datatype){
			switch (datatype){
				case BSharpContextDataType.Working:
					return Working;
				case BSharpContextDataType.Orphans:
					return Orphans;
				case BSharpContextDataType.Abstracts:
					return Abstracts;
				case BSharpContextDataType.Statics:
					return Static;
				case BSharpContextDataType.Overrides:
					return Overrides;
				case BSharpContextDataType.Extensions:
					return Extensions;
				case BSharpContextDataType.Ignored:
					return Ignored;
				case BSharpContextDataType.SrcPkg:
					return Working.Union(Abstracts).Union(Overrides).Union(Extensions).Distinct();
				case BSharpContextDataType.LibPkg:
					return Working.Where(_ => !_.Is(BSharpClassAttributes.Embed)).Distinct();
				case BSharpContextDataType.Errors:

				default:
					return new IBSharpClass[]{};
			}
		}

		/// <summary>
		///     Возвращает ошибки указанного уровня
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public IEnumerable<BSharpError> GetErrors(ErrorLevel level = ErrorLevel.None){
			if (null == Errors) yield break;
			foreach (BSharpError e in Errors.ToArray()){
				if (null == e) continue;
				if (e.Level >= level){
					yield return e;
				}
			}
		}

		/// <summary>
		///     Строит рабочий индекс классов
		/// </summary>
		public void Build(){
			if (_built) return;
			Overrides = RawClasses.Values.Where(
				_ => _.Is(BSharpClassAttributes.Override))
			                      .OrderBy(_ => _.Source.Attr(BSharpSyntax.PriorityAttribute).ToInt())
			                      .ToList();
			Extensions = RawClasses.Values.Where(
				_ => _.Is(BSharpClassAttributes.Extension))
			                       .OrderBy(_ => _.Source.Attr(BSharpSyntax.ClassExtensionPriorityAttribute).ToInt())
			                       .ToList();
			ApplyOverrides();
			ApplyExtensions();
			ResolveOrphans();
			ResolveIgnored();

			Orphans = RawClasses.Values.Where(_ => _.IsOrphaned).ToList();
			Ignored = RawClasses.Values.Where(_ => _.Is(BSharpClassAttributes.Ignored)).ToList();
			foreach (IBSharpClass o in Orphans){
				RegisterError(BSharpErrors.OrphanClass(o));
			}
			Abstracts =
				RawClasses.Values.Where(
					_ => _.Is(BSharpClassAttributes.Abstract) && !_.IsOrphaned && !_.Is(BSharpClassAttributes.Ignored)).ToList();
			Working = RawClasses.Values.Where(_ =>
			                                  !_.Is(BSharpClassAttributes.Extension) &&
			                                  !_.Is(BSharpClassAttributes.Override) &&
			                                  !_.Is(BSharpClassAttributes.Abstract) &&
			                                  !_.IsOrphaned &&
			                                  !_.Is(BSharpClassAttributes.Ignored)).ToList();
			Static =
				RawClasses.Values.Where(
					_ => _.Is(BSharpClassAttributes.Static) && !_.IsOrphaned && !_.Is(BSharpClassAttributes.Ignored)).ToList();

			ResolveImports();
			_built = true;
		}

		/// <summary>
		///     Регистрирует ошибку в контексте
		/// </summary>
		/// <param name="error"></param>
		public void RegisterError(BSharpError error){
			lock (this){
				if (null == Errors) Errors = new List<BSharpError>();
				Errors.Add(error);
			}
		}

		/// <summary>
		///     Определяет наличие словаря
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public bool HasDictionary(string code){
			return Dictionaries != null && Dictionaries.ContainsKey(code);
		}

		/// <summary>
		///     Проверяет необходимость линковки контекста в целом
		/// </summary>
		/// <returns></returns>
		public bool RequireLinking(){
			return Working.Any(_ => _.Is(BSharpClassAttributes.RequireLinking));
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public bool RequirePatching(){
			return MetaClasses.Values.Any(_ => _.Is(BSharpClassAttributes.Patch));
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public bool RequrePostProcess(){
			return Working.Any(_ => _.Compiled.Descendants(BSharpSyntax.PostProcessRemoveBefore).Any()
			                        || _.Compiled.Descendants(BSharpSyntax.PostProcessSelectElements).Any());
		}

		private void ExecuteGenerator(IBSharpClass generator){
			ExecuteGenerator(new XElement(generator.Source), generator.Namespace);
		}

		/// <summary>
		///     Возвращает строку соединения
		/// </summary>
		/// <param name="code"></param>
		/// <param name="mode"></param>
		/// <param name="ns"></param>
		/// <returns></returns>
		private string GetConnectionString(string code, string mode, string ns){
			IBSharpClass cls = GetConnection(mode, code, ns);
			if (null == cls) return null;
			return cls.Source.Attr(BSharpSyntax.ConnectionStringAttribute);
		}

		/// <summary>
		///     Возвращает класс соединения
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="code"></param>
		/// <param name="ns"></param>
		/// <returns></returns>
		private IBSharpClass GetConnection(string mode, string code, string ns){
			return Get(BSharpSyntax.GenerateConnectionClassName(mode, code), ns, true);
		}

		private void ExecuteGenerator(XElement generator, string ns){
			XElement[] datasets = generator.Elements(BSharpSyntax.Dataset).ToArray();
			string classCode = generator.Attr(BSharpSyntax.GeneratorClassCodeAttribute);
			string className = generator.Attr(BSharpSyntax.GeneratorClassNameAttribute);
			XAttribute ca = generator.Attribute(BSharpSyntax.GeneratorClassCodeAttribute);


			if (null != ca){
				ca.Remove();
			}
			XAttribute na = generator.Attribute(BSharpSyntax.GeneratorClassNameAttribute);
			if (null != na){
				na.Remove();
			}

			XElement[][] resolvedDatasets = datasets.Select(_ => GetDataSet(_, ns).ToArray()).ToArray();
			foreach (XElement dataset in datasets){
				dataset.Remove();
			}
			XElement[][] combinations = resolvedDatasets.Combine().ToArray();

			foreach (var elementSet in combinations){
				GenerateVariant(generator, ns, classCode, className, elementSet);
			}
		}

		private void GenerateVariant(XElement generator, string ns, string classCode, string className, XElement[] elementSet){
			var clselement = new XElement(BSharpSyntax.Class);
			string realcode = classCode;
			if (string.IsNullOrWhiteSpace(realcode) || !realcode.Contains("${")){
				if (string.IsNullOrWhiteSpace(realcode)){
					realcode = "auto_class_" + autogenIndex++;
				}
				else{
					realcode += "_" + autogenIndex++;
				}
			}
			clselement.SetAttributeValue("code", realcode);
			clselement.SetAttributeValue("name", className);
			foreach (XElement xElement in elementSet){
				foreach (XAttribute a in xElement.Attributes()){
					clselement.SetAttributeValue(a.Name, a.Value);
				}
			}
			genInt.Interpolate(clselement);
			foreach (XAttribute a in generator.Attributes()){
				if (a.Name != "code" && a.Name != "name" && a.Name != "id"){
					clselement.SetAttributeValue(a.Name, a.Value);
				}
			}

			foreach (XElement e in generator.Elements()){
				XElement ex = e;
				if (ex.Name.LocalName == "import"){
					ex = new XElement(e);
					genInt.Interpolate(ex, clselement);
				}
				clselement.Add(ex);
			}
			foreach (XAttribute a in clselement.Attributes()){
				a.Value = a.Value.Replace("~{", "${");
			}
			IBSharpClass cls = Compiler.ReadSingleClassSource(clselement, ns, null);
			RegisterClassInIndex(cls);
		}


		private IEnumerable<XElement> GetDataSet(string code, string ns, bool optional){
			string realcode = BSharpSyntax.DatasetClassCodePrefix + code;
			IBSharpClass targetcls = Get(realcode, ns, true);
			// not existed optional dataset support

			if (null == targetcls){
				if (optional){
					return new XElement[]{};
				}
				else{
					throw new Exception("dataset " + ns + "." + code + " not found");
				}
			}
			return GetDataSet(targetcls.Source, targetcls.Namespace);
		}

		private IEnumerable<XElement> GetDataSet(XElement source, string ns){
			string mode = source.Attr(BSharpSyntax.ConnectionModeAttribute, "native");
			if (mode != "native"){
				if (mode == "sql"){
					foreach (XElement item in GetSqlDataSet(source, ns)){
						yield return item;
					}
				}
				else{
					throw new Exception("not defined mode " + mode);
				}
			}


			foreach (XAttribute attr in source.Attributes()){
				if (attr.Name == BSharpSyntax.DatasetImport){
					foreach (XElement e in GetDataSet(attr.Value, ns, false)){
						yield return e;
					}
				}

				if (attr.Name == "code" && string.IsNullOrWhiteSpace(source.Attr(BSharpSyntax.DatasetImport)) &&
				    source.Parent.Name.LocalName == BSharpSyntax.Generator){
					foreach (
						XElement e in GetDataSet(attr.Value, ns, source.Describe().Name == "optional" || source.Attr("optional").ToBool())
						){
						yield return e;
					}
				}
			}

			foreach (XElement element in source.Elements()){
				if (element.Name == BSharpSyntax.DatasetImport){
					foreach (
						XElement e in
							GetDataSet(element.Attr("code"), ns, element.Describe().Name == "optional" || element.Attr("optional").ToBool())){
						yield return e;
					}
				}
				else if (element.Name == BSharpSyntax.DatasetItem){
					yield return element;
				}
			}
		}

		/// <summary>
		///     Считывает SQL-набор данных
		/// </summary>
		/// <param name="source"></param>
		/// <param name="ns"></param>
		/// <returns></returns>
		private IEnumerable<XElement> GetSqlDataSet(XElement source, string ns){
			string connectionCode = source.Attr("connection", "default");
			string query = source.Attr("query");
			if (query[0] == '#'){
				query = GetTemplate(query.Substring(1), source, ns);
			}
			else{
				query = InterpolateSingleString(source, query);
			}
			if (string.IsNullOrWhiteSpace(query)){
				return new XElement[]{};
			}
			string connection = GetConnectionString(connectionCode, "sql", ns);
			if (null == connection){
				throw new Exception("sql connection with code " + connectionCode + " not found");
			}
			return Compiler.SqlAdapter.ExecuteReader(connection, query, BSharpSyntax.DatasetItem);
		}

		private string GetTemplate(string code, XElement source, string ns){
			IBSharpClass template = Get(BSharpSyntax.GenerateTemplateClassName(code), ns: ns);
			if (null == template){
				throw new Exception("template not found " + code);
			}
			string templatestring = template.Source.Attr(BSharpSyntax.TemplateValueAttribute);
			return InterpolateSingleString(source, templatestring);
		}

		private string InterpolateSingleString(XElement source, string templatestring){
			var dict = new Dictionary<string, string>();
			foreach (XAttribute a in source.Attributes()){
				dict[a.Name.LocalName] = a.Value;
			}

			string result = templater.Interpolate(templatestring, dict);
			return result;
		}

		private void BuildDictionaryIndex(){
			Dictionaries = Dictionaries ?? new Dictionary<string, IList<ExportRecord>>();
			foreach (IBSharpClass cls in Working.Where(_ => _.Is(BSharpClassAttributes.RequireDictionaryRegistration))){
				XElement[] exports = cls.Compiled.Elements(BSharpSyntax.ClassExportDefinition).ToArray();
				exports.Remove();
				foreach (XElement e in exports){
					string code = e.GetCode();
					if (!Dictionaries.ContainsKey(code)){
						Dictionaries[code] = new List<ExportRecord>();
					}
					Dictionaries[code].Add(new ExportRecord(cls, e));
				}
			}
		}

		/// <summary>
		///     Разрешает словари
		/// </summary>
		public void ResolveDictionaries(){
			BuildLinkingIndex();
		}

		private void RegisterPartials(){
			foreach (var partialSet in PartialsRegistry.Values){
				IBSharpClass merged = JoinPartials(partialSet);
				if (null != merged){
					RegisterClassInIndex(merged);
				}
			}
		}

		private IBSharpClass JoinPartials(IEnumerable<IBSharpClass> partials){
			IBSharpClass basis = partials.First();
			bool reg = true;
			if (RawClasses.ContainsKey(basis.FullName)){
				basis = RawClasses[basis.FullName];
				reg = false;
			}
			foreach (IBSharpClass cls in partials){
				if (basis == cls) continue;
                if(IsIgnored(cls))continue;
				MergePartial(basis, cls);
			}
			if (reg) return basis;
			return null;
		}

		private void MergePartial(IBSharpClass basis, IBSharpClass cls){
			basis.Set(cls.GetAttributes());
			foreach (IBSharpImport import in cls.SelfImports){
				if (basis.SelfImports.All(_ => _.TargetCode != import.TargetCode)){
					basis.SelfImports.Add(import);
				}
			}
			foreach (XAttribute attribute in cls.Source.Attributes()){
				string n = attribute.Name.LocalName;
				string v = attribute.Value;
				if (n == "_file" || n == "_line" || n == "code" || n == "id" || n=="_dir") continue;
				string ex = basis.Source.Attr(n);
				if (ex == v) continue;
				if (string.IsNullOrWhiteSpace(ex)){
					basis.Source.SetAttr(n, v);
				}
				else{
					RegisterError(new BSharpError{
						Class = basis,
						AltClass = cls,
						Level = ErrorLevel.Error,
						Type = BSharpErrorType.PartialError,
						Message = "Conflict Attribute: " + n
					});
				}
			}
			foreach (XElement element in cls.Source.Elements()){
				if (string.IsNullOrWhiteSpace(element.GetCode())){
					RegisterError(new BSharpError{
						Class = basis,
						AltClass = cls,
						Level = ErrorLevel.Error,
						Type = BSharpErrorType.PartialError,
						Xml = element,
						Message = "Only coded elements can be used in partials"
					});
					continue;
				}
				XElement existed = basis.Source.Elements(element.Name).FirstOrDefault(_ => _.GetCode() == element.GetCode());
				if (null == existed){
					basis.Source.Add(element);
				}
				else{
					if (element.Elements().Any()){
						if (existed.Elements().Any()){
							RegisterError(new BSharpError{
								Class = basis,
								AltClass = cls,
								Level = ErrorLevel.Error,
								Type = BSharpErrorType.PartialError,
								Xml = element,
								Message = "Body conflict in element"
							});
							continue;
						}
						else{
							existed.Add(element.Elements());
						}
					}
					foreach (XAttribute attribute in element.Attributes()){
						string n = attribute.Name.LocalName;
						string v = attribute.Value;
						if (n == "_file" || n == "_line" || n == "code" || n == "id" || n=="if") continue;
						string ex = existed.Attr(n);
						if (ex == v) continue;
						if (string.IsNullOrWhiteSpace(ex)){
							existed.SetAttr(n, v);
						}
						else{
							RegisterError(new BSharpError{
								Class = basis,
								AltClass = cls,
								Level = ErrorLevel.Error,
								Type = BSharpErrorType.PartialError,
								Xml = element,
								Message = "Conflict Attribute In Element: " + n
							});
						}
					}
				}
			}
		}

		private void RegisterClassInIndex(IBSharpClass cls){
			IDictionary<string, IBSharpClass> target = cls.Is(BSharpClassAttributes.MetaClass) ? MetaClasses : RawClasses;
			if (target.ContainsKey(cls.FullName)){
				if (target[cls.FullName] != cls){
					IBSharpClass current = RawClasses[cls.FullName];
					int currentPriority = current.Source.Attr(BSharpSyntax.PriorityAttribute).ToInt();
					int givenPriority = cls.Source.Attr(BSharpSyntax.PriorityAttribute).ToInt();
					if (givenPriority > currentPriority){
						target[cls.FullName] = cls;
					}
					else{
						if (!cls.Is(BSharpClassAttributes.Anonymous)){
							// doubling of anonymous classed can be smoothed
							//если же они совпадают, то значит это дублирующиеся классы
							if (givenPriority == currentPriority){
								Errors.Add(BSharpErrors.DuplicateClassNames(cls, target[cls.FullName]));
							}
						}
					}
				}
			}
			else{
				if (cls.Is(BSharpClassAttributes.Patch)){
					cls.Source = cls.Source.NoEvidenceCopy();
				}
				target[cls.FullName] = cls;
			}
		}


		private void ResolveIgnored(){
			foreach (
				IBSharpClass c in
					RawClasses.Values.Where(_ => null != _.Source.Attribute(BSharpSyntax.ConditionalAttribute)).ToArray()){
			    if (IsIgnored(c)) {
                    c.Set(BSharpClassAttributes.Ignored);
                }
			}
		}

	    private bool IsIgnored(IBSharpClass cls) {
	        var condition = cls.Source.Attribute(BSharpSyntax.ConditionalAttribute);
	        if (null == condition) return false;
	        var scope = (Scope)(null == Compiler ? new Scope() : Compiler.GetConditions());
	        scope.ApplyXml(cls.Source);
	        return !new LogicalExpressionEvaluator().Eval(condition.Value, scope);
	    }

		private void ApplyExtensions(){
			foreach (IBSharpClass o in Extensions){
				IBSharpClass cls = Get(o.TargetClassName, ns: o.Namespace);
				if (null == cls){
					RegisterError(BSharpErrors.ClassCreatedFormExtension(o.Source, o.TargetClassName));
					o.Remove(BSharpClassAttributes.Extension);
					o.Name = o.TargetClassName;
				}
				else{
					ApplyExtension(o, cls);
				}
			}
		}

		private void ApplyExtension(IBSharpClass src, IBSharpClass trg){
			foreach (XAttribute e in src.Source.Attributes()){
				if (null == trg.Source.Attribute(e.Name)){
					trg.Source.Add(e);
				}
			}
			foreach (XElement e in src.Source.Elements()){
				if (e.Name.LocalName.StartsWith(BSharpSyntax.ElementOverridePrefix) ||
				    (e.Name.LocalName.StartsWith(BSharpSyntax.ElementExtensionPrefix))){
					trg.Source.AddFirst(e);
				}
				else{
					trg.Source.Add(e);
				}
			}
		}

		private void ApplyOverrides(){
			foreach (IBSharpClass o in Overrides){
				IBSharpClass cls = Get(o.TargetClassName, ns: o.Namespace);
				if (null == cls){
					RegisterError(BSharpErrors.ClassCreatedFormOverride(o.Source, o.TargetClassName));
					o.Remove(BSharpClassAttributes.Override);
					o.Name = o.TargetClassName;
				}
				else{
					ApplyOverride(o, cls);
				}
			}
		}

		private void ApplyOverride(IBSharpClass src, IBSharpClass trg){
			foreach (XAttribute e in src.Source.Attributes()){
				trg.Source.SetAttributeValue(e.Name, e.Value);
				if (e.Name.LocalName == "abstract"){
					if (e.Value.ToBool()){
						trg.Set(BSharpClassAttributes.Abstract);
					}
					else{
						trg.Remove(BSharpClassAttributes.Abstract);
					}
				}
			}
			foreach (XElement e in src.Source.Elements()){
				if (e.Name.LocalName.StartsWith(BSharpSyntax.ElementOverridePrefix) ||
				    (e.Name.LocalName.StartsWith(BSharpSyntax.ElementExtensionPrefix))){
					trg.Source.AddFirst(e);
				}
				else{
					trg.Source.Add(e);
				}
			}
		}

		private void ResolveImports(){
			foreach (IBSharpClass w in Working.Union(Abstracts)){
				foreach (IBSharpImport i in w.SelfImports){
					i.Orphaned = true;
					IBSharpClass import = Get(i.TargetCode, ns: w.Namespace);
					if (null != import){
						i.Orphaned = false;
						i.Target = import;
					}
					else{
						Errors.Add(BSharpErrors.NotResolvedImport(w, i));
					}
				}
			}
		}


		private void ResolveOrphans(){
			IEnumerable<IBSharpClass> _initiallyorphaned = RawClasses.Values.Where(_ => !_.Is(BSharpClassAttributes.Explicit));
			foreach (IBSharpClass o in _initiallyorphaned){
				string code = o.DefaultImportCode;
				string ns = o.Namespace;
				IBSharpClass import = Get(code, ns: ns);
				if (import != null){
					o.Remove(BSharpClassAttributes.Orphan);
					o.DefaultImport = import;
				}
			}
		}

		
	}
}