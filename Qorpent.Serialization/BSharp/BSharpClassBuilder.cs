using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.BSharp.Matcher;
using Qorpent.Config;
using Qorpent.Log;
using Qorpent.LogicalExpressions;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.LogicalExpressions;

namespace Qorpent.BSharp {
	/// <summary>
	/// 
	/// </summary>
	public class BSharpClassBuilder {
		private readonly IBSharpCompiler _compiler;
		private readonly IBSharpClass _cls;
		private readonly IBSharpContext _context;

		IUserLog Log {
			get { return _compiler.GetConfig().Log; }
		}

		private IBSharpConfig GetConfig() {
			return _compiler.GetConfig();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="phase"></param>
		/// <param name="compiler"></param>
		/// <param name="cls"></param>
		/// <param name="context"></param>
		public static void Build(BuildPhase phase, IBSharpCompiler compiler, IBSharpClass cls, IBSharpContext context) {
			new BSharpClassBuilder(compiler, cls, context).Build(phase);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="phase"></param>
		/// <param name="compiler"></param>
		/// <param name="cls"></param>
		/// <param name="context"></param>
		public static Task BuildAsync(BuildPhase phase, BSharpCompiler compiler, IBSharpClass cls, BSharpContext context)
		{
			var task = new Task(() => new BSharpClassBuilder(compiler, cls, context).Build(phase));
			cls.BuildTask = task;
			task.Start();
			return task;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bSharpCompiler"></param>
		/// <param name="cls"></param>
		/// <param name="context"></param>
		protected BSharpClassBuilder(IBSharpCompiler bSharpCompiler, IBSharpClass cls, IBSharpContext context) {
			_compiler = bSharpCompiler;
			_cls = cls;
			_context = context;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="phase"></param>
		public void Build(BuildPhase phase)
		{
			
			lock (_cls) {
				if (BuildPhase.Compile == phase) {
					PerformCompilation();
				}
				else if (BuildPhase.Link == phase) {
					PerformLinking();
				}
			}
		}

		private void PerformLinking() {
			if (CheckExistedLink()) return;
			_cls.Set(BSharpClassAttributes.InLink);
			InternalLink();
			_cls.Remove(BSharpClassAttributes.InLink);
			_cls.Set(BSharpClassAttributes.Linked);
		}

		private void InternalLink() {
			ResolveAdvancedIncludes();
			ResolveClassReferences();
			ResolveDictionaries();
		}

		private void ResolveAdvancedIncludes() {
			if (!_cls.Is(BSharpClassAttributes.RequireAdvancedIncludes)) return;
			var includes =
				_cls.Compiled.Descendants(BSharpSyntax.IncludeBlock)
				    .Where(_ => null != _.Attribute(BSharpSyntax.IncludeAllModifier))
					.ToArray();
			foreach (var i in includes) {
				var query = i.GetName();
				if (query == BSharpSyntax.IncludeBodyModifier || query == BSharpSyntax.IncludeNoChildModifier) {
					query = "";
				}
				var sources = _context.ResolveAll(query, _cls.Namespace).ToArray();
				if (0 == sources.Length) {
					i.Remove();
					continue;
				}
				ProcessAdvancedInclude(_cls, i, sources);
			}

		}

		private void ProcessAdvancedInclude(IBSharpClass cls, XElement e, IBSharpClass[] sources) {
			throw new System.NotImplementedException();
		}

		private void ResolveDictionaries() {
			if (!_cls.Is(BSharpClassAttributes.RequireDictionaryResolution)) return;
			foreach (var a in _cls.Compiled.DescendantsAndSelf().SelectMany(_ => _.Attributes())) {
				var val = a.Value;
				if (string.IsNullOrWhiteSpace(val)||val.Length<2) continue;
				if (val[0]==BSharpSyntax.DictionaryReferencePrefix)
				{
					bool valueonly = false;
					bool canbeignored = false;
					val = val.Substring(1);
					if (val[0] == BSharpSyntax.DictionaryReferenceValueOnlyModifier)
					{
						valueonly = true;
						val = val.Substring(1);
					}
					if (val.Length == 0) {
						continue;
					}
					if (val[0]==BSharpSyntax.DictionaryReferenceOptionalModifier)
					{
						canbeignored = true;
						val = val.Substring(1);
					}
					if (val.Length == 0) continue;
					var fstdot = val.IndexOf(BSharpSyntax.DictionaryCodeElementDelimiter);
					var code = val.Substring(0, fstdot);
					var element = val.Substring(fstdot + 1);

					var result = _context.ResolveDictionary(code, element);
					if (null == result)
					{
						if (canbeignored)
						{
							a.Value = "";
						}
						else
						{
							a.Value = "notresolved:" + val;
							if (!_context.HasDictionary(code))
							{
								_context.RegisterError(BSharpErrors.NotResolvedDictionary(_cls.FullName, a.Parent,
									code));
							}
							else
							{
								_context.RegisterError(BSharpErrors.NotResolvedDictionaryElement(_cls.FullName,
									a.Parent, val));
							}
						}
					}
					else
					{
						if (valueonly)
						{
							a.Value = result.Split('|')[0];
						}
						else
						{
							a.Value = result;
						}
					}
				}
			}
		}

		private void PerformCompilation() {
			if (CheckExistedBuild()) return;
			_cls.Set(BSharpClassAttributes.InBuild);
			InternalBuild();
			_cls.Remove(BSharpClassAttributes.InBuild);
			_cls.Set(BSharpClassAttributes.Built);
		}


		private void InternalBuild() {
			InitializeBuildIndexes();
			IntializeMergeIndexes();
			InterpolateFields();
			BindParametersToCompiledClass();

			CleanupElementsWithConditions();
			
			MergeInternals();
			InterpolateElements();
			PerformMergingWithElements();
			
			CleanupElementsWithConditions();

			ProcessSimpleIncludes();

			CleanupPrivateMembers();

			CheckoutRequireLinkingRequirements();

		}

		private void CheckoutRequireLinkingRequirements() {
			var attrs = _cls.Compiled.DescendantsAndSelf().SelectMany(_ => _.Attributes());
			foreach (var a in attrs) {
				var val = a.Value;
				if (string.IsNullOrWhiteSpace(val) || val.Length < 2) continue;
				if (val[0]==BSharpSyntax.ClassReferencePrefix) {
					_cls.Set(BSharpClassAttributes.RequireClassResolution);
				}else if (val[0]==BSharpSyntax.DictionaryReferencePrefix) {
					_cls.Set(BSharpClassAttributes.RequireDictionaryResolution);
				}
			}
			if (_cls.Compiled.Elements(BSharpSyntax.ClassExportDefinition).Any()) {
				_cls.Set(BSharpClassAttributes.RequireDictionaryRegistration);
			}
			if (_cls.Compiled.Descendants(BSharpSyntax.IncludeBlock).Any())
			{
				_cls.Set(BSharpClassAttributes.RequireAdvancedIncludes);
			}
		}


		private void ResolveClassReferences() {
			if (!_cls.Is(BSharpClassAttributes.RequireClassResolution)) return;
			//найдем все атрибуты, начинающиеся на ^
			foreach (var a in _cls.Compiled.DescendantsAndSelf().SelectMany(_ => _.Attributes())) {
				if (a.Value.StartsWith("^")) {
					var clsname = a.Value.Substring(1);
					var normallyresolvedClass = _context.Get(clsname, _cls.Namespace);
					if (null != normallyresolvedClass) {
						a.Value = normallyresolvedClass.FullName;
					}
					else {
						var candidates = _context.Get(BSharpContextDataType.Working).Where(_ => _.FullName.EndsWith(clsname)).ToArray();
						if (!candidates.Any()) {
							a.Value = "NOTRESOLVED::" + clsname;
							_context.RegisterError(BSharpErrors.NotResolvedClassReference(_cls, a.Parent,clsname));
						}else if (1 == candidates.Count()) {
							a.Value = candidates[0].FullName;
							_context.RegisterError(BSharpErrors.NotDirectClassReference(_cls, a.Parent, clsname));
						}
						else {
							a.Value = "AMBIGOUS::" + clsname;
							_context.RegisterError(BSharpErrors.AmbigousClassReference(_cls, a.Parent, clsname));
						}
					}
				}
			}
		}

		private void ProcessSimpleIncludes() {
			XElement[] includes;
			var needReInterpolate = false;
			while ((includes = _cls.Compiled.Descendants(BSharpSyntax.IncludeBlock).Where(_=>_.GetCode()!=BSharpSyntax.IncludeAllModifier).ToArray()).Length != 0) {
				foreach (var i in includes) {
					needReInterpolate = ProcessSimpleInclude(i, needReInterpolate);
				}				
			}
			if (needReInterpolate) {
				InterpolateElements(BSharpSyntax.IncludeInterpolationAncor);
			}
		}

		private bool ProcessSimpleInclude(XElement i, bool needReInterpolate) {
			var code = i.GetCode();
			if (string.IsNullOrWhiteSpace(code)) {
				_context.RegisterError(BSharpErrors.FakeInclude(_cls,i));
				i.Remove();
				return needReInterpolate;
			}

			var includecls = _context.Get(code, _cls.Namespace);
			if (null == includecls) {
				_context.RegisterError(BSharpErrors.NotResolvedInclude(_cls, i));
				i.Remove();
			}else if (includecls.IsOrphaned) {
				_context.RegisterError(BSharpErrors.OrphanInclude(_cls, i));
				i.Remove();
			}
			else {
				Build(BuildPhase.Compile,_compiler, includecls, _context);
				var includeelement = new XElement(includecls.Compiled);
				var usebody = null != i.Attribute(BSharpSyntax.IncludeBodyModifier) || i.GetName() == BSharpSyntax.IncludeBodyModifier;
				var nochild = null != i.Attribute(BSharpSyntax.IncludeNoChildModifier) || i.GetName() == BSharpSyntax.IncludeNoChildModifier;
				needReInterpolate = needReInterpolate || includeelement.HasAttributes(contains: BSharpSyntax.IncludeInterpolationAncor+"{", skipself: usebody);

				if (usebody) {
					var elements = includeelement.Elements().ToArray();
					var wheres = i.Elements(BSharpSyntax.IncludeWhereClause).ToArray();
					if (0 != wheres.Length) {
						var matcher = new XmlTemplateMatcher(wheres);
						elements = elements.Where(matcher.IsMatch).ToArray();
					}
					if (0 == elements.Length) {
						_context.RegisterError(BSharpErrors.EmptyInclude(_cls, i));
					}
					StoreParentParameters(includeelement,i);
					foreach (var e in elements) {
						if (nochild) {
							e.Elements().Remove();
						}
						StoreIncludeParameters(i, e);
					}
					i.ReplaceWith(elements);
				}
				else {
					if (nochild) {
						includeelement.Elements().Remove();
					}
					StoreIncludeParameters(i,includeelement);
					includeelement.Name = includeelement.Attr(BSharpSyntax.ClassFullNameAttribute);
					includeelement.Attribute(BSharpSyntax.ClassFullNameAttribute).Remove();
					includeelement.Attribute(BSharpSyntax.ClassNameAttribute).Remove();
					includeelement.Attribute("id").Remove();
					i.ReplaceWith(includeelement);
				}
			}
			return needReInterpolate;
		}

		private void StoreIncludeParameters(XElement src, XElement trg) {
			foreach (var a in src.Attributes()) {
				if(a.Name.LocalName=="code") continue;
				if (a.Name.LocalName == "name") continue;
				if (a.Name.LocalName == BSharpSyntax.IncludeBodyModifier) continue;
				trg.SetAttributeValue(a.Name,a.Value);
			}
		}
		private void StoreParentParameters(XElement src, XElement trg)
		{
			foreach (var a in src.Attributes())
			{
				if (trg.AncestorsAndSelf().All(_=>null==_.Attribute(a.Name))) {
					trg.SetAttributeValue(a.Name,a.Value);
				}
			}
		}

		private void CleanupElementsWithConditions() {
				_cls.Compiled.Descendants().Where(_ => !IsMatch(_)).Remove();

		}

		private void PerformMergingWithElements() {
			foreach (var root in _cls.AllElements.Where(_ => _.Type == BSharpElementType.Define).ToArray()) {
				var allroots = _cls.Compiled.Elements(root.Name).ToArray();
				var groupedroots = allroots.GroupBy(_ => _.GetCode());
				foreach(var doublers in groupedroots.Where(_=>_.Count()>1)) {
					doublers.Skip(1).Remove();
				}
				var alloverrides =
					_cls.AllElements.Where(_ => _.Type != BSharpElementType.Define && _.TargetName == root.Name).ToArray();
//				foreach (var over in alloverrides) {
					foreach (var g in groupedroots) {
						var e = g.First();
						//реверсировать надо для правильного порядка обхода
						var candidates = e.ElementsBeforeSelf().Reverse().Where(_=>_.GetCode()==g.Key).ToArray();

						foreach (var o in candidates) {
							var over = alloverrides.FirstOrDefault(_ => _.Name == o.Name.LocalName);
							if (null != over) {
								if (over.Type == BSharpElementType.Override) {
									foreach (var a in o.Attributes()) {
										e.SetAttributeValue(a.Name, a.Value);
									}
									if (o.HasElements) {
										e.Elements().Remove();
										e.Add(o.Elements());
									}
								}else if (over.Type == BSharpElementType.Extension) {
									foreach (var a in o.Attributes())
									{
										if (null == e.Attribute(a.Name)) {
											e.SetAttributeValue(a.Name, a.Value);
										}
									}
									if (o.HasElements)
									{
										e.Add(o.Elements());
									}
								}

								o.Remove();	
							}
						}
						
					}
	//			}
			}
			
		}
		private bool CheckExistedLink()
		{
			if (_cls.Is(BSharpClassAttributes.Linked)) return true;
			lock (_cls)
			{
				if (_cls.Is(BSharpClassAttributes.InLink))
				{
					if (null != _cls.BuildTask)
					{
						_cls.BuildTask.Wait();
					}
					else
					{
						for (var i = 0; i <= 10; i++)
						{
							Thread.Sleep(10);
							if (_cls.Is(BSharpClassAttributes.Linked))
							{
								break;
							}
						}
					}
					if (_cls.Is(BSharpClassAttributes.Linked)) return true;
				}
			}
			return false;
		}
		private bool CheckExistedBuild() {
			if (_cls.Is(BSharpClassAttributes.Built)) return true;
			lock (_cls) {
				if (_cls.Is(BSharpClassAttributes.InBuild))
				{
					if (null != _cls.BuildTask) {
						_cls.BuildTask.Wait();
					}
					else {
						for (var i = 0; i <= 10; i++) {
							Thread.Sleep(10);
							if (_cls.Is(BSharpClassAttributes.Built))
							{
								break;
							}
						}
					}
					if (_cls.Is(BSharpClassAttributes.Built)) return true;
				}
			}
			return false;
		}

		private void IntializeMergeIndexes()
		{
			foreach (var m in _cls.AllElements) {
				m.Name = m.Name.ConvertToXNameCompatibleString();
				if (!string.IsNullOrWhiteSpace(m.TargetName)) {
					m.TargetName = m.TargetName.ConvertToXNameCompatibleString();
				}
			}
			var allroots = _cls.AllElements.Where(_ => _.Type == BSharpElementType.Define).Select(_ => _.Name).ToArray();
			foreach (var root in allroots) {
				var defoverride = new BSharpElement { Name = BSharpSyntax.ElementOverridePrefix + root, TargetName = root, Type = BSharpElementType.Override };
				if (!_cls.AllElements.Contains(defoverride)) {
					_cls.AllElements.Add(defoverride);
				}
				var defext = new BSharpElement { Name = BSharpSyntax.ElementExtensionPrefix + root, TargetName = root, Type = BSharpElementType.Extension };
				if (!_cls.AllElements.Contains(defext))
				{
					_cls.AllElements.Add(defext);
				}
			}
		}

		

		private void MergeInternals()
		{//реверс нужен для правильного порядка наката элементов
			
			foreach (var e in _cls.AllImports.Reverse())
			{
				if (e.Is(BSharpClassAttributes.Static)) {
					if (!e.Is(BSharpClassAttributes.Built)) {
						Build(BuildPhase.Compile, _compiler,e,_context);
					}
					_cls.Compiled.Add(e.Compiled.Elements().Where(IsMatch));
				}
				else {
					_cls.Compiled.Add(e.Source.Elements().Where(IsMatch));
				}
			}

			
		}
		
		private bool IsMatch(XElement arg) {
			var cond = arg.Attr(BSharpSyntax.ConditionalAttribute);
			if (string.IsNullOrWhiteSpace(cond)) {
				return true;
			}
			//мы должны пропускать интерполяции, так как сверить их все равно нельзя пока
			if (cond.Contains("${")) return true;
			var compilerOptions = _compiler.GetConditions();
			var srcp = _cls.ParamIndex;
			if (null != compilerOptions) {
				compilerOptions.SetParent(srcp);
				srcp = compilerOptions;
			}
			var src = new DictionaryTermSource(srcp);
			return new LogicalExpressionEvaluator().Eval(cond, src);
		}

		private void CleanupPrivateMembers() {
			var name = _cls.Compiled.GetName();
			if (name == BSharpSyntax.ClassAbstractModifier || name == BSharpSyntax.ClassStaticModifier)
			{
				_cls.Compiled.Attribute("name").Remove();
			}
			_cls.Compiled.Attributes(BSharpSyntax.ClassAbstractModifier).Remove();
			_cls.Compiled.Attributes(BSharpSyntax.ClassStaticModifier).Remove();
			_cls.Compiled.Elements(BSharpSyntax.ClassImportDefinition).Remove();
			_cls.Compiled.Elements(BSharpSyntax.ClassElementDefinition).Remove();
			_cls.Compiled.Descendants(BSharpSyntax.IncludeBlock).Remove();

			foreach (var e in _cls.Compiled.DescendantsAndSelf())
			{
				if (null != e.Attribute("id")) {
					if (e.Attr("code") == e.Attr("id")) {
						e.Attribute("id").Remove();
					}

				}
				if (null != e.Attribute(BSharpSyntax.ConditionalAttribute))
				{
					e.Attribute(BSharpSyntax.ConditionalAttribute).Remove();
				}
				e.Attributes().Where(_ => _.Name.LocalName[0]==BSharpSyntax.PrivateAttributePrefix || string.IsNullOrEmpty(_.Value)).Remove();
			}

			foreach (var m in _cls.AllElements.Where(_ => _.Type != BSharpElementType.Define).Select(_ => _.Name).Distinct()) {
				_cls.Compiled.Elements(m).Remove();
			}

		}

		private void InitializeBuildIndexes()
		{
			_cls.Compiled = new XElement(_cls.Source);
			_cls.Compiled.Elements(BSharpSyntax.ClassImportDefinition).Remove();
			_cls.Compiled.Elements(BSharpSyntax.ClassElementDefinition).Remove();
			_cls.ParamSourceIndex = BuildParametersConfig();
			_cls.ParamIndex = new ConfigBase();
			_cls.Compiled.SetAttributeValue(BSharpSyntax.ClassFullNameAttribute, _cls.FullName);
			foreach (var p in _cls.ParamSourceIndex)
			{
				_cls.ParamIndex.Set(p.Key, p.Value);
			}

		}

		/// <summary>
		///     Возвращает XML для резолюции атрибутов
		/// </summary>
		/// <returns></returns>
		private IConfig BuildParametersConfig()
		{

			var result = new ConfigBase();
			ConfigBase current = result;
			foreach (var i in _cls.AllImports.Union(new[] { _cls }))
			{
				var selfconfig = new ConfigBase();
				selfconfig.Set("_class_", _cls.FullName);
				selfconfig.SetParent(current);
				current = selfconfig;
				if (i.Is(BSharpClassAttributes.Static) && _cls != i)
				{
					if (!i.Is(BSharpClassAttributes.Built)) {
						Build(BuildPhase.Compile, _compiler, i, _context);
					}
					foreach (var p in i.ParamIndex)
					{
						current.Set(p.Key, p.Value);
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

		private  void BindParametersToCompiledClass()
		{
			foreach (var e in _cls.ParamIndex)
			{
				_cls.Compiled.SetAttributeValue(e.Key, e.Value.ToStr());
			}
		}

		private void InterpolateElements(char ancor='$')
		{
			if (GetConfig().UseInterpolation)
			{
				var xi = new XmlInterpolation{AncorSymbol = ancor};
				xi.Interpolate(_cls.Compiled);
			}
		}

		private void InterpolateFields()
		{
			if (GetConfig().UseInterpolation)
			{
				var si = new StringInterpolation();

				for (int i = 0; i <= 3; i++)
				{
					foreach (var v in _cls.ParamIndex.ToArray()) {
						var s = v.Value as string;
						if (null == s) continue;
						if (-1 == s.IndexOf('{')) continue;
						_cls.ParamIndex.Set(v.Key, si.Interpolate(s, _cls.ParamSourceIndex));
					}
				}
			}
		}

	}
}