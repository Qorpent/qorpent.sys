using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.LogicalExpressions;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.LogicalExpressions;

namespace Qorpent.BSharp {
	/// <summary>
	///     Внутренний индекс компилятора
	/// </summary>
	public class BSharpContext : ConfigBase, IBSharpContext {
		private const string RAWCLASSES = "rawclasses";
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

		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string, object> ExtendedData { get; private set; }

		/// <summary>
		///     Исходные сырые определения классов
		/// </summary>
		public IDictionary<string, IBSharpClass> RawClasses {
			get { return Get<IDictionary<string, IBSharpClass>>(RAWCLASSES); }
			set { Set(RAWCLASSES, value); }
		}


		/// <summary>
		///     Классы с непроинициализированным наследованием
		/// </summary>
		public List<IBSharpClass> Orphans {
			get { return Get<List<IBSharpClass>>(ORPHANED); }
			set { Set(ORPHANED, value); }
		}

		/// <summary>
		///     Классы с непроинициализированным наследованием
		/// </summary>
		public List<IBSharpClass> Abstracts {
			get { return Get<List<IBSharpClass>>(ABSTRACTS); }
			set { Set(ABSTRACTS, value); }
		}

		/// <summary>
		///     Классы с непроинициализированным наследованием
		/// </summary>
		public List<IBSharpClass> Working {
			get { return Get<List<IBSharpClass>>(WORKING); }
			set { Set(WORKING, value); }
		}

		/// <summary>
		/// Классы со статической компиляцией
		/// </summary>
		public List<IBSharpClass> Static {
			get { return Get<List<IBSharpClass>>(STATIC); }
			set { Set(STATIC, value); }
		}
		/// <summary>
		/// Реестр перезагрузок классов
		/// </summary>
		public List<IBSharpClass> Overrides
		{
			get { return Get<List<IBSharpClass>>(OVERRIDES); }
			set { Set(OVERRIDES, value); }
		}
		/// <summary>
		/// Реестр перезагрузок классов
		/// </summary>
		public List<IBSharpClass> Extensions
		{
			get { return Get<List<IBSharpClass>>(EXTENSIONS); }
			set { Set(EXTENSIONS, value); }
		}

		/// <summary>
		/// Реестр перезагрузок классов
		/// </summary>
		public List<IBSharpClass> Ignored
		{
			get { return Get<List<IBSharpClass>>(IGNORED); }
			set { Set(IGNORED, value); }
		}

		/// <summary>
		/// Реестр перезагрузок классов
		/// </summary>
		public List<BSharpError> Errors
		{
			get { return Get<List<BSharpError>>(ERRORS); }
			set { Set(ERRORS, value); }
		}

		/// <summary>
		/// Реестр перезагрузок классов
		/// </summary>
		public IDictionary<string,IBSharpClass[]> PrototypeMap
		{
			get { return Get<IDictionary<string, IBSharpClass[]>>(PROTOTYPE_MAP); }
			set { Set(PROTOTYPE_MAP, value); }
		}
        
	    private IDictionary<string, string> _resolveDictCache = new Dictionary<string, string>();

		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string,IList<ExportRecord>> Dictionaries
        {
            get { return Get<IDictionary<string, IList<ExportRecord>>>(EXPORTS); }
            set { Set(EXPORTS, value); }
        }

	    /// <summary>
	    /// Строит индекс словарей
	    /// </summary>
	    public void BuildLinkingIndex() {
		    PrototypeMap = Working.Where(_ => !string.IsNullOrWhiteSpace(_.Prototype))
		                          .GroupBy(_ => _.Prototype).ToDictionary(_ => _.Key, _ => _.ToArray());
			BuildDictionaryIndex();
			
	    }

