using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.BSharp.Matcher;
using Qorpent.Log;
using Qorpent.LogicalExpressions;
using Qorpent.Serialization;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.LogicalExpressions;
using Qorpent.Utils.XDiff;

namespace Qorpent.BSharp{
	/// <summary>
	/// </summary>
	public class BSharpClassBuilder{
		private readonly IBSharpClass _cls;
		private readonly IBSharpCompiler _compiler;
		private readonly IBSharpContext _context;
		private readonly XmlInterpolation _lateincluder = new XmlInterpolation{AncorSymbol = '%'};

		/// <summary>
		/// </summary>
		/// <param name="bSharpCompiler"></param>
		/// <param name="cls"></param>
		/// <param name="context"></param>
		protected BSharpClassBuilder(IBSharpCompiler bSharpCompiler, IBSharpClass cls, IBSharpContext context){
			_compiler = bSharpCompiler;
			_cls = cls;
			_context = context;
		}


		private IUserLog Log{
			get { return _compiler.GetConfig().Log; }
		}

		private IBSharpConfig GetConfig(){
			return _compiler.GetConfig();
		}

		/// <summary>
		/// </summary>
		/// <param name="phase"></param>
		/// <param name="compiler"></param>
		/// <param name="cls"></param>
		/// <param name="context"></param>
		public static void Build(BuildPhase phase, IBSharpCompiler compiler, IBSharpClass cls, IBSharpContext context){
			var _cls = cls as BSharpClass;
			if (null == _cls){
				new BSharpClassBuilder(compiler, cls, context).Build(phase);
				return;
			}

			if (null == _cls.Builder){
				_cls.Builder = new BSharpClassBuilder(compiler, _cls, context);
			}
			_cls.Builder.Build(phase);
		}

		/// <summary>
		/// </summary>
		/// <param name="phase"></param>
		public void Build(BuildPhase phase){
			if (BuildPhase.Compile == phase){
				PerformCompilation();
			}
			else if (BuildPhase.AutonomeLink == phase){
				PerformAutonomeLinking();
			}

			else if (BuildPhase.CrossClassLink == phase){
				PerformCrossClassLinking();
			}

			else if (BuildPhase.Evaluate == phase){
				PerformEvaluation();
			}
			else if (BuildPhase.ApplyPatch == phase){
				ApplyPatch();
			}
			else if (BuildPhase.PostProcess == phase){
				DoPostProcess();
			}
		}

		private void DoPostProcess(){
			XElement[] selects = _cls.Compiled.Descendants(BSharpSyntax.PostProcessSelectElements).ToArray();
			foreach (XElement e in selects){
				string xpath = e.Attr("xpath");
				XElement[] elements = _cls.Compiled.XPathSelectElements(xpath).Select(_ => new XElement(_)).ToArray();
				if (0 != elements.Length){
					e.ReplaceWith(elements);
				}
				else{
					e.Remove();
				}
			}
			XElement[] removebefores = _cls.Compiled.Descendants(BSharpSyntax.PostProcessRemoveBefore).ToArray();
			foreach (XElement removebefore in removebefores){
				string code = removebefore.Attr("code");
				removebefore.ElementsBeforeSelf(code).ToArray().Remove();
				removebefore.Remove();
			}
		}

		/// <summary>
		/// </summary>
		private void PerformEvaluation(){
			foreach (BSharpEvaluation definition in _cls.AllEvaluations){
				definition.Evaluate(_cls);
			}
		}

		private void PerformAutonomeLinking(){
			if (CheckExistedLink()) return;
			_cls.Set(BSharpClassAttributes.InLink);
			InternalAutonomeLink();
			_cls.Remove(BSharpClassAttributes.InLink);
			//_cls.Set(BSharpClassAttributes.Linked);
		}

		private void PerformCrossClassLinking(){
			if (CheckExistedLink()) return;
			_cls.Set(BSharpClassAttributes.InLink);
			InternalCrossClassLink();
			_cls.Remove(BSharpClassAttributes.InLink);
			_cls.Set(BSharpClassAttributes.Linked);
		}

		private void InternalAutonomeLink(){
			ResolveClassReferences();
		}

		private void InternalCrossClassLink(){
			ResolveAdvancedIncludes();
			ResolveDictionaries();
		}

