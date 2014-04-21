using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.BSharp.Matcher;
using Qorpent.Config;
using Qorpent.Log;
using Qorpent.LogicalExpressions;
using Qorpent.Serialization;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.LogicalExpressions;
using Qorpent.Utils.XDiff;

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
		public static void Build(BuildPhase phase, IBSharpCompiler compiler, IBSharpClass cls, IBSharpContext context){
			var _cls = cls as BSharpClass;
			if (null == _cls){
				new BSharpClassBuilder(compiler,cls,context).Build(phase);
				return;
			}
			
			if (null ==_cls.Builder){
				_cls.Builder =  new BSharpClassBuilder(compiler, _cls, context);
			}
			_cls.Builder.Build(phase);
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
			
		
				if (BuildPhase.Compile == phase) {
					PerformCompilation();
				}
				else if (BuildPhase.AutonomeLink == phase) {
					PerformAutonomeLinking();
				}

                else if (BuildPhase.CrossClassLink == phase)
                {
                    PerformCrossClassLinking();
                }

				else if (BuildPhase.ApplyPatch == phase){
					ApplyPatch();
				}
			
		}

        private void PerformAutonomeLinking()
        {
			if (CheckExistedLink()) return;
			_cls.Set(BSharpClassAttributes.InLink);
			InternalAutonomeLink();
			_cls.Remove(BSharpClassAttributes.InLink);
			//_cls.Set(BSharpClassAttributes.Linked);
		}
        private void PerformCrossClassLinking()
        {
            if (CheckExistedLink()) return;
            _cls.Set(BSharpClassAttributes.InLink);
            InternalCrossClassLink();
            _cls.Remove(BSharpClassAttributes.InLink);
            _cls.Set(BSharpClassAttributes.Linked);
        }

        private void InternalAutonomeLink()
        {
            ResolveClassReferences();
		}
        private void InternalCrossClassLink()
        {
            ResolveAdvancedIncludes();
            ResolveDictionaries();

        }

		private void ApplyPatch(){
			if (string.IsNullOrWhiteSpace(_cls.PatchTarget) || _cls.PatchTarget.Contains("NOTRESOLVED::") ||
				_cls.PatchTarget.Contains("AMBIGOUS::")){
				_context.RegisterError(BSharpErrors.PatchUndefinedTarget(_cls,_cls.Compiled));					
				return;
			}
			if (_cls.PatchBehavior == BSharpPatchBehavior.Invalid){
				_context.RegisterError(BSharpErrors.PatchInvalidBehavior(_cls, _cls.Compiled));
				return;
					
			}
			var targets = _context.ResolveAll(_cls.PatchTarget,_cls.Namespace);
			

			foreach (var target in targets){
				lock (target){
					Log.Trace("Apply patch "+_cls.FullName+" to "+target.FullName);
					try{
						var difference = EvalDiff(target);
						Log.Debug(difference.LogToString());
						
					}
					catch (Exception ex){
						_context.RegisterError(BSharpErrors.PatchError(_cls,target,ex.Message,ex));
					}

				}
				
			}
		}

		private IEnumerable<XDiffItem> EvalDiff(IBSharpClass target){
			var opts = new XDiffOptions{
				IsHierarchy = true,
				IncludeActions =
					XDiffAction.ChangeAttribute | XDiffAction.CreateAttribute | XDiffAction.CreateElement | XDiffAction.ChangeElement |
					XDiffAction.ChangeHierarchyPosition
			};
			if (_cls.PatchBehavior == BSharpPatchBehavior.ErrorOnNew){
				opts.ErrorActions |= XDiffAction.CreateElement;
			}
			else if (_cls.PatchBehavior == BSharpPatchBehavior.NoneOnNew){
				opts.IncludeActions &= ~XDiffAction.CreateElement;
			}
			var diff = new XDiffGenerator(opts);

			var difference = diff.GetDiff(target.Compiled, _cls.Compiled).ToArray();
			difference.Apply(target.Compiled, opts);
			foreach (var e in target.Compiled.DescendantsAndSelf()){
				e.SetAttributeValue("__parent",null);
			}
			return difference;
		}

		private void ResolveAdvancedIncludes() {
			if (!_cls.Is(BSharpClassAttributes.RequireAdvancedIncludes)) return;
			XElement[] includes = null;
			while (0!=(includes =
			       _cls.Compiled.Descendants(BSharpSyntax.IncludeBlock)
			           .Where(_ => _.GetCode()==BSharpSyntax.IncludeAllModifier && null==_.Attribute(BSharpSyntax.NoProcessDirective))
			           .ToArray()).Length) {
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
					ProcessAdvancedInclude(i, sources);
				}
			}
		}

		private void ProcessAdvancedInclude( XElement i, IBSharpClass[] sources) {
		    var _i = i;//на случай группировки дублируем
            var usebody = null != _i.Attribute(BSharpSyntax.IncludeBodyModifier) || _i.GetName() == BSharpSyntax.IncludeBodyModifier;
			var stub = new XElement("_");
		    XElement stubi = null;
            if (usebody && null != _i.Element(BSharpSyntax.IncludeGroupByClause) ||
                null != _i.Element(BSharpSyntax.IncludeOrderByClause)) {
                stubi= new XElement(_i);
                _i = new XElement(_i);
                foreach (var e in _i.Elements().ToArray()) {
                    if (e.Name.LocalName != BSharpSyntax.IncludeWhereClause) {
                        e.Remove();
                    }
                    else {
                        stubi.Element(BSharpSyntax.IncludeWhereClause).Remove();
                    }
                }
                
            }
			foreach (var s in sources) {
			    var __cls = _cls as BSharpClass;
                if (null != __cls && !__cls.LateIncludedClasses.Contains(s)) {
                    __cls.LateIncludedClasses.Add(s);
                }
				var includeelement = new XElement(s.Compiled);
				
				var nochild = null != _i.Attribute(BSharpSyntax.IncludeNoChildModifier) || _i.GetName() == BSharpSyntax.IncludeNoChildModifier;
				if (usebody)
				{
					var elements = ExtractIncludeBody(_i, includeelement, nochild);
					stub.Add(elements);
				}
				else
				{
					ExctractIncludeClass(_i, nochild, includeelement,s);
					if (IsIncludeClassMatch(_i, includeelement)) {
						stub.Add(includeelement);
					}
				}
			}
            if (usebody && null != stubi) {
                var elements = ExtractIncludeBody(stubi, stub, false);
                stub = new XElement("_",elements);
            }
			i.ReplaceWith(stub.Elements());
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
                    var __cls = _cls as BSharpClass;
                    if (null != __cls && !__cls.ReferencedDictionaries.Contains(code))
                    {
                        __cls.ReferencedDictionaries.Add(code);
                    }
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
			DownFallEmbedAttribute();
			IntializeMergeIndexes();
			InterpolateFields();
			BindParametersToCompiledClass();
			CleanupElementsWithConditions();
			MergeInternals();
			InterpolateElements();
			PerformMergingWithElements();
			CleanupElementsWithConditions();
            ExecutePreSimpleIncludeExtensions();
			ProcessSimpleIncludes();
			CleanupPrivateMembers();
			CheckoutRequireLinkingRequirements();
		}
		/// <summary>
		/// we require implicitly descent embed state
		/// </summary>
		private void DownFallEmbedAttribute(){
			if(_cls.Is(BSharpClassAttributes.Embed))return;
			if (_cls.AllImports.Any(_ => _.Is(BSharpClassAttributes.Embed))){
				_cls.Set(BSharpClassAttributes.Embed);
			}
		}

		private void ExecutePreSimpleIncludeExtensions() {
	        _compiler.CallExtensions(_cls, _context, BSharpCompilePhase.PreSimpleInclude);
	    }

	    private void CheckoutRequireLinkingRequirements(){
		    bool ispureEmbed = _cls.Is(BSharpClassAttributes.Embed) && !_cls.Is(BSharpClassAttributes.Patch);
		    if (ispureEmbed) return;
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
			if (_cls.Compiled.DescendantsAndSelf().Any(_ => _.Attributes().Any(__ => __.Value.Contains("%{")))) {
				_cls.Set(BSharpClassAttributes.RequireLateInterpolation);
			}
		}


		private void ResolveClassReferences() {
			if (!_cls.Is(BSharpClassAttributes.RequireClassResolution)) return;
			//найдем все атрибуты, начинающиеся на ^
			foreach (var a in _cls.Compiled.DescendantsAndSelf().SelectMany(_ => _.Attributes())) {
				if (a.Value[0]=='^') {
					var clsname = a.Value.Substring(1);
					var isarray = clsname.EndsWith("*");
					if (isarray){
						clsname = clsname.Substring(0, clsname.Length - 1);
					}
					var normallyresolvedClass = _context.Get(clsname, _cls.Namespace);
					if (null != normallyresolvedClass) {
						a.Value = normallyresolvedClass.FullName;
						if (isarray){
							a.Value += "*";
						}
                        var __cls = _cls as BSharpClass;
                        if (null != __cls && !__cls.ReferencedClasses.Contains(normallyresolvedClass))
                        {
                            __cls.ReferencedClasses.Add(normallyresolvedClass);
                        }
					}
					else {
						var candidates = _context.Get(BSharpContextDataType.Working).Where(_ => _.FullName.EndsWith(clsname)).ToArray();
						if (!candidates.Any()) {
							a.Value = "NOTRESOLVED::" + clsname;
							_context.RegisterError(BSharpErrors.NotResolvedClassReference(_cls, a.Parent,clsname));
						}else if (1 == candidates.Count()) {
							a.Value = candidates[0].FullName;
						    var start = candidates[0].FullName.Replace( clsname,"");
						    if (!_cls.FullName.StartsWith(start)) {
						        _context.RegisterError(BSharpErrors.NotDirectClassReference(_cls, a.Parent, clsname));
						    }
						}
						else {
							a.Value = "AMBIGOUS::" + clsname;
							_context.RegisterError(BSharpErrors.AmbigousClassReference(_cls, a.Parent, clsname));
						}
					}
				}
			}
		}

		private void 
            ProcessSimpleIncludes() {
			XElement[] includes;
			var needReInterpolate = false;
			while ((includes = _cls.Compiled.Descendants(BSharpSyntax.IncludeBlock).Where(_=>_.GetCode()!=BSharpSyntax.IncludeAllModifier && null==_.Attribute(BSharpSyntax.NoProcessDirective)).ToArray()).Length != 0) {
				
				foreach (var i in includes){
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
                var __cls = _cls as BSharpClass;
                if (null != __cls && !__cls.IncludedClasses.Contains(includecls))
                {
                    __cls.IncludedClasses.Add(includecls);
                }
                
			    var includeelement = new XElement(includecls.Compiled);
                
			    var usebody = null != i.Attribute(BSharpSyntax.IncludeBodyModifier) || i.GetName() == BSharpSyntax.IncludeBodyModifier;
				var nochild = null != i.Attribute(BSharpSyntax.IncludeNoChildModifier) || i.GetName() == BSharpSyntax.IncludeNoChildModifier;
				needReInterpolate = needReInterpolate || includeelement.HasAttributes(contains: BSharpSyntax.IncludeInterpolationAncor+"{", skipself: usebody);

				if (usebody) {
					var elements = ExtractIncludeBody(i, includeelement, nochild);
					i.ReplaceWith(elements);
				}
				else {
					ExctractIncludeClass(i, nochild, includeelement, includecls);
                    var ename = i.Attr(BSharpSyntax.IncludeElementNameModifier);
                    if (!string.IsNullOrWhiteSpace(ename)) {
                        includeelement.Name = ename;
                        if (i.Attr(BSharpSyntax.IncludeKeepCodeModifier).ToBool()) {
                            includeelement.SetAttributeValue("code", includecls.Name);
                        }
                    }

					if (IsIncludeClassMatch(i, includeelement)) {
						i.ReplaceWith(includeelement);
					}
					else {
						i.Remove();
					}
				}
			}
			return needReInterpolate;
		}

		private bool IsIncludeClassMatch(XElement include, XElement includeclass) {
			var wheres = include.Elements(BSharpSyntax.IncludeWhereClause);
			if (wheres.Any())
			{
				var matcher = new XmlTemplateMatcher(wheres);
				return matcher.IsMatch(includeclass);
				
			}
			return true;
		}

		private XmlInterpolation _lateincluder = new XmlInterpolation{AncorSymbol = '%'};
		private void ExctractIncludeClass(XElement i, bool nochild, XElement includeelement, IBSharpClass cls) {
			if (nochild) {
				includeelement.Elements().Remove();
			}
			foreach (var source in i.Attributes().Where(_=>_.Value[0]==BSharpSyntax.ClassReferencePrefix)) {
				source.Value = _context.Get(source.Value.Substring(1)).FullName;
			}
			StoreIncludeParameters(i, includeelement);
			includeelement.Name = includeelement.Attr(BSharpSyntax.ClassFullNameAttribute);
			includeelement.Attribute(BSharpSyntax.ClassFullNameAttribute).Remove();
			includeelement.Attribute(BSharpSyntax.ClassNameAttribute).Remove();
			var a = includeelement.Attribute("id");

			if (null != a) a.Remove();
			var sc = includeelement.Attribute("set-code");
			if (null != sc){
				includeelement.SetAttributeValue("code",sc.Value);
				sc.Remove();
			}
			var sen = includeelement.Attribute("set-elname");
			if (null != sen){
				includeelement.Name = sen.Value;
				sen.Remove();
			}
			if (cls.Is(BSharpClassAttributes.RequireLateInterpolation)) {
				_lateincluder.Interpolate(includeelement, i);
			}
			
		}

		private XElement[] ExtractIncludeBody(XElement i, XElement includeelement, bool nochild) {
		    var elements = includeelement.Elements();
		    var wheres = i.Elements(BSharpSyntax.IncludeWhereClause);
			if (wheres.Any()) {
				var matcher = new XmlTemplateMatcher(wheres);
				elements = elements.Where(matcher.IsMatch).ToArray();
			}
			if (!elements.Any()) {
				_context.RegisterError(BSharpErrors.EmptyInclude(_cls, i));
			}
            
            StoreParentParameters(includeelement, i);
			foreach (var e in elements) {
				if (nochild) {
					e.Elements().Remove();
				}
				StoreIncludeParameters(i, e);
			}
            var orderby = i.Element(BSharpSyntax.IncludeOrderByClause);
            if (null != orderby) {
                elements = elements.OrderBy(_ => _.Attr(orderby.Attr("code"))).ToArray();
            }
		    var select = i.Element(BSharpSyntax.IncludeSelectClause);
            var grp = i.Element(BSharpSyntax.IncludeGroupByClause);
            if (null != select && null!=grp) {
                if (null == select.Attribute(grp.Attr("code"))) {
                    select.SetAttributeValue(grp.Attr("code"), "false");
                }
            }
            if (null != grp) {
                var ename = grp.Attr("as", "group");
                var gattr = grp.Attr("with", "code");
                var gelements = elements.GroupBy(_ => _.Attr(grp.Attr("code"))).ToArray();
                var groups = new List<XElement>();
                foreach (var e in gelements) {
                    
                    var grpe = new XElement(ename, new XAttribute(gattr,e.Key));
                    var els = e.ToArray();
                    if (null != select)
                    {
                        foreach (var el in els) {
                            FilterWithSelect(el,select);
                        }
                    }
                    grpe.Add(els);
                    groups.Add(grpe);
                }
                return groups.ToArray();

            }
            else {
                if (null != select)
                {
                    foreach (var el in elements)
                    {
                        FilterWithSelect(el, select);
                    }
                }
                return elements.ToArray();    
            }
		}

	    private void FilterWithSelect(XElement e, XElement select) {
	        foreach (var a in e.Attributes().ToArray()) {
	            if (a.Name.LocalName == "_file") continue;
                if (a.Name.LocalName == "_line") continue;
	            var sa = select.Attribute(a.Name.LocalName);
                if (null == sa || "false"==sa.Value || "0"==sa.Value) {
                    a.Remove();
                    continue;
                }
                if (sa.Value == "true" || sa.Value == "1") {
                    continue;
                }
	            e.SetAttributeValue(sa.Value,a.Value);
	            a.Remove();
	        }
	    }

	    private void StoreIncludeParameters(XElement src, XElement trg) {
			foreach (var a in src.Attributes()) {
				if(a.Name.LocalName=="code") continue;
				if (a.Name.LocalName == "name") continue;
				if (a.Name.LocalName == BSharpSyntax.IncludeBodyModifier) continue;
                if (a.Name.LocalName == BSharpSyntax.IncludeNoChildModifier) continue;
                if (a.Name.LocalName == BSharpSyntax.IncludeElementNameModifier) continue;
                    if (a.Name.LocalName.StartsWith("__TILD__")) {
                        trg.SetAttributeValue(a.Name.LocalName.Substring("__TILD__".Length), a.Value);
                    } else if (a.Name.LocalName.StartsWith("__PLUS__")) {
                        var localName = a.Name.LocalName.Substring("__PLUS__".Length);
                        if (trg.Attribute(localName) == null) {
                            trg.SetAttributeValue(localName, a.Value);
                        }
                    } else {
                        if (null == trg.Attribute(a.Name.LocalName)) {
                            trg.SetAttributeValue(a.Name, a.Value);
                        }
                    }
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
				var allroots = _cls.Compiled.Descendants(root.Name).ToArray();
				var groupedroots = allroots.GroupBy(_ => _.GetCode());
				foreach(var doublers in groupedroots.Where(_=>_.Count()>1)) {
					doublers.Skip(1).Remove();
				}

				var alloverrides =
					_cls.AllElements.Where(_ => _.Type != BSharpElementType.Define && _.TargetName == root.Name).ToArray();
                //если нет целевых элементов, то не обрабатываем мержи
                if (!_cls.Compiled.Elements().Any(_ => alloverrides.Any(__ => __.Name == _.Name.LocalName))) {
                    continue;
                }
//				foreach (var over in alloverrides) {
					foreach (var g in groupedroots) {
						var e = g.First();
						//реверсировать надо для правильного порядка обхода
					    XElement[] candidates;
					    if (e.Parent == _cls.Compiled) {
					        candidates = e.ElementsBeforeSelf().Reverse().Where(_ => _.GetCode() == g.Key).ToArray();
					    }
					    else {
                            candidates = _cls.Compiled.Elements().Reverse().Where(_ => _.GetCode() == g.Key).ToArray();
					    }
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
									if (!string.IsNullOrWhiteSpace(o.Value)){
										e.Value = o.Value;
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
									if (!string.IsNullOrWhiteSpace(o.Value)){
										//join embeded code
										if (o.Value[0]=='(' && o.Value[o.Value.Length-1]==')' && e.Value[0]=='(' && e.Value[e.Value.Length]==')'){
											e.Value = e.Value.Substring(0, e.Value.Length - 1) + o.Value.Substring(1);
										}
										else{
											e.Value += o.Value;
										}
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
			if (_cls.Is(BSharpClassAttributes.InLink))
			{
				for (var i = 0; i <= 10; i++)
				{
					Thread.Sleep(5);
					if (_cls.Is(BSharpClassAttributes.Linked))
					{
						break;
					}
				}	
			}
			return _cls.Is(BSharpClassAttributes.Linked);
		}
		private bool CheckExistedBuild() {
			if (_cls.Is(BSharpClassAttributes.Built)) return true;
			if (_cls.Is(BSharpClassAttributes.InBuild))
			{					
					for (var i = 0; i <= 10; i++) {
						Thread.Sleep(5);
						if (_cls.Is(BSharpClassAttributes.Built))
						{
							break;
						}
					}
	
			}
			return _cls.Is(BSharpClassAttributes.Built);
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
					_cls.Compiled.Add(e.Compiled.Elements().Where(IsMatch));
				}
				else {
					_cls.Compiled.Add(e.Source.Elements().Where(IsMatch));
				}
			}

			
		}
		
		private bool IsMatch(XElement arg){
			//due to fact that generics just prepare template for override classes it doesn't exclude 
			//anything from elements due to conditions
			if (_cls.Is(BSharpClassAttributes.Generic)) return true;
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
			var src = new DictionaryTermSource<object>(srcp);
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
			_cls.Compiled.Attributes(BSharpSyntax.ClassGenericModifier).Remove();
			_cls.Compiled.Elements(BSharpSyntax.ClassImportDefinition).Remove();
			_cls.Compiled.Elements(BSharpSyntax.ClassElementDefinition).Remove();
			
			foreach (var e in _cls.Compiled.DescendantsAndSelf())
			{
				if (null != e.Attribute("id")) {
					if (e.Attr("code") == e.Attr("id")) {
						e.Attribute("id").Remove();
					}

				}
				//generics MUST keep "if" attribute alive
				if (!_cls.Is(BSharpClassAttributes.Generic)){
					if (null != e.Attribute(BSharpSyntax.ConditionalAttribute)){
						e.Attribute(BSharpSyntax.ConditionalAttribute).Remove();
					}
				}
				e.Attributes().Where(_ =>
					(_.Name.LocalName[0]==BSharpSyntax.PrivateAttributePrefix || string.IsNullOrEmpty(_.Value))&&
					!(_.Name.LocalName.StartsWith("__AT__"))
					).Remove();
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
			current.Stornate();
			return current;
		}

		private  void BindParametersToCompiledClass()
		{
			foreach (var e in _cls.ParamIndex)
			{
				_cls.Compiled.SetAttributeValue(e.Key, e.Value.ToStr());
			}
		}

		private void InterpolateElements(char ancor = '\0'){
			if (ancor == '\0'){
				ancor = _cls.Is(BSharpClassAttributes.Generic) ? '`' : '$';
			}
			
			if (GetConfig().UseInterpolation)
			{
				
				var xi = new XmlInterpolation{AncorSymbol = ancor};
				xi.Interpolate(_cls.Compiled);
			}
		}


		private void InterpolateFields()
		{
			// у генериков на этой фазе еще производится полная донастройка элементов по анкору ^
			
			if (GetConfig().UseInterpolation)
			{
				var si = new StringInterpolation();
				si.AncorSymbol = _cls.Is(BSharpClassAttributes.Generic) ? '`' : '$';
				bool requireInterpolateNames = _cls.ParamIndex.Keys.Any(_ => _.Contains("__LBLOCK__"));
				while (true){
					bool changed = false;
					foreach (var v in _cls.ParamIndex.ToArray()){
						var key = v.Key;
						if (requireInterpolateNames){
							var esckey = key.Unescape(EscapingType.XmlName);
							if (-1 != esckey.IndexOf('{')){
								var _key = si.Interpolate(esckey, _cls.ParamSourceIndex).Escape(EscapingType.XmlName);
								if (_key != key){
									changed = true;
									_cls.ParamIndex.Remove(key);
									_cls.ParamIndex[_key] = v.Value;
									key = _key;
								}
							}
						}
						var s = v.Value as string;
						if (null == s) continue;
						if (-1 == s.IndexOf('{')) continue;
						var newval = si.Interpolate(s, _cls.ParamSourceIndex, _compiler.Global);
						if (newval != s){
							changed = true;
							_cls.ParamIndex.Set(key, newval);
						}
					}
					if(!changed)break;
				}
				
			}
		}

	}
}