		/// <summary>
		/// Специальная индексация для модификатора all
		/// </summary>
		/// <param name="query"></param>
		/// <param name="ns"></param>
		/// <returns></returns>
		public IEnumerable<IBSharpClass> ResolveAll(string query, string ns) {

			if (query.StartsWith("attr:")){
				var attrname = query.Substring(5);
				return Working.Where(_ => _.Compiled.Attr(attrname).ToBool());
			}

			if (string.IsNullOrWhiteSpace(query)) {
				return Working;
			}
			if (query.EndsWith(".*")) {
				return Working.Where(_ => _.Namespace.StartsWith(query.Substring(0, query.Length - 2)));
			}
			if (PrototypeMap.ContainsKey(query)) {
				return PrototypeMap[query];
			}
			if (Dictionaries.ContainsKey(query)) {
				return Dictionaries[query].Select(_ => _.cls);
			}
			

			var basetype = Get(query, ns);
		
			if (null != basetype) {
				var result = Working.Where(_ =>  _.AllImports.Contains( basetype)).ToList();
				if (!basetype.Is(BSharpClassAttributes.Abstract)){
					result.Add(basetype);
				}
				return result.ToArray();
			}
			return new IBSharpClass[] {};
		}

		/// <summary>
		/// Выполняет генераторы, формируя дополнительные классы
		/// </summary>
		public void ExecuteGenerators()
		{

			var generators = RawClasses.Values.Where(_ => _.Is(BSharpClassAttributes.Generator)).ToArray();
			foreach (var cls in generators)
			{
				RawClasses.Remove(cls.FullName);
			}
			foreach (var generator in generators)
			{
				ExecuteGenerator(generator);
			}
			foreach (var bSharpClass in RawClasses.Values.Where(_=>_.Is(BSharpClassAttributes.Dataset)||_.Is(BSharpClassAttributes.Connection)||_.Is(BSharpClassAttributes.Template)).ToArray())
			{
				RawClasses.Remove(bSharpClass.FullName);
			}
		}

		private void ExecuteGenerator(IBSharpClass generator)
		{
			ExecuteGenerator(new XElement(generator.Source),generator.Namespace);
		}
		/// <summary>
		/// Возвращает строку соединения
		/// </summary>
		/// <param name="code"></param>
		/// <param name="mode"></param>
		/// <param name="ns"></param>
		/// <returns></returns>
		private string GetConnectionString(string code, string mode, string ns){
			var cls = GetConnection(mode, code, ns);
			if (null == cls) return null;
			return cls.Source.Attr(BSharpSyntax.ConnectionStringAttribute);
		}
		/// <summary>
		/// Возвращает класс соединения
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="code"></param>
		/// <param name="ns"></param>
		/// <returns></returns>
		private IBSharpClass GetConnection(string mode, string code, string ns){
			return Get(BSharpSyntax.GenerateConnectionClassName(mode, code),ns);
		}

		XmlInterpolation genInt = new XmlInterpolation();
		private static int autogenIndex = 0;
		private void ExecuteGenerator(XElement generator, string ns)
		{
			var datasets = generator.Elements(BSharpSyntax.Dataset).ToArray();
			var classCode = generator.Attr(BSharpSyntax.GeneratorClassCodeAttribute);
			var className = generator.Attr(BSharpSyntax.GeneratorClassNameAttribute);
			var ca = generator.Attribute(BSharpSyntax.GeneratorClassCodeAttribute);


			if (null != ca)
			{
				ca.Remove();
				
			}
			var na = generator.Attribute(BSharpSyntax.GeneratorClassNameAttribute);
			if (null != na)
			{
				na.Remove();
			}
			
			var resolvedDatasets = datasets.Select(_ => GetDataSet(_, ns).ToArray()).ToArray();
			foreach (var dataset in datasets)
			{
				dataset.Remove();
			}
			var combinations = resolvedDatasets.Combine().ToArray();
			
			foreach (var elementSet in combinations)
			{
				GenerateVariant(generator, ns, classCode, className, elementSet);
			}
		}