		private void ApplyPatch(){
			if (string.IsNullOrWhiteSpace(_cls.PatchTarget) || _cls.PatchTarget.Contains("NOTRESOLVED::") ||
			    _cls.PatchTarget.Contains("AMBIGOUS::")){
				_context.RegisterError(BSharpErrors.PatchUndefinedTarget(_cls, _cls.Compiled));
				return;
			}
			if (_cls.PatchCreateBehavior == BSharpPatchCreateBehavior.Invalid){
				_context.RegisterError(BSharpErrors.PatchInvalidBehavior(_cls, _cls.Compiled));
				return;
			}
			IEnumerable<IBSharpClass> targets = _context.ResolveAll(_cls.PatchTarget, _cls.Namespace);


			foreach (IBSharpClass target in targets){
				if (null == _cls.Compiled){
					if (_cls.PatchPhase != BSharpPatchPhase.Before){
						Build(BuildPhase.Compile);
					}
					if (_cls.PatchPhase == BSharpPatchPhase.After){
						Build(BuildPhase.AutonomeLink);
						Build(BuildPhase.CrossClassLink);
					}
				}
				lock (target){
					Log.Trace("Apply patch " + _cls.FullName + " to " + target.FullName);
					try{
						EvalDiff(target);
						//var difference = EvalDiff(target);
						//Log.Debug(difference.LogToString());
					}
					catch (Exception ex){
						_context.RegisterError(BSharpErrors.PatchError(_cls, target, ex.Message, ex));
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
			if (target.PatchNameBehavior == BSharpPatchNameBehavior.Match){
				opts.IsNameIndepended = false;
			}
			if (_cls.PatchPlain){
				opts.IncludeActions = opts.IncludeActions & ~XDiffAction.ChangeHierarchyPosition;
			}
			if (_cls.PatchCreateBehavior == BSharpPatchCreateBehavior.ErrorOnNew){
				opts.ErrorActions |= XDiffAction.CreateElement;
			}
			else if (_cls.PatchCreateBehavior == BSharpPatchCreateBehavior.NoneOnNew){
				opts.IncludeActions &= ~XDiffAction.CreateElement;
			}
			var diff = new XDiffGenerator(opts);
			IEnumerable<XDiffItem> difference;
			if (_cls.PatchPhase == BSharpPatchPhase.Before){
				difference = diff.GetDiff(target.Source, _cls.Source);

				difference.Apply(target.Source, opts);
			}
			else{
				difference = diff.GetDiff(target.Compiled, _cls.Compiled);

				difference.Apply(target.Compiled, opts);
			}
			XElement attrsrc = _cls.Compiled;
			if (_cls.PatchPhase == BSharpPatchPhase.Before){
				attrsrc = _cls.Source;
			}
			foreach (XAttribute attribute in  attrsrc.Attributes()){
				if (attribute.Name.LocalName == BSharpSyntax.PatchTargetAttribute) continue;
				if (attribute.Name.LocalName == "id") continue;
				if (attribute.Name.LocalName == "code") continue;
				if (attribute.Name.LocalName == "name") continue;
				if (attribute.Name.LocalName == "fullcode") continue;
				if (attribute.Name.LocalName == BSharpSyntax.PatchPlainAttribute) continue;
				if (attribute.Name.LocalName == BSharpSyntax.PatchCreateBehavior) continue;
				if (attribute.Name.LocalName == BSharpSyntax.PatchNameBehavior) continue;
				if (attribute.Name.LocalName == BSharpSyntax.PatchBeforeAttribute) continue;
				if (attribute.Name.LocalName == BSharpSyntax.PatchAfterBuildAttribute) continue;
				if (attribute.Name.LocalName == BSharpSyntax.PatchAfterAttribute) continue;
				if (attribute.Name.LocalName == "priority") continue;
				string name = attribute.Name.LocalName;
				if (name.StartsWith("set-")){
					name = name.Substring(4);
				}
				if (_cls.PatchPhase == BSharpPatchPhase.Before){
					target.Source.SetAttributeValue(name, attribute.Value);
				}
				else{
					target.Compiled.SetAttributeValue(name, attribute.Value);
				}
			}

			foreach (XElement descendant in target.Compiled.Descendants()){
				XAttribute p = descendant.Attribute("__parent");
				if (null != p) p.Remove();
			}

			return difference;
		}

		private void ResolveAdvancedIncludes(){
			if (!_cls.Is(BSharpClassAttributes.RequireAdvancedIncludes)) return;
			XElement[] includes = null;
			while (0 != (includes =
			             _cls.Compiled.Descendants(BSharpSyntax.IncludeBlock)
			                 .Where(
				                 _ =>
				                 _.GetCode() == BSharpSyntax.IncludeAllModifier &&
				                 null == _.Attribute(BSharpSyntax.NoProcessDirective))
			                 .ToArray()).Length){
				foreach (XElement i in includes){
					string query = i.GetName();
					if (query == BSharpSyntax.IncludeBodyModifier || query == BSharpSyntax.IncludeNoChildModifier){
						query = "";
					}
					IBSharpClass[] sources = _context.ResolveAll(query, basens: _cls.Namespace).ToArray();
					if (0 == sources.Length){
						i.Remove();
						continue;
					}
					ProcessAdvancedInclude(i, sources);
				}
			}
		}

		private void ProcessAdvancedInclude(XElement i, IBSharpClass[] sources){
			XElement _i = i; //на случай группировки дублируем
			bool usebody = null != _i.Attribute(BSharpSyntax.IncludeBodyModifier) ||
			               _i.GetName() == BSharpSyntax.IncludeBodyModifier;
			var stub = new XElement("_");
			XElement stubi = null;
			if (usebody && null != _i.Element(BSharpSyntax.IncludeGroupByClause) ||
			    null != _i.Element(BSharpSyntax.IncludeOrderByClause)){
				stubi = new XElement(_i);
				_i = new XElement(_i);
				foreach (XElement e in _i.Elements().ToArray()){
					if (e.Name.LocalName != BSharpSyntax.IncludeWhereClause){
						e.Remove();
					}
					else{
						stubi.Element(BSharpSyntax.IncludeWhereClause).Remove();
					}
				}
			}
			foreach (IBSharpClass s in sources){
				var __cls = _cls as BSharpClass;
				if (null != __cls && !__cls.LateIncludedClasses.Contains(s)){
					__cls.LateIncludedClasses.Add(s);
				}
				var includeelement = new XElement(s.Compiled);

				bool nochild = null != _i.Attribute(BSharpSyntax.IncludeNoChildModifier) ||
				               _i.GetName() == BSharpSyntax.IncludeNoChildModifier;
				if (usebody){
					XElement[] elements = ExtractIncludeBody(_i, includeelement, nochild);
					stub.Add(elements);
				}
				else{
					ExctractIncludeClass(_i, nochild, includeelement, s);
					if (IsIncludeClassMatch(_i, includeelement)){
						stub.Add(includeelement);
					}
				}
			}
			if (usebody && null != stubi){
				XElement[] elements = ExtractIncludeBody(stubi, stub, false);
				stub = new XElement("_", elements);
			}
			i.ReplaceWith(stub.Elements());
		}

		private void ResolveDictionaries(){
			if (!_cls.Is(BSharpClassAttributes.RequireDictionaryResolution)) return;
			foreach (XAttribute a in _cls.Compiled.DescendantsAndSelf().SelectMany(_ => _.Attributes())){
				string val = a.Value;
				if (string.IsNullOrWhiteSpace(val) || val.Length < 2) continue;
				if (val[0] == BSharpSyntax.DictionaryReferencePrefix){
					bool valueonly = false;
					bool canbeignored = false;
					val = val.Substring(1);
					if (val[0] == BSharpSyntax.DictionaryReferenceValueOnlyModifier){
						valueonly = true;
						val = val.Substring(1);
					}
					if (val.Length == 0){
						continue;
					}
					if (val[0] == BSharpSyntax.DictionaryReferenceOptionalModifier){
						canbeignored = true;
						val = val.Substring(1);
					}
					if (val.Length == 0) continue;
					int fstdot = val.IndexOf(BSharpSyntax.DictionaryCodeElementDelimiter);
					string code = val.Substring(0, fstdot);
					string element = val.Substring(fstdot + 1);
					var __cls = _cls as BSharpClass;
					if (null != __cls && !__cls.ReferencedDictionaries.Contains(code)){
						__cls.ReferencedDictionaries.Add(code);
					}
					string result = _context.ResolveDictionary(code, element);
					if (null == result){
						if (canbeignored){
							a.Value = "";
						}
						else{
							a.Value = "notresolved:" + val;
							if (!_context.HasDictionary(code)){
								_context.RegisterError(BSharpErrors.NotResolvedDictionary(_cls.FullName, a.Parent,
								                                                          code));
							}
							else{
								_context.RegisterError(BSharpErrors.NotResolvedDictionaryElement(_cls.FullName,
								                                                                 a.Parent, val));
							}
						}
					}
					else{
						if (valueonly){
							a.Value = result.Split('|')[0];
						}
						else{
							a.Value = result;
						}
					}
				}
			}
		}

		private void PerformCompilation(){
			if (CheckExistedBuild()) return;
			_cls.Set(BSharpClassAttributes.InBuild);
			InternalBuild();
			_cls.Remove(BSharpClassAttributes.InBuild);
			_cls.Set(BSharpClassAttributes.Built);
		}

		private void GetInheritedSourceIndex(){
			_cls.Compiled = new XElement(_cls.Source);
			_cls.Compiled.Elements(BSharpSyntax.ClassImportDefinition).Remove();
			_cls.Compiled.Elements(BSharpSyntax.ClassElementDefinition).Remove();
			_cls.Compiled.Elements(BSharpSyntax.ClassEvaluateDefinition).Remove();
			_cls.ParamSourceIndex = BuildParametersConfig();
		}


		private void InternalBuild(){
         //   Console.WriteLine("enter "+_cls.Name);
			GetInheritedSourceIndex();
         //   Console.WriteLine("GetInheritedSourceIndex " + _cls.Name);
			SupplyClassAttributesForEvaluations();
          //  Console.WriteLine("SupplyClassAttributesForEvaluations " + _cls.Name);
			InitializeBuildIndexes();
            // Console.WriteLine("InitializeBuildIndexes " + _cls.Name);
            MergeClassLevelUpSets();
            DownFallEmbedAttribute();
          //  Console.WriteLine("DownFallEmbedAttribute " + _cls.Name);
			IntializeMergeIndexes();
		    
			InterpolateFields();
			BindParametersToCompiledClass();
		    RemoveClassLevelUpSets();
			MergeInternals();
			SupplyEvaluationsForElements();
			InterpolateElements(codeonly: true);
			PerformMergingWithElements();
		    ProcessInElementUpSets();
			InterpolateElements();
			CleanupElementsWithConditions();
			ExecutePreSimpleIncludeExtensions();
			ProcessSimpleIncludes();
			CleanupPrivateMembers();
			CheckoutRequireLinkingRequirements();
		}

	    private void ProcessInElementUpSets() {
	        var elements = _cls.Compiled.Descendants(BSharpSyntax.UpSetBlock).ToArray();
	        if (0 == elements.Length) return;
            var scope = new Scope(_cls.ParamIndex);
            var le= new LogicalExpressionEvaluator();
	        foreach (var element in elements) {
	            var condition = element.Attribute(BSharpSyntax.ConditionalAttribute);
	            if (null != condition) {
	                if(!le.Eval(condition.Value,scope))continue;
	            }
                ApplyUpset(element,element.Parent);
	        }
            elements.Remove();
	    }

	    private void RemoveClassLevelUpSets() {
	        _cls.Compiled.Elements(BSharpSyntax.UpSetBlock).ToArray().Remove();
	    }

	    private void MergeClassLevelUpSets() {
	        var upsets = GetAllUpSets().ToArray();
	        if (0 != upsets.Length) {
                var scope = new Scope(_cls.ParamIndex);
	            if (null != _compiler) {
	                scope.AddParent(_compiler.Global);
	            }
                var le = new LogicalExpressionEvaluator();
	            foreach (var element in GetAllUpSets()) {
	                var condition = element.Attribute(BSharpSyntax.ConditionalAttribute);
	                if (null != condition) {
	                    if(!le.Eval(condition.Value,scope))continue;
	                    
	                }
                    ApplyUpset(element);
                }
               
	        }
	    }

	    private void ApplyUpset(XElement e) {
	        foreach (var attribute in e.Attributes()) {
                var n = attribute.Name.LocalName;
                if (n == "_file" || n == "_line" || n == "if") continue;
	            _cls.ParamIndex[n] = attribute.Value;
	            _cls.ParamSourceIndex[n] = attribute.Value;
                _cls.Source.SetAttributeValue(n,attribute.Value);
	            if (null != _cls.Compiled) {
	                _cls.Compiled.SetAttributeValue(n,attribute.Value);
	            }
	        }
	    }

        private void ApplyUpset(XElement e, XElement target)
        {
            foreach (var attribute in e.Attributes())
            {
                var n = attribute.Name.LocalName;
                if (n == "_file" || n == "_line" || n == "if") continue;
               target.SetAttributeValue(n, attribute.Value);
            }
        }

        private IEnumerable<XElement> GetAllUpSets() {
            foreach (var cls in _cls.AllImports)
            {
                foreach (var upset in cls.Source.Elements(BSharpSyntax.UpSetBlock)) {
                    yield return upset;
                }
            }
            foreach (var upset in _cls.Source.Elements(BSharpSyntax.UpSetBlock))
            {
                yield return upset;
            }
        } 


	    private void SupplyClassAttributesForEvaluations(){
			foreach (BSharpEvaluation d in _cls.AllEvaluations){
				if (d.ResultType == "attribute" && d.Target == "class"){
					if (null == _cls.Compiled.Attribute(d.Code) && !_cls.ParamSourceIndex.ContainsKey(d.Code)){
						_cls.Compiled.SetAttributeValue(d.Code, d.Value);
					}
				}
			}
		}

		private void SupplyEvaluationsForElements(){
			foreach (BSharpEvaluation d in _cls.AllEvaluations){
				if (d.ResultType == "attribute" && d.Target == "elements"){
					foreach (XElement element in _cls.Compiled.Elements()){
						if (null == element.Attribute(d.Code)){
							element.SetAttributeValue(d.Code, d.Value);
						}
					}
				}
			}
		}

		/// <summary>
		///     we require implicitly descent embed state
		/// </summary>
		private void DownFallEmbedAttribute(){
			if (_cls.Is(BSharpClassAttributes.Embed)) return;
			if (_cls.AllImports.Any(_ => _.Is(BSharpClassAttributes.Embed))){
				_cls.Set(BSharpClassAttributes.Embed);
			}
		}

		private void ExecutePreSimpleIncludeExtensions(){
			_compiler.CallExtensions(_cls, _context, BSharpCompilePhase.PreSimpleInclude);
		}

		private void CheckoutRequireLinkingRequirements(){
			if (
				_cls.Compiled.DescendantsAndSelf()
				    .Any(_ => _.Value.Contains("%{") || _.Attributes().Any(__ => __.Value.Contains("%{")))){
				_cls.Set(BSharpClassAttributes.RequireLateInterpolation);
			}
			bool ispureEmbed = _cls.Is(BSharpClassAttributes.Embed) && !_cls.Is(BSharpClassAttributes.Patch);
			if (ispureEmbed) return;
			IEnumerable<XAttribute> attrs = _cls.Compiled.DescendantsAndSelf().SelectMany(_ => _.Attributes());

			foreach (XAttribute a in attrs){
				string val = a.Value;
				if (string.IsNullOrWhiteSpace(val) || val.Length < 2) continue;
				if (val[0] == BSharpSyntax.ClassReferencePrefix){
					_cls.Set(BSharpClassAttributes.RequireClassResolution);
				}
				else if (val[0] == BSharpSyntax.DictionaryReferencePrefix){
					_cls.Set(BSharpClassAttributes.RequireDictionaryResolution);
				}
			}
			if (_cls.Compiled.Elements(BSharpSyntax.ClassExportDefinition).Any()){
				_cls.Set(BSharpClassAttributes.RequireDictionaryRegistration);
			}
			if (_cls.Compiled.Descendants(BSharpSyntax.IncludeBlock).Any()){
				_cls.Set(BSharpClassAttributes.RequireAdvancedIncludes);
			}
		}


		private void ResolveClassReferences(){
			if (!_cls.Is(BSharpClassAttributes.RequireClassResolution)) return;
			//найдем все атрибуты, начинающиеся на ^
			foreach (XAttribute a in _cls.Compiled.DescendantsAndSelf().SelectMany(_ => _.Attributes())){
				if (a.Value[0] == '^'){
					string clsname = a.Value.Substring(1);
					string xpath = "";
					if (clsname.Contains("(")){
						int xpstart = clsname.IndexOf('(');
						xpath = clsname.Substring(xpstart);
						clsname = clsname.Substring(0, xpstart);
					}
					bool isarray = clsname.EndsWith("*");
					if (isarray){
						clsname = clsname.Substring(0, clsname.Length - 1);
					}
					IBSharpClass normallyresolvedClass = _context.Get(clsname, ns: _cls.Namespace);
					if (null != normallyresolvedClass){
						if (normallyresolvedClass.Is(BSharpClassAttributes.Abstract)){
							a.Value = "ABSTRACT::" + normallyresolvedClass.FullName;
							_context.RegisterError(BSharpErrors.AbstractClassReference(_cls, a.Parent, normallyresolvedClass.FullName));
							continue;
						}
						a.Value = normallyresolvedClass.FullName;
						if (isarray){
							a.Value += "*";
						}
						if (!string.IsNullOrWhiteSpace(xpath)){
							object xpathresult = normallyresolvedClass.Compiled.XPathEvaluate(xpath);
							if (xpathresult.GetType().IsValueType){
								a.Value = xpathresult.ToStr();
							}
							else{
								IEnumerable<object> ie = (xpathresult as IEnumerable).OfType<object>();
								var sb = new StringBuilder();
								foreach (object xNode in ie){
									var attr = xNode as XAttribute;
									if (null != attr){
										sb.Append(attr.Value);
									}
									var e = xNode as XElement;
									if (null != e){
										sb.Append(e.Value);
									}
									var t = xNode as XText;
									if (null != t){
										sb.Append(t.Value);
									}
								}

								a.Value = sb.ToString();
							}
						}
						var __cls = _cls as BSharpClass;
						if (null != __cls && !__cls.ReferencedClasses.Contains(normallyresolvedClass)){
							__cls.ReferencedClasses.Add(normallyresolvedClass);
						}
					}
					else{
						IBSharpClass[] candidates =
							_context.Get(BSharpContextDataType.Working).Where(_ => _.FullName.EndsWith(clsname)).ToArray();
						if (!candidates.Any()){
							a.Value = "NOTRESOLVED::" + clsname;
							_context.RegisterError(BSharpErrors.NotResolvedClassReference(_cls, a.Parent, clsname));
						}
						else if (1 == candidates.Count()){
							a.Value = candidates[0].FullName;
							string start = candidates[0].FullName.Replace(clsname, "");
							if (!_cls.FullName.StartsWith(start)){
								_context.RegisterError(BSharpErrors.NotDirectClassReference(_cls, a.Parent, clsname));
							}
						}
						else{
							a.Value = "AMBIGOUS::" + clsname;
							_context.RegisterError(BSharpErrors.AmbigousClassReference(_cls, a.Parent, clsname));
						}
					}
				}
			}
		}

		private void
			ProcessSimpleIncludes(){
			XElement[] includes;
			bool needReInterpolate = false;
			while (
				(includes =
				 _cls.Compiled.Descendants(BSharpSyntax.IncludeBlock)
				     .Where(
					     _ => _.GetCode() != BSharpSyntax.IncludeAllModifier && null == _.Attribute(BSharpSyntax.NoProcessDirective))
				     .ToArray()).Length != 0){
				foreach (XElement i in includes){
					needReInterpolate = ProcessSimpleInclude(i, needReInterpolate);
				}
			}
			if (needReInterpolate){
				InterpolateElements(BSharpSyntax.IncludeInterpolationAncor);
			}
		}

		private bool ProcessSimpleInclude(XElement i, bool needReInterpolate){
			string code = i.GetCode();
			if (string.IsNullOrWhiteSpace(code)){
				_context.RegisterError(BSharpErrors.FakeInclude(_cls, i));
				i.Remove();
				return needReInterpolate;
			}

			IBSharpClass includecls = _context.Get(code, ns: _cls.Namespace);
			if (null == includecls){
				_context.RegisterError(BSharpErrors.NotResolvedInclude(_cls, i));
				i.Remove();
			}
			else if (includecls.IsOrphaned){
				_context.RegisterError(BSharpErrors.OrphanInclude(_cls, i));
				i.Remove();
			}
			else{
				Build(BuildPhase.Compile, _compiler, includecls, _context);
				var __cls = _cls as BSharpClass;
				if (null != __cls && !__cls.IncludedClasses.Contains(includecls)){
					__cls.IncludedClasses.Add(includecls);
				}

				var includeelement = new XElement(includecls.Compiled);

				bool usebody = null != i.Attribute(BSharpSyntax.IncludeBodyModifier) ||
				               i.GetName() == BSharpSyntax.IncludeBodyModifier;
				bool nochild = null != i.Attribute(BSharpSyntax.IncludeNoChildModifier) ||
				               i.GetName() == BSharpSyntax.IncludeNoChildModifier;
				needReInterpolate = needReInterpolate ||
				                    includeelement.HasAttributes(contains: BSharpSyntax.IncludeInterpolationAncor + "{",
				                                                 skipself: usebody);

				if (usebody){
					XElement[] elements = ExtractIncludeBody(i, includeelement, nochild);
					i.ReplaceWith(elements);
				}
				else{
					ExctractIncludeClass(i, nochild, includeelement, includecls);
					string ename = i.Attr(BSharpSyntax.IncludeElementNameModifier);
					if (!string.IsNullOrWhiteSpace(ename)){
						includeelement.Name = ename;
						if (i.Attr(BSharpSyntax.IncludeKeepCodeModifier).ToBool()){
							includeelement.SetAttributeValue("code", includecls.Name);
						}
					}

					if (IsIncludeClassMatch(i, includeelement)){
						i.ReplaceWith(includeelement);
					}
					else{
						i.Remove();
					}
				}
			}
			return needReInterpolate;
		}

		private bool IsIncludeClassMatch(XElement include, XElement includeclass){
			IEnumerable<XElement> wheres = include.Elements(BSharpSyntax.IncludeWhereClause);
			if (wheres.Any()){
				var matcher = new XmlTemplateMatcher(wheres);
				return matcher.IsMatch(includeclass);
			}
			return true;
		}

		private void ExctractIncludeClass(XElement i, bool nochild, XElement includeelement, IBSharpClass cls){
			foreach (BSharpEvaluation eval in cls.AllEvaluations){
				_cls.AllEvaluations.Add(eval.Bind(includeelement));
			}
			if (nochild){
				includeelement.Elements().Remove();
			}
			foreach (
				XAttribute source in
					i.Attributes().Where(_ => !string.IsNullOrWhiteSpace(_.Value) && _.Value[0] == BSharpSyntax.ClassReferencePrefix)){
				source.Value = _context.Get(source.Value.Substring(1)).FullName;
			}
			StoreIncludeParameters(i, includeelement);
			includeelement.Name = includeelement.Attr(BSharpSyntax.ClassFullNameAttribute);
			includeelement.Attribute(BSharpSyntax.ClassFullNameAttribute).Remove();
			XAttribute keepcode = includeelement.Attribute("keepcode");
			if (null != keepcode){
				keepcode.Remove();
				includeelement.SetAttributeValue("fullcode", cls.FullName);
			}
			else{
				includeelement.Attribute(BSharpSyntax.ClassNameAttribute).Remove();
			}

			XAttribute a = includeelement.Attribute("id");

			if (null != a) a.Remove();
			XAttribute sc = includeelement.Attribute("set-code");
			if (null != sc){
				includeelement.SetAttributeValue("code", sc.Value);
				sc.Remove();
			}
			XAttribute sen = includeelement.Attribute("set-elname");
			if (null != sen){
				includeelement.Name = sen.Value;
				sen.Remove();
			}
			else{
				if (null != i.Attribute("element")){
					includeelement.Name = i.Attribute("element").Value;
				}
			}

			if (cls.Is(BSharpClassAttributes.RequireLateInterpolation)){
				_lateincluder.Interpolate(includeelement, i);
			}
		}

		private XElement[] ExtractIncludeBody(XElement i, XElement includeelement, bool nochild){
			IEnumerable<XElement> elements = includeelement.Elements();
			IEnumerable<XElement> wheres = i.Elements(BSharpSyntax.IncludeWhereClause);
			if (wheres.Any()){
				var matcher = new XmlTemplateMatcher(wheres);
				elements = elements.Where(matcher.IsMatch).ToArray();
			}
			if (!elements.Any()){
				_context.RegisterError(BSharpErrors.EmptyInclude(_cls, i));
			}

			StoreParentParameters(includeelement, i);
			foreach (XElement e in elements){
				if (nochild){
					e.Elements().Remove();
				}
				StoreIncludeParameters(i, e);
			}
			XElement orderby = i.Element(BSharpSyntax.IncludeOrderByClause);
			if (null != orderby){
				elements = elements.OrderBy(_ => _.Attr(orderby.Attr("code"))).ToArray();
			}
			XElement select = i.Element(BSharpSyntax.IncludeSelectClause);
			XElement grp = i.Element(BSharpSyntax.IncludeGroupByClause);
			if (null != select && null != grp){
				if (null == select.Attribute(grp.Attr("code"))){
					select.SetAttributeValue(grp.Attr("code"), "false");
				}
			}
			if (null != grp){
				string ename = grp.Attr("as", "group");
				string gattr = grp.Attr("with", "code");
				IGrouping<string, XElement>[] gelements = elements.GroupBy(_ => _.Attr(grp.Attr("code"))).ToArray();
				var groups = new List<XElement>();
				foreach (var e in gelements){
					var grpe = new XElement(ename, new XAttribute(gattr, e.Key));
					XElement[] els = e.ToArray();
					if (null != select){
						foreach (XElement el in els){
							FilterWithSelect(el, select);
						}
					}
					grpe.Add(els);
					groups.Add(grpe);
				}
				return groups.ToArray();
			}
			else{
				if (null != select){
					foreach (XElement el in elements){
						FilterWithSelect(el, select);
					}
				}
				return elements.ToArray();
			}
		}

		private void FilterWithSelect(XElement e, XElement select){
			foreach (XAttribute a in e.Attributes().ToArray()){
				if (a.Name.LocalName == "_file") continue;
				if (a.Name.LocalName == "_line") continue;
				XAttribute sa = select.Attribute(a.Name.LocalName);
				if (null == sa || "false" == sa.Value || "0" == sa.Value){
					a.Remove();
					continue;
				}
				if (sa.Value == "true" || sa.Value == "1"){
					continue;
				}
				e.SetAttributeValue(sa.Value, a.Value);
				a.Remove();
			}
		}

		private void StoreIncludeParameters(XElement src, XElement trg){
			foreach (XAttribute a in src.Attributes()){
				if (a.Name.LocalName == "code") continue;
				if (a.Name.LocalName == "name") continue;
				if (a.Name.LocalName == BSharpSyntax.IncludeBodyModifier) continue;
				if (a.Name.LocalName == BSharpSyntax.IncludeNoChildModifier) continue;
				if (a.Name.LocalName == BSharpSyntax.IncludeElementNameModifier) continue;
				if (a.Name.LocalName.StartsWith("__TILD__")){
					trg.SetAttributeValue(a.Name.LocalName.Substring("__TILD__".Length), a.Value);
				}
				else if (a.Name.LocalName.StartsWith("__PLUS__")){
					string localName = a.Name.LocalName.Substring("__PLUS__".Length);
					if (trg.Attribute(localName) == null){
						trg.SetAttributeValue(localName, a.Value);
					}
				}
				else{
					if (null == trg.Attribute(a.Name.LocalName)){
						trg.SetAttributeValue(a.Name, a.Value);
					}
				}
			}
		}

		private void StoreParentParameters(XElement src, XElement trg){
			foreach (XAttribute a in src.Attributes()){
				if (trg.AncestorsAndSelf().All(_ => null == _.Attribute(a.Name))){
					trg.SetAttributeValue(a.Name, a.Value);
				}
			}
		}

		private void CleanupElementsWithConditions(){
			var elements = _cls.Compiled.Descendants().ToArray();
			elements = elements.Where(_ => !IsMatch(_)).ToArray();
			elements.Remove();
		}

		private void PerformMergingWithElements(){
			foreach (IBSharpElement root in _cls.AllElements.Where(_ => _.Type == BSharpElementType.Define).ToArray()){
				try{
					XElement[] allroots = _cls.Compiled.Descendants(root.Name).ToArray();
					IEnumerable<IGrouping<string, XElement>> groupedroots = allroots.GroupBy(_ => _.GetCode());
					foreach (var doublers in groupedroots.Where(_ => _.Count() > 1)){
					    if (!string.IsNullOrWhiteSpace(doublers.First().Attr("code"))) {
					        doublers.Skip(1).Remove();
					    }
					}

					IBSharpElement[] alloverrides =
						_cls.AllElements.Where(_ => _.Type != BSharpElementType.Define && _.TargetName == root.Name).ToArray();
					//если нет целевых элементов, то не обрабатываем мержи
					if (!_cls.Compiled.Elements().Any(_ => alloverrides.Any(__ => __.Name == _.Name.LocalName))){
						continue;
					}
//				foreach (var over in alloverrides) {
					foreach (var g in groupedroots){
						XElement e = g.First();
						//реверсировать надо для правильного порядка обхода
						XElement[] candidates;
						if (e.Parent == _cls.Compiled){
							candidates = e.ElementsBeforeSelf().Reverse().Where(_ => _.GetCode() == g.Key).ToArray();
						}
						else{
							candidates = _cls.Compiled.Elements().Reverse().Where(_ => _.GetCode() == g.Key).ToArray();
						}
						foreach (XElement o in candidates){
							IBSharpElement over = alloverrides.FirstOrDefault(_ => _.Name == o.Name.LocalName);
							if (null != over){
								if (over.Type == BSharpElementType.Override){
									foreach (XAttribute a in o.Attributes()){
										e.SetAttributeValue(a.Name, a.Value);
									}
									if (o.HasElements){
										e.Elements().Remove();
										e.Add(o.Elements());
									}
									if (!string.IsNullOrWhiteSpace(o.Value) && !o.HasElements){
										e.Value = o.Value;
									}
								}
								else if (over.Type == BSharpElementType.Extension){
									foreach (XAttribute a in o.Attributes()){
										if (null == e.Attribute(a.Name)){
											e.SetAttributeValue(a.Name, a.Value);
										}
									}
									if (o.HasElements){
										e.Add(o.Elements());
									}
									if (!string.IsNullOrWhiteSpace(o.Value) && !o.HasElements){
										//join embeded code
										if (o.Value[0] == '(' && o.Value[o.Value.Length - 1] == ')' && e.Value[0] == '(' &&
										    e.Value[e.Value.Length - 1] == ')'){
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
				}
				catch (Exception e){
					throw e;
				}
			}
		}

		private bool CheckExistedLink(){
			if (_cls.Is(BSharpClassAttributes.Linked)) return true;
			if (_cls.Is(BSharpClassAttributes.InLink)){
				for (int i = 0; i <= 10; i++){
					Thread.Sleep(5);
					if (_cls.Is(BSharpClassAttributes.Linked)){
						break;
					}
				}
			}
			return _cls.Is(BSharpClassAttributes.Linked);
		}

		private bool CheckExistedBuild(){
			if (_cls.Is(BSharpClassAttributes.Built)) return true;
			if (_cls.Is(BSharpClassAttributes.InBuild)){
				for (int i = 0; i <= 10; i++){
					Thread.Sleep(5);
					if (_cls.Is(BSharpClassAttributes.Built)){
						break;
					}
				}
			}
			return _cls.Is(BSharpClassAttributes.Built);
		}

		private void IntializeMergeIndexes(){
			foreach (IBSharpElement m in _cls.AllElements){
				m.Name = m.Name.ConvertToXNameCompatibleString();
				if (!string.IsNullOrWhiteSpace(m.TargetName)){
					m.TargetName = m.TargetName.ConvertToXNameCompatibleString();
				}
			}
			string[] allroots = _cls.AllElements.Where(_ => _.Type == BSharpElementType.Define).Select(_ => _.Name).ToArray();
			foreach (string root in allroots){
				var defoverride = new BSharpElement{
					Name = BSharpSyntax.ElementOverridePrefix + root,
					TargetName = root,
					Type = BSharpElementType.Override
				};
				if (!_cls.AllElements.Contains(defoverride)){
					_cls.AllElements.Add(defoverride);
				}
				var defext = new BSharpElement{
					Name = BSharpSyntax.ElementExtensionPrefix + root,
					TargetName = root,
					Type = BSharpElementType.Extension
				};
				if (!_cls.AllElements.Contains(defext)){
					_cls.AllElements.Add(defext);
				}
			}
		}


		private void MergeInternals(){
//реверс нужен для правильного порядка наката элементов

			foreach (IBSharpClass e in _cls.AllImports.Reverse()){
				if (e.Is(BSharpClassAttributes.Static)){
					_cls.Compiled.Add(e.Compiled.Elements().Where(IsMatch));
				}
				else{
					_cls.Compiled.Add(e.Source.Elements().Where(IsMatch));
				}
			}
		}

		private bool IsMatch(XElement arg){
			//due to fact that generics just prepare template for override classes it doesn't exclude 
			//anything from elements due to conditions
			if (_cls.Is(BSharpClassAttributes.Generic)) return true;
			string cond = arg.Attr(BSharpSyntax.ConditionalAttribute);
			if (string.IsNullOrWhiteSpace(cond)){
				return true;
			}
			//мы должны пропускать интерполяции, так как сверить их все равно нельзя пока
			if (cond.Contains("${")) return true;
			IScope compilerOptions = _compiler.GetConditions();
			IScope srcp = _cls.ParamIndex;
			if (null != compilerOptions){
				compilerOptions.SetParent(srcp);
				srcp = compilerOptions;
			}
			var src = new DictionaryTermSource<object>(srcp);
			return new LogicalExpressionEvaluator().Eval(cond, src);
		}

		private void CleanupPrivateMembers(){
			string name = _cls.Compiled.GetName();
			if (name == BSharpSyntax.ClassAbstractModifier || name == BSharpSyntax.ClassStaticModifier){
				_cls.Compiled.Attribute("name").Remove();
			}
			_cls.Compiled.Attributes(BSharpSyntax.ClassAbstractModifier).Remove();
			_cls.Compiled.Attributes(BSharpSyntax.ClassStaticModifier).Remove();
			_cls.Compiled.Attributes(BSharpSyntax.ClassGenericModifier).Remove();
			_cls.Compiled.Elements(BSharpSyntax.ClassImportDefinition).Remove();
			_cls.Compiled.Elements(BSharpSyntax.ClassElementDefinition).Remove();
			_cls.Compiled.Elements(BSharpSyntax.ClassEvaluateDefinition).Remove();

			foreach (XElement e in _cls.Compiled.DescendantsAndSelf()){
				if (null != e.Attribute("id")){
					if (e.Attr("code") == e.Attr("id")){
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
				                     (_.Name.LocalName[0] == BSharpSyntax.PrivateAttributePrefix || string.IsNullOrEmpty(_.Value)) &&
				                     !(_.Name.LocalName.StartsWith("__AT__"))
				                     &&
				                     (!(_compiler.GetConfig().KeepLexInfo &&
				                        (_.Name.LocalName == "_file" || _.Name.LocalName == "_line" || _.Name.LocalName == "_dir")))
					).Remove();
			}

			foreach (string m in _cls.AllElements.Where(_ => _.Type != BSharpElementType.Define).Select(_ => _.Name).Distinct()){
				_cls.Compiled.Elements(m).Remove();
			}
		}

		private void InitializeBuildIndexes(){
			_cls.ParamIndex = new Scope();
			_cls.Compiled.SetAttributeValue(BSharpSyntax.ClassFullNameAttribute, _cls.FullName);
			foreach (var p in _cls.ParamSourceIndex){
				_cls.ParamIndex.Set(p.Key, p.Value);
			}
		}


		/// <summary>
		///     Возвращает XML для резолюции атрибутов
		/// </summary>
		/// <returns></returns>
		private IScope BuildParametersConfig(){
			var result = new Scope();
			Scope current = result;
			foreach (IBSharpClass i in _cls.AllImports.Union(new[]{_cls})){
				var selfconfig = new Scope();
				selfconfig.Set("_class_", _cls.FullName);
				selfconfig.SetParent(current);
				current = selfconfig;
				if (i.Is(BSharpClassAttributes.Static) && _cls != i){
                    while (!i.Is(BSharpClassAttributes.Built))
                    {
				        Thread.Sleep(5);
				    }
					foreach (var p in i.ParamIndex){
						current.Set(p.Key, p.Value);
					}
				}
				else{
					foreach (XAttribute a in i.Source.Attributes()){
                        current.Set("_class_",i.Name);
						current.Set(a.Name.LocalName, a.Value);
					}
				}
			}
			current.Stornate();
			return current;
		}

		private void BindParametersToCompiledClass(){
			foreach (var e in _cls.ParamIndex){
				_cls.Compiled.SetAttributeValue(e.Key, e.Value.ToStr());
			}
		}

	    class seq {
	        public int Current;
	        public int Start;
	        public int Step;

	        public int Next() {
	            var result = Current;
	            Current = Current + Step;
	            return result;

	        }
	    }
		private void InterpolateElements(char ancor = '\0', bool codeonly = false){
			if (ancor == '\0'){
				ancor = _cls.Is(BSharpClassAttributes.Generic) ? '`' : '$';
			}

			if (GetConfig().UseInterpolation){
				var xi = new XmlInterpolation{
					AncorSymbol = ancor,
					SecondSource = _compiler.Global,
					CodeOnly = codeonly,
					Level = codeonly ? 1 : 0
				};
			    var ctx = GetInterpolationContext();
                ctx.Set("self",new Scope(_cls.Compiled));
                var basescope = new Scope((object[])_cls.ParamSourceIndex.GetParents().ToArray());
                basescope.Stornate();
			    var p = _cls.ParamSourceIndex.GetParent();
			    while (null != p) {
                    basescope[p["_class_"].ToStr()] = p;
			        p = p.GetParent();
			    }
                ctx.Set("base", basescope);
			    xi.Interpolate(_cls.Compiled,ctx);
			}
		}

	    

	    private IScope GetInterpolationContext() {
	        if (_cls.InterpolationContext != null) {
	            return _cls.InterpolationContext;
	        }
	        IDictionary<string, seq> _sequences = new ConcurrentDictionary<string, seq>();
	        var initseq = (Func<string, int, int, string>) ((n, start, step) => {
	            if (0 == step) {
	                step = 1;
	            }
	            _sequences[n] = new seq {Current = start, Start = start, Step = step};
	            return n;
	        });
	        var advctx = new {
	            mycls = _cls,
	            initseq = initseq,
	            nextseq = (Func<string, int>) (n => {
	                if (string.IsNullOrWhiteSpace(n)) {
	                    n = "default";
	                }
	                if (!_sequences.ContainsKey(n)) {
	                    initseq(n, 0, 1);
	                }
	                return _sequences[n].Next();
	            })
	        };
	        var result = new Scope(advctx);
	        result.SetParent(_compiler.Global);

	        _cls.InterpolationContext = result;

	        return _cls.InterpolationContext;
	    }


	    private void InterpolateFields(){
			// у генериков на этой фазе еще производится полная донастройка элементов по анкору ^
            
			if (GetConfig().UseInterpolation){
				var si = new StringInterpolation{AncorSymbol = _cls.Is(BSharpClassAttributes.Generic) ? '`' : '$'};
			    var global = GetInterpolationContext();

				bool requireInterpolateNames = _cls.ParamIndex.Keys.Any(_ => _.Contains("__LBLOCK__"));
				while (true){
					bool changed = false;
					foreach (var v in _cls.ParamIndex.OrderBy(_ => {
					    if (_.Value.ToStr().Contains("${") && _.Value.ToStr().Contains("(")) {
					        return 1000;
					    }
					    return 0;
					}).ToArray()){
						string key = v.Key;
						if (requireInterpolateNames) {
						    var scope = new Scope(_cls.ParamSourceIndex);
						    scope["self"] = _cls.ParamSourceIndex;
							string esckey = key.Unescape(EscapingType.XmlName);
							if (-1 != esckey.IndexOf('{')){
								string _key = si.Interpolate(esckey, scope).Escape(EscapingType.XmlName);
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
						if (-1 == s.IndexOf('{') || -1 == s.IndexOf(si.AncorSymbol)) continue;
					    var src = s.Contains("(") ? _cls.ParamIndex : _cls.ParamSourceIndex;
					    src.Set("self",src);
					    var basescope = new Scope((object[])src.GetParents().ToArray());
                        basescope.Stornate();
                        var p = _cls.ParamSourceIndex.GetParent();
                        while (null != p)
                        {
                            basescope[p["_class_"].ToStr()] = p;
                            p = p.GetParent();
                        }
                        src.Set("base",basescope);
						string newval = si.Interpolate(s,src, global, key);
						if (newval != s){
							changed = true;
							_cls.ParamIndex.Set(key, newval);
						}
					}
					if (!changed) break;
				}
			}
           
	    }
	}
}