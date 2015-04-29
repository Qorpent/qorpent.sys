using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp{
	/// <summary>
	/// </summary>
	[Serialize]
	public class BSharpClass : IBSharpClass{
		/// <summary>
		/// Контекст класса
		/// </summary>
		public IBSharpContext Context
		{
			get{
				return
					_context;
			}
		}

	    public IScope InterpolationContext { get; set; }

	    private static int EXTCOUNTER = 1;
		private readonly IBSharpContext _context;
		private List<IBSharpElement> _allelements;

		/// <summary>
		///     Атрибуты класса
		/// </summary>
		private BSharpClassAttributes _attributes;

		private List<BSharpEvaluation> _cachedDefs;
		private IBSharpClass[] _cachedImports;
		private Dictionary<string, IBSharpElement> _elementRegistry;

		private IList<IBSharpClass> _includedClasses;
		private IList<IBSharpClass> _lateIncludedClasses;
		private BSharpPatchCreateBehavior _patchBehavior = BSharpPatchCreateBehavior.None;
		private BSharpPatchNameBehavior _patchNameBehavior = BSharpPatchNameBehavior.None;
		private bool? _patchPlain;
		private string _patchTarget;
		private IList<IBSharpClass> _referencedClasses;
		private IList<string> _referencedDictionaries;
		private IList<BSharpEvaluation> _selfdefs;

		/// <summary>
		/// </summary>
		public BSharpClass(IBSharpContext context){
			SelfElements = new List<IBSharpElement>();
			SelfImports = new List<IBSharpImport>();
			_context = context;
		}

		/// <summary>
		///     Сериализуемая версия атрибутов
		/// </summary>
		[SerializeNotNullOnly]
		public BSharpClassAttributes Attributes{
			get { return _attributes; }
		}

		/// <summary>
		/// </summary>
		[IgnoreSerialize]
		public IList<IBSharpClass> IncludedClasses{
			get { return _includedClasses ?? (_includedClasses = new List<IBSharpClass>()); }
		}

		/// <summary>
		/// </summary>
		[IgnoreSerialize]
		public IList<IBSharpClass> LateIncludedClasses{
			get { return _lateIncludedClasses ?? (_lateIncludedClasses = new List<IBSharpClass>()); }
		}

		/// <summary>
		/// </summary>
		[IgnoreSerialize]
		public IList<IBSharpClass> ReferencedClasses{
			get { return _referencedClasses ?? (_referencedClasses = new List<IBSharpClass>()); }
		}

		/// <summary>
		/// </summary>
		[IgnoreSerialize]
		public IList<string> ReferencedDictionaries{
			get { return _referencedDictionaries ?? (_referencedDictionaries = new List<string>()); }
		}

		/// <summary>
		/// </summary>
		public BSharpClassBuilder Builder { get; set; }

		/// <summary>
		/// </summary>
		[SerializeNotNullOnly]
		public string Name { get; set; }

		/// <summary>
		///     Возвращает селектор классов для
		/// </summary>
		public string PatchTarget{
			get{
				if (!Is(BSharpClassAttributes.Patch)) return null;
				if (null == _patchTarget){
					_patchTarget = Source.Attr(BSharpSyntax.PatchTargetAttribute);
				}
				return _patchTarget;
			}
		}

		/// <summary>
		///     Возвращает селектор классов для
		/// </summary>
		public bool PatchPlain{
			get{
				if (!Is(BSharpClassAttributes.Patch)) return false;
				if (null == _patchPlain){
					_patchPlain = Source.Attr(BSharpSyntax.PatchPlainAttribute).ToBool();
				}
				return _patchPlain.Value;
			}
		}

		/// <summary>
		/// </summary>
		public BSharpPatchCreateBehavior PatchCreateBehavior{
			get{
				if (!Is(BSharpClassAttributes.Patch)) return BSharpPatchCreateBehavior.None;
				if (BSharpPatchCreateBehavior.None == _patchBehavior){
					string _val = Source.Attr(BSharpSyntax.PatchCreateBehavior);
					if (string.IsNullOrWhiteSpace(_val)){
						_patchBehavior = BSharpPatchCreateBehavior.Default;
					}
					else if (_val == BSharpSyntax.PatchCreateBehaviorNone){
						_patchBehavior = BSharpPatchCreateBehavior.NoneOnNew;
					}
					else if (_val == BSharpSyntax.PatchCreateBehaviorCreate){
						_patchBehavior = BSharpPatchCreateBehavior.CreateOnNew;
					}
					else if (_val == BSharpSyntax.PatchCreateBehaviorError){
						_patchBehavior = BSharpPatchCreateBehavior.ErrorOnNew;
					}
					else{
						_patchBehavior = BSharpPatchCreateBehavior.Invalid;
					}
				}
				return _patchBehavior;
			}
		}

		/// <summary>
		/// </summary>
		public BSharpPatchPhase PatchPhase{
			get{
				var result = BSharpPatchPhase.After;
				if (Source.GetSmartValue(BSharpSyntax.PatchBeforeAttribute).ToBool()){
					result = BSharpPatchPhase.Before;
				}
				else if (Source.GetSmartValue(BSharpSyntax.PatchAfterBuildAttribute).ToBool()){
					result = BSharpPatchPhase.AfterBuild;
				}
				return result;
			}
		}

		/// <summary>
		/// </summary>
		public BSharpPatchNameBehavior PatchNameBehavior{
			get{
				if (!Is(BSharpClassAttributes.Patch)) return BSharpPatchNameBehavior.None;
				if (BSharpPatchNameBehavior.None == _patchNameBehavior){
					string _val = Source.Attr(BSharpSyntax.PatchNameBehavior);
					if (string.IsNullOrWhiteSpace(_val)){
						_patchNameBehavior = BSharpPatchNameBehavior.Default;
					}
					else if (_val == BSharpSyntax.PatchNameBehaviorFree){
						_patchNameBehavior = BSharpPatchNameBehavior.Free;
					}
					else if (_val == BSharpSyntax.PatchNameBehaviorMatch){
						_patchNameBehavior = BSharpPatchNameBehavior.Match;
					}
				}
				return _patchNameBehavior;
			}
		}

		/// <summary>
		///     Возвращает true при наличии флага
		/// </summary>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public bool Is(BSharpClassAttributes attribute){
			return 0 != (_attributes & attribute);
		}

		/// <summary>
		///     Возвращает полный комплект атрибутов
		/// </summary>
		/// <returns></returns>
		public BSharpClassAttributes GetAttributes(){
			return _attributes;
		}

		/// <summary>
		///     Устанавливает определенные флаги
		/// </summary>
		/// <param name="flags"></param>
		public void Set(BSharpClassAttributes flags){
			_attributes = _attributes | flags;
			if (
				flags == BSharpClassAttributes.RequireClassResolution
				||
				flags == BSharpClassAttributes.RequireDictionaryResolution
				||
				flags == BSharpClassAttributes.RequireAdvancedIncludes
				){
				_attributes = _attributes | BSharpClassAttributes.RequireLinking;
			}
			else if (
				flags == BSharpClassAttributes.Override
				||
				flags == BSharpClassAttributes.Extension
				){
				if (null == TargetClassName){
					TargetClassName = Name;
					string name = flags.ToString() + "_" + Name + "_" + Source.Attr("name") + "_" + EXTCOUNTER++;
					Name = name;
				}
				_attributes = _attributes | BSharpClassAttributes.Explicit;
				Remove(BSharpClassAttributes.Orphan);
			}
			else if (flags == BSharpClassAttributes.Explicit){
				Remove(BSharpClassAttributes.Orphan);
			}
		}

		/// <summary>
		///     Снимает определенные флаги
		/// </summary>
		/// <param name="flags"></param>
		public void Remove(BSharpClassAttributes flags){
			_attributes = _attributes & ~flags;
		}

		/// <summary>
		///     Упрощенный доступ компилированному контенту
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public string this[string code]{
			get { return this.Compiled.ResolveValue(code); }
		}


		/// <summary>
		///     Расширение имени, пакет, используется при конфликтующем разрешении имен
		///     по умолчанию если классы указаны в Namespace резолюция ведется только в рамках
		///     этого namespace, если без namespace, то глобально (RootNs)
		/// </summary>
		[SerializeNotNullOnly]
		public string Namespace { get; set; }

		/// <summary>
		///     Полное имя
		/// </summary>
		[IgnoreSerialize]
		public string FullName{
			get{
				if (string.IsNullOrWhiteSpace(Namespace)) return Name;
				return Namespace + "." + Name;
			}
		}

		/// <summary>
		///     Прототип класса
		/// </summary>
		[Serialize]
		public string Prototype{
			get { return Compiled.Attr(BSharpSyntax.ClassPrototypeAttribute); }
		}

		/// <summary>
		///     Прототип класса
		/// </summary>
		[Serialize]
		public int Priority{
			get{
				int p = Compiled.Attr(BSharpSyntax.PriorityAttribute).ToInt();
				return p;
			}
		}

		/// <summary>
		/// </summary>
		[Serialize]
		public string AliasImportCode { get; set; }


		/// <summary>
		///     Код первичного класса импорта
		/// </summary>
		[IgnoreSerialize]
		public IBSharpClass DefaultImport { get; set; }

		/// <summary>
		///     Код первичного класса импорта
		/// </summary>
		[SerializeNotNullOnly]
		public string DefaultImportCode { get; set; }

		/// <summary>
		///     Явные импорты
		/// </summary>
		[IgnoreSerialize]
		public IList<IBSharpImport> SelfImports { get; private set; }

		/// <summary>
		/// </summary>
		public IList<BSharpEvaluation> SelfEvaluations{
			get { return _selfdefs ?? (_selfdefs = new List<BSharpEvaluation>()); }
		}

		/// <summary>
		///     Определение сводимых элементов
		/// </summary>
		[IgnoreSerialize]
		public IList<IBSharpElement> SelfElements { get; private set; }

		/// <summary>
		/// </summary>
		[SerializeNotNullOnly]
		public XElement Source { get; set; }

		/// <summary>
		///     Компилированная версия класса
		/// </summary>
		[SerializeNotNullOnly]
		public XElement Compiled { get; set; }

		/// <summary>
		///     Элемент хранящий данные об индексе параметров
		/// </summary>
		[IgnoreSerialize]
		public IScope ParamSourceIndex { get; set; }

		/// <summary>
		///     Сведенный словарь параметров
		/// </summary>
		[IgnoreSerialize]
		public IScope ParamIndex { get; set; }

		/// <summary>
		///     Список всех определений мержа
		/// </summary>
		[IgnoreSerialize]
		public List<IBSharpElement> AllElements{
			get{
				lock (this)
				{
					if (null == _allelements){

						_allelements = GetAllElements().ToList();

					}
				}
				return _allelements;
			}
		}

		/// <summary>
		///     Текущая задача на построение
		/// </summary>
		[IgnoreSerialize]
		public Task BuildTask { get; set; }

		/// <summary>
		///     Ошибка компиляции
		/// </summary>
		[SerializeNotNullOnly]
		public Exception Error { get; set; }


		/// <summary>
		///     Для расширений - имя целевого класса
		/// </summary>
		[SerializeNotNullOnly]
		public string TargetClassName { get; set; }


		/// <summary>
		///     Возвращает полное перечисление импортируемых классов в порядке их накатывания
		/// </summary>
		/// <value></value>
		[IgnoreSerialize]
		public IEnumerable<IBSharpClass> AllImports{
			get{
				if (null != _cachedImports) return _cachedImports;
				lock (this){
					_cachedImports = GetAllImports(FullName, new Scope()).Distinct().ToArray();
					return _cachedImports;
				}
			}
		}

		/// <summary>
		///     Все определения в классе
		/// </summary>
		public IList<BSharpEvaluation> AllEvaluations{
			get{
				if (null == _cachedDefs){
					_cachedDefs = BuildAllEvaluations().ToList();
				}
				return _cachedDefs;
			}
		}

		/// <summary>
		///     Выполняет действия, необходимые для подготовки компиляции, выполняются в синхронном режиме
		/// </summary>
		public void PrepareForCompilation(){
			if (null == _cachedImports) _cachedImports = AllImports.ToArray();
		}

		/// <summary>
		///     Полная проверка статуса Orphan
		/// </summary>
		/// <value></value>
		[SerializeNotNullOnly]
		public bool IsOrphaned{
			get{
				if (Is(BSharpClassAttributes.Explicit)) return false;
				if (Is(BSharpClassAttributes.Orphan)) return true;
				if (null == DefaultImport) return true;
				return DefaultImport.IsOrphaned;
			}
		}

		/// <summary>
		///     Метод построения собственного индекса параметров
		/// </summary>
		/// <returns></returns>
		private IScope BuildSelfParametesSource(){
			var result = new Scope();
			foreach (XAttribute a in Source.Attributes()){
				result.Set(a.Name.LocalName, a.Value);
			}
			return result;
		}

		private IEnumerable<BSharpEvaluation> BuildAllEvaluations(){
			var vhash = new HashSet<string>();
			var hash = new HashSet<string>();
			vhash.Add(FullName);
			foreach (BSharpEvaluation evaluation in SelfEvaluations){
				if (hash.Contains(evaluation.Code)) continue;
				hash.Add(evaluation.Code);
				yield return evaluation;
			}
			foreach (IBSharpClass i in AllImports){
				if (vhash.Contains(i.FullName)) continue;
				foreach (BSharpEvaluation evaluation in i.SelfEvaluations){
					if (hash.Contains(evaluation.Code)) continue;
					hash.Add(evaluation.Code);
					yield return evaluation;
				}
			}
		}


		/// <summary>
		///     Собирает все определения мержей из класса
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IBSharpElement> GetAllElements(){
			lock (this){

				if (null == _elementRegistry){
					_elementRegistry = new Dictionary<string, IBSharpElement>();
					IBSharpClass[] imports = AllImports.ToArray();
					if (!Is(BSharpClassAttributes.Cycle)){
						foreach (IBSharpClass c in imports){
							if (!c.Is(BSharpClassAttributes.Cycle)){
								IBSharpElement[] elements = null;
								try{
									elements = c.AllElements.ToArray();
								}
								catch{
									Thread.Sleep(1);
									elements = c.AllElements.ToArray();
								}
								foreach (IBSharpElement edef in elements){
									if (null != edef){
										RegisterElement(edef);
									}
								}
							}
						}
					}
					foreach (IBSharpElement edef in SelfElements){
						if (null != edef){
							RegisterElement(edef);
						}
					}

					foreach (var registered in _elementRegistry.ToArray()){
						if (!_elementRegistry.ContainsKey(registered.Value.TargetName)){
							_elementRegistry[registered.Value.TargetName] = new BSharpElement{
								Implicit = true,
								Name = registered.Value.TargetName,
								TargetName = registered.Value.TargetName,
								Type = BSharpElementType.Define
							};
						}
					}
				}
				if (AllImports.Any(_ => _.Source.Attr(BSharpSyntax.ExplicitClassMarker) == BSharpSyntax.ExplicitClassPropagateValue)){
					foreach (var element in _elementRegistry.ToArray()){
						if (element.Value.Implicit){
							_elementRegistry.Remove(element.Key);
						}
					}
				}
				return _elementRegistry.Values;
			}
		}

		private void RegisterElement(IBSharpElement edef){
			string name = edef.Name;
			if (_elementRegistry.ContainsKey(name)){
				IBSharpElement existed = _elementRegistry[name];
				if (existed.Implicit || (!edef.Implicit)){
					_elementRegistry[name] = edef;
				}
			}
			else{
				_elementRegistry[name] = edef;
			}
		}


		private IEnumerable<IBSharpClass> GetAllImports(string root, IScope config){
			var dict = ((IDictionary<string, object>) config);
			var self = ((IDictionary<string, object>) BuildSelfParametesSource());
			foreach (var p in self.Where(_ => _.Value != null)){
				if (!dict.ContainsKey(p.Key) && !p.Value.ToStr().Contains("${")){
					dict[p.Key] = p.Value;
				}
			}

			if (null != DefaultImport){
				if (root != DefaultImport.FullName){
					foreach (IBSharpClass i in ((BSharpClass) DefaultImport).GetAllImports(root, config)){
						yield return i;
					}
					yield return DefaultImport;
				}
			}

			foreach (IBSharpImport i in SelfImports){
				if (null != i.Target
				    &&
				    !i.Target.IsOrphaned
				    && !i.Target.Is(BSharpClassAttributes.Ignored)
				    && i.Match(config)
					){
					if (root != i.Target.FullName){
						if (!i.Target.Is(BSharpClassAttributes.Static)){
							foreach (BSharpClass ic in ((BSharpClass) i.Target).GetAllImports(root, config)){
								yield return ic;
							}
						}
						yield return i.Target;
					}
					else{
						if (!Is(BSharpClassAttributes.Cycle)){
							_context.RegisterError(BSharpErrors.RecycleImport(this, root, i));
							Set(BSharpClassAttributes.Cycle);
						}
					}
				}
			}

			foreach (IBSharpImport i in SelfImports.Where(_ => null != _.Target && _.Target.IsOrphaned)){
				_context.RegisterError(BSharpErrors.OrphanImport(this, i));
			}
			foreach (
				IBSharpImport i in
					SelfImports.Where(_ => null != _.Target && _.Target.Is(BSharpClassAttributes.Ignored) && _.Match(config))){
				_context.RegisterError(BSharpErrors.IgnoredImport(this, i));
			}
		}
	}
}