		private void GenerateVariant(XElement generator, string ns, string classCode, string className, XElement[] elementSet){
			var clselement = new XElement(BSharpSyntax.Class);
			var realcode = classCode;
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
			foreach (var xElement in elementSet){
				foreach (var a in xElement.Attributes()){
					clselement.SetAttributeValue(a.Name, a.Value);
				}
			}
			genInt.Interpolate(clselement);
			foreach (var a in generator.Attributes()){
				if (a.Name != "code" && a.Name != "name" && a.Name != "id"){
					clselement.SetAttributeValue(a.Name, a.Value);
				}
			}

			foreach (var e in generator.Elements()){
				var ex = e;
				if (ex.Name.LocalName == "import"){
					ex = new XElement(e);
					genInt.Interpolate(ex, clselement);
				}
				clselement.Add(ex);
			}
			foreach (var a in clselement.Attributes())
			{
				a.Value = a.Value.Replace("~{", "${");
			}
			var cls = Compiler.ReadSingleClassSource(clselement, ns,null);
			RegisterClassInIndex(cls);
		}


		private IEnumerable<XElement> GetDataSet(string code, string ns, bool optional)
		{
			var realcode = BSharpSyntax.DatasetClassCodePrefix + code;
			var targetcls = Get(realcode, ns);
			// not existed optional dataset support
			
			if (null == targetcls ){
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

			var mode = source.Attr(BSharpSyntax.ConnectionModeAttribute,"native");
			if (mode != "native"){
				if (mode == "sql"){

					foreach (var item in GetSqlDataSet(source,ns)){
						yield return item;

					}
				}
				else{
					throw new Exception("not defined mode "+mode);
				}
			}
			

			foreach (var attr in source.Attributes())
			{
				if (attr.Name == BSharpSyntax.DatasetImport)
				{
					foreach (var e in GetDataSet(attr.Value,ns,false))
					{
						yield return e;
					}
				}

				if (attr.Name=="code" && string.IsNullOrWhiteSpace(source.Attr(BSharpSyntax.DatasetImport)) && source.Parent.Name.LocalName==BSharpSyntax.Generator)
				{
					foreach (var e in GetDataSet(attr.Value, ns, source.Describe().Name == "optional" || source.Attr("optional").ToBool()))
					{
						yield return e;
					}
				}
			}

			foreach (var element in source.Elements())
			{
				if (element.Name == BSharpSyntax.DatasetImport)
				{
					foreach (var e in GetDataSet(element.Attr("code"), ns,element.Describe().Name=="optional"||element.Attr("optional").ToBool()))
					{
						yield return e;
					}
				}else if (element.Name == BSharpSyntax.DatasetItem)
				{
					yield return element;
				}
			}
		}
		/// <summary>
		/// Считывает SQL-набор данных
		/// </summary>
		/// <param name="source"></param>
		/// <param name="ns"></param>
		/// <returns></returns>
		private IEnumerable<XElement> GetSqlDataSet(XElement source, string ns){
			var connectionCode = source.Attr("connection","default");
			var query = source.Attr("query");
			if (query[0]=='#'){
				query = GetTemplate(query.Substring(1), source, ns);
			}
			else{
				query = InterpolateSingleString(source, query);
			}
			if (string.IsNullOrWhiteSpace(query)){
				return new XElement[]{};
			}
			var connection = GetConnectionString(connectionCode, "sql", ns);
			if (null == connection){
				throw new Exception("sql connection with code "+connectionCode+" not found");
			}
			return Compiler.SqlAdapter.ExecuteReader(connection, query,BSharpSyntax.DatasetItem);
		}
		StringInterpolation templater = new StringInterpolation();
		private string GetTemplate(string code, XElement source, string ns){
			var template = Get(BSharpSyntax.GenerateTemplateClassName(code), ns);
			if (null == template){
				throw new Exception("template not found " + code);
			} 
			var templatestring = template.Source.Attr(BSharpSyntax.TemplateValueAttribute);
			return InterpolateSingleString(source, templatestring);
		}

		private string InterpolateSingleString(XElement source, string templatestring){
			var dict = new Dictionary<string, string>();
			foreach (var a in source.Attributes()){
				dict[a.Name.LocalName] = a.Value;
			}

			var result = templater.Interpolate(templatestring, dict);
			return result;
		}

		private void BuildDictionaryIndex() {
			Dictionaries = Dictionaries ?? new Dictionary<string, IList<ExportRecord>>();
			foreach (var cls in Working.Where(_ => _.Is(BSharpClassAttributes.RequireDictionaryRegistration))) {
				var exports = cls.Compiled.Elements(BSharpSyntax.ClassExportDefinition).ToArray();
				exports.Remove();
				foreach (var e in exports) {
					var code = e.GetCode();
					if (!Dictionaries.ContainsKey(code)) {
						Dictionaries[code] = new List<ExportRecord>();
					}
					Dictionaries[code].Add(new ExportRecord(cls, e));
				}
			}
		}

		/// <summary>
        /// Разрешает элементы в словаре
        /// </summary>
        /// <returns></returns>
	    public string ResolveDictionary(string code, string element) {
	        if (null == Dictionaries) return null;
			var key = code + BSharpSyntax.ClassPathDelimiter + element;
            if (_resolveDictCache.ContainsKey(key)) {
                return _resolveDictCache[key];
            }
            lock (this) {
                string result = null;
                if (Dictionaries.ContainsKey(code)) {
                    foreach (var e in Dictionaries[code]) {
                        result = e.Resolve(element);
                        if (null != result) break;
                    }   
                }
                _resolveDictCache[key] = result;
                return result;
            }
	    }
        /// <summary>
        /// Разрешает словари
        /// </summary>
        public void ResolveDictionaries() {
            BuildLinkingIndex();
        }
		/// <summary>
		/// 
		/// </summary>
		public void ClearBuildTasks() {
			foreach (var c in RawClasses.Values) {
				c.BuildTask = null;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public BSharpContext(IBSharpCompiler compiler = null) {
			this.Compiler = compiler;
			ExtendedData = new Dictionary<string, object>();
		}
		/// <summary>
		/// Компилятор
		/// </summary>
		public IBSharpCompiler Compiler { get; set; }

		/// <summary>
		/// Загружает исходные определения классов
		/// </summary>
		/// <param name="rawclasses"></param>
		public void Setup(IEnumerable<IBSharpClass> rawclasses) {
			if (null == RawClasses) {
				RawClasses = new Dictionary<string, IBSharpClass>();
			}
			if (null == Errors) {
				Errors = new List<BSharpError>();
			}
			foreach (var cls in rawclasses) {
				RegisterClassInIndex(cls);
			}
		}

		private void RegisterClassInIndex(IBSharpClass cls)
		{
			if (RawClasses.ContainsKey(cls.FullName))
			{
				
				if (RawClasses[cls.FullName] != cls){
					var current = RawClasses[cls.FullName];
					var currentPriority = current.Source.Attr(BSharpSyntax.PriorityAttribute).ToInt();
					var givenPriority = cls.Source.Attr(BSharpSyntax.PriorityAttribute).ToInt();
					if (givenPriority > currentPriority){
						RawClasses[cls.FullName] = cls;
					}
					else{
						if (!cls.Is(BSharpClassAttributes.Anonymous)){ // doubling of anonymous classed can be smoothed
							//если же они совпадают, то значит это дублирующиеся классы
							if (givenPriority == currentPriority){
								Errors.Add(BSharpErrors.DuplicateClassNames(cls, RawClasses[cls.FullName]));
							}
						}
					}
					
				}
			}
			else
			{
				RawClasses[cls.FullName] = cls;
			}
		}


		/// <summary>
		///     Присоединяет и склеивается с другим результатом
		/// </summary>
		/// <param name="othercontext"></param>
		public void Merge(IBSharpContext othercontext) {
			var subresult = othercontext as BSharpContext;

			if (null != subresult.Abstracts) {
				if (null == Abstracts) {
					Abstracts = new List<IBSharpClass>();
				}
				foreach (IBSharpClass a in subresult.Abstracts) {
					Abstracts.Add(a);
				}
			}
			if (null != subresult.Orphans) {
				if (null == Orphans) {
					Orphans = new List<IBSharpClass>();
				}
				foreach (IBSharpClass a in subresult.Orphans) {
					Orphans.Add(a);
				}
			}
			if (null != subresult.Working) {
				if (null == Working) {
					Working = new List<IBSharpClass>();
				}
				foreach (IBSharpClass a in subresult.Working) {
					Working.Add(a);
				}
			}

			if (null != subresult.Overrides)
			{
				if (null == Overrides)
				{
					Overrides = new List<IBSharpClass>();
				}
				foreach (IBSharpClass a in subresult.Overrides)
				{
					Overrides.Add(a);
				}
			}

			if (null != subresult.Extensions)
			{
				if (null == Extensions)
				{
					Extensions = new List<IBSharpClass>();
				}
				foreach (IBSharpClass a in subresult.Extensions)
				{
					Extensions.Add(a);
				}
			}

			if (null != subresult.Errors)
			{
				if (null == Errors)
				{
					Errors = new List<BSharpError>();
				}
				foreach (var a in subresult.Errors)
				{
					Errors.Add(a);
				}
			}

            if (null != subresult.Dictionaries) {
                Dictionaries = Dictionaries ?? new Dictionary<string, IList<ExportRecord>>();
                foreach (var p in subresult.Dictionaries) {
                    if (!Dictionaries.ContainsKey(p.Key)) {
                        Dictionaries[p.Key]= new List<ExportRecord>();
                    }

                    var t = Dictionaries[p.Key];
                    foreach (var  d in p.Value) {
                        if (!t.Contains(d)) {
                            t.Add(d);
                        }
                    }
                }
            }
			if (null != subresult.PrototypeMap){
				if (null == PrototypeMap){
					PrototypeMap = new Dictionary<string, IBSharpClass[]>();
				}
				foreach (var p in subresult.PrototypeMap)
				{

					if (!PrototypeMap.ContainsKey(p.Key))
					{
						PrototypeMap[p.Key] = p.Value;
					}
					else
					{
						PrototypeMap[p.Key] = PrototypeMap[p.Key].Union(p.Value).Distinct().ToArray();
					}
				}
			}
		    
		}

		private IDictionary<string, IBSharpClass> _resolveclassCache = new Dictionary<string, IBSharpClass>();
		/// <summary>
		/// Разрешает класс по коду и заявленному пространству имен
		/// </summary>
		/// <param name="code"></param>
		/// <param name="ns"></param>
		/// <returns></returns>
		public  IBSharpClass Get( string code, string ns = null) {

			if (null == ns) {
				var full = Working.FirstOrDefault(_ => _.FullName == code);
				if (null != full) return full;
				return Working.FirstOrDefault(_ => _.Name == code);
			}
			var key = ns + BSharpSyntax.ClassPathDelimiter + code;
			if (_resolveclassCache.ContainsKey(key)) {
				return _resolveclassCache[key];
			}
			IBSharpClass result = null;
			if (!String.IsNullOrWhiteSpace(code)) {
				if (code.Contains(BSharpSyntax.ClassPathDelimiter))
				{
					if (RawClasses.ContainsKey(code)) {
						result = RawClasses[code];
					}
				}
				else if (ns.Contains(BSharpSyntax.ClassPathDelimiter))
				{
					var nsparts = ns.SmartSplit(false, true, BSharpSyntax.ClassPathDelimiter);
					for (var i = nsparts.Count - 1; i >= -1; i--) {
						if (i == -1) {
							var probe = code;
							if (RawClasses.ContainsKey(probe)) {
								result = RawClasses[probe];
								break;
							}
						}
						else {
							var probe = "";
							for (var j = 0; j <= i; j++) {
								probe += nsparts[j] + BSharpSyntax.ClassPathDelimiter;
							}
							probe += code;
							if (RawClasses.ContainsKey(probe)) {
								result = RawClasses[probe];
								break;
							}
						}
					}


				}
				else if(!String.IsNullOrWhiteSpace(ns)) {
					var probe = ns+"." + code;
					if (RawClasses.ContainsKey(probe))
					{
						result = RawClasses[probe];
					}
					
				}
			}
			if (null == result) {
				if (null!=RawClasses && RawClasses.ContainsKey(code))
				{
					result = RawClasses[code];
				}
			}

            if (null == result) {
                var variants = RawClasses.Values.Where(_ => ("." + _.FullName).EndsWith("." + code));
                if (variants.Count() == 1) {
                    result = variants.First();
                } else {
                    variants = variants.Where(_ => _.Namespace.Contains(ns));
                    if (variants.Count() == 1) {
                        result = variants.First();
                    }
                }
            }

			if (null != result) {
				_resolveclassCache[key] = result;
			}
			return result;
		}
		/// <summary>
		/// Возвращает коллекцию классов по типу классов
		/// </summary>
		/// <param name="datatype"></param>
		/// <returns></returns>
		public IEnumerable<IBSharpClass> Get(BSharpContextDataType datatype) {
			switch (datatype) {
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
                    return Working.Where(_=>!_.Is(BSharpClassAttributes.Embed)).Distinct();
				case BSharpContextDataType.Errors:
					
				default :
					return new IBSharpClass[] {};
			}
		}

		/// <summary>
		/// Возвращает ошибки указанного уровня
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public IEnumerable<BSharpError> GetErrors(ErrorLevel level = ErrorLevel.None) {
			if (null == Errors) yield break;
			foreach (var e in Errors.ToArray()) {
                if (null == e) continue;
				if (e.Level >= level) {
					yield return e;
				}
			}
		}

		private bool _built = false;
		/// <summary>
		/// Строит рабочий индекс классов
		/// </summary>
		public void Build() {
			if (_built) return;
			Overrides = RawClasses.Values.Where(
				_ => _.Is(BSharpClassAttributes.Override))
				.OrderBy(_=>_.Source.Attr(BSharpSyntax.PriorityAttribute).ToInt())
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
			foreach (var o in Orphans) {
				RegisterError(BSharpErrors.OrphanClass(o));
			}
			Abstracts = RawClasses.Values.Where(_ => _.Is(BSharpClassAttributes.Abstract) && !_.IsOrphaned && !_.Is(BSharpClassAttributes.Ignored)).ToList();
			Working = RawClasses.Values.Where(_ => 
				!_.Is(BSharpClassAttributes.Extension) && 
				!_.Is(BSharpClassAttributes.Override) && 
				!_.Is(BSharpClassAttributes.Abstract) && 
				!_.IsOrphaned && 
				!_.Is(BSharpClassAttributes.Ignored)).ToList();
			Static = RawClasses.Values.Where(_ => _.Is(BSharpClassAttributes.Static) && !_.IsOrphaned && !_.Is(BSharpClassAttributes.Ignored)).ToList();
			
			ResolveImports();
			_built = true;
		}

		private void ResolveIgnored() {
			foreach (var c in RawClasses.Values.Where(_ => null != _.Source.Attribute(BSharpSyntax.ConditionalAttribute)).ToArray()) {
				var options = null==Compiler? new ConfigBase() : Compiler.GetConditions();
				foreach (var a in c.Source.Attributes()) {
					if (!options.ContainsKey(a.Name.LocalName)) {
						options.Set(a.Name.LocalName, a.Value);
					}
				}
				var src = new DictionaryTermSource<object>(options);
				if (!new LogicalExpressionEvaluator().Eval(c.Source.Attr(BSharpSyntax.ConditionalAttribute), src))
				{
					c.Set(BSharpClassAttributes.Ignored);
				}
			}
		}

		/// <summary>
		/// Регистрирует ошибку в контексте
		/// </summary>
		/// <param name="error"></param>
		public void RegisterError(BSharpError error) {
			if (null == Errors) Errors = new List<BSharpError>();
			Errors.Add(error);
		}

		private void ApplyExtensions() {
			foreach (var o in Extensions) {
				var cls = Get(o.TargetClassName, o.Namespace);
				if (null == cls) {
					RegisterError(BSharpErrors.ClassCreatedFormExtension(o.Source, o.TargetClassName));
					o.Remove(BSharpClassAttributes.Extension);
					o.Name = o.TargetClassName;
				}
				else
				{
					ApplyExtension(o, cls);
				}
			}
		}

		private void ApplyExtension(IBSharpClass src, IBSharpClass trg) {
			foreach (var e in src.Source.Attributes()) {
				if (null == trg.Source.Attribute(e.Name)) {
					trg.Source.Add(e);
				}
			}
            foreach (var e in src.Source.Elements())
            {
				if (e.Name.LocalName.StartsWith(BSharpSyntax.ElementOverridePrefix) || (e.Name.LocalName.StartsWith(BSharpSyntax.ElementExtensionPrefix)))
                {
                    trg.Source.AddFirst(e);
                }
                else
                {
                    trg.Source.Add(e);
                }
            }
		}

		private void ApplyOverrides()
		{
			foreach (var o in Overrides) {
				var cls = Get(o.TargetClassName, o.Namespace);
				if (null == cls) {
					RegisterError(BSharpErrors.ClassCreatedFormOverride(o.Source, o.TargetClassName));
					o.Remove(BSharpClassAttributes.Override);
					o.Name = o.TargetClassName;
				}
				else {
					ApplyOverride(o, cls);
				}
			}
		}

		private void ApplyOverride(IBSharpClass src, IBSharpClass trg) {
			foreach (var e in src.Source.Attributes())
			{
				trg.Source.SetAttributeValue(e.Name,e.Value);
                if (e.Name.LocalName == "abstract") {
                    if (e.Value.ToBool()) {
                        trg.Set(BSharpClassAttributes.Abstract);
                    }
                    else {
                        trg.Remove(BSharpClassAttributes.Abstract);
                    }
                }
			}
            foreach (var e in src.Source.Elements())
            {
				if (e.Name.LocalName.StartsWith(BSharpSyntax.ElementOverridePrefix) || (e.Name.LocalName.StartsWith(BSharpSyntax.ElementExtensionPrefix)))
                {
                    trg.Source.AddFirst(e);
                }
                else
                {
                    trg.Source.Add(e);
                }
            }
		}
		/// <summary>
		/// Определяет наличие словаря
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public bool HasDictionary(string code) {
			return Dictionaries != null && Dictionaries.ContainsKey(code);
		}
		/// <summary>
		/// Проверяет необходимость линковки контекста в целом
		/// </summary>
		/// <returns></returns>
		public bool RequireLinking() {
			return Working.Any(_ => _.Is(BSharpClassAttributes.RequireLinking));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool RequirePatching(){
			return Working.Any(_ => _.Is(BSharpClassAttributes.Patch));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool RequrePostProcess(){
			return Working.Any(_ => _.Compiled.Descendants(BSharpSyntax.PostProcessRemoveBefore).Any() 
				|| _.Compiled.Descendants(BSharpSyntax.PostProcessSelectElements).Any());
		}

		private void ResolveImports()
		{
			foreach (var w in Working.Union(Abstracts))
			{
				foreach (IBSharpImport i in w.SelfImports)
				{
					i.Orphaned = true;
					var import = Get(i.TargetCode, w.Namespace);
					if (null != import) {
						i.Orphaned = false;
						i.Target = import;
					}
					else {
						Errors.Add(BSharpErrors.NotResolvedImport(w, i));	
					}
				}
			}
		}


		private void ResolveOrphans() {
			IEnumerable<IBSharpClass> _initiallyorphaned = RawClasses.Values.Where(_ => !_.Is(BSharpClassAttributes.Explicit));
			foreach (var o in _initiallyorphaned) {
				string code = o.DefaultImportCode;
				string ns = o.Namespace;
				var import = Get(code, ns);
				if (import != null) {
					o.Remove(BSharpClassAttributes.Orphan);
					o.DefaultImport = import;
				}
			}
		}
	}
}