using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp {
	/// <summary>
	/// </summary>
	[Serialize]
	public class BSharpClass : IBSharpClass {
		/// <summary>
		/// </summary>
		public BSharpClass(IBSharpContext context) {
			SelfElements = new List<IBSharpElement>();
			SelfImports = new List<IBSharpImport>();
			_context = context;
		}

		/// <summary>
		/// </summary>
		[SerializeNotNullOnly]
		public string Name { get; set; }
		/// <summary>
		/// Сериализуемая версия атрибутов
		/// </summary>
		[SerializeNotNullOnly]
		public BSharpClassAttributes Attributes {
			get { return _attributes; }
		}

		/// <summary>
		/// Атрибуты класса
		/// </summary>
		private BSharpClassAttributes _attributes;
		/// <summary>
		/// Возвращает true при наличии флага
		/// </summary>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public bool Is(BSharpClassAttributes attribute) {
			return _attributes.HasFlag(attribute);
		}
		/// <summary>
		/// Устанавливает определенные флаги
		/// </summary>
		/// <param name="flags"></param>
		public void Set(BSharpClassAttributes flags) {
			_attributes = _attributes | flags;
			if (
				flags==BSharpClassAttributes.RequireClassResolution
				||
				flags==BSharpClassAttributes.RequireDictionaryResolution
				||
				flags==BSharpClassAttributes.RequireAdvancedIncludes
				) {

				_attributes = _attributes | BSharpClassAttributes.RequireLinking;
			}
			else if (
				flags==BSharpClassAttributes.Override
				||
				flags==BSharpClassAttributes.Extension
				) {
				if (null == TargetClassName) {
					TargetClassName = Name;
					Name = Guid.NewGuid().ToString();
				}
				_attributes = _attributes | BSharpClassAttributes.Explicit;
				Remove(BSharpClassAttributes.Orphan);
			}
			else if (flags==BSharpClassAttributes.Explicit) {
				Remove(BSharpClassAttributes.Orphan);
			}
		}
		/// <summary>
		/// Снимает определенные флаги
		/// </summary>
		/// <param name="flags"></param>
		public void Remove(BSharpClassAttributes flags) {
			_attributes = _attributes & ~flags;
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
		public string FullName {
			get {
				if (string.IsNullOrWhiteSpace(Namespace)) return Name;
				return Namespace + "." + Name;
			}
		}

		

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
		public IConfig ParamSourceIndex { get; set; }

		/// <summary>
		///     Сведенный словарь параметров
		/// </summary>
		[IgnoreSerialize]
		public IConfig ParamIndex { get; set; }

		private List<IBSharpElement> _allelements = null;
		/// <summary>
		/// Список всех определений мержа
		/// </summary>
		[IgnoreSerialize]
		public List<IBSharpElement> AllElements {
			get {
				if (null == _allelements) {
					_allelements = GetAllElements().ToList();
				}
				return _allelements;
			}
		}
		/// <summary>
		/// Текущая задача на построение
		/// </summary>
		[IgnoreSerialize]
		public Task BuildTask { get; set; }

		/// <summary>
		/// Ошибка компиляции
		/// </summary>
		[SerializeNotNullOnly]
		public Exception Error { get; set; }

	
		/// <summary>
		/// Для расширений - имя целевого класса
		/// </summary>
		[SerializeNotNullOnly]
		public string TargetClassName { get; set; }


		/// <summary>
		/// Метод построения собственного индекса параметров
		/// </summary>
		/// <returns></returns>
		private IConfig BuildSelfParametesSource() {
			var result = new ConfigBase();
			foreach (var a in Source.Attributes()) {
				result.Set(a.Name.LocalName, a.Value);
			}
			return result;
		}

		private IBSharpClass[] _cachedImports;
		private IBSharpContext _context;

		/// <summary>
		///     Возвращает полное перечисление импортируемых классов в порядке их накатывания
		/// </summary>
		/// <value></value>
		[IgnoreSerialize]
		public IEnumerable<IBSharpClass> AllImports {
			get {
				if (null != _cachedImports) return _cachedImports;
				lock (this) {
					_cachedImports = GetAllImports(FullName, new ConfigBase()).Distinct().ToArray();
					return _cachedImports;
				}
			}
		}


		/// <summary>
		/// Собирает все определения мержей из класса
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IBSharpElement> GetAllElements() {
			return AllImports.SelectMany(_ => _.SelfElements).Union(SelfElements).Distinct();
		}

		

		private IEnumerable<IBSharpClass> GetAllImports(string root,IConfig config) {

			var dict = ((IDictionary<string, object>) config);
			var self = ((IDictionary<string, object>)BuildSelfParametesSource());
			foreach (var p in self.Where(_=>_.Value!=null)) {
				if (!dict.ContainsKey(p.Key) && !p.Value.ToStr().Contains("${")) {
					dict[p.Key] = p.Value;
				}
			}

			if (null != DefaultImport) {
				if (root != DefaultImport.FullName) {
					foreach (var i in ((BSharpClass)DefaultImport).GetAllImports(root,config)) {
						yield return i;
					}
					yield return DefaultImport;
				}
			}

			


			foreach (IBSharpImport i in SelfImports) {
				if (null!=i.Target
					&&
					!i.Target.IsOrphaned
					&& !i.Target.Is(BSharpClassAttributes.Ignored)
					&& i.Match(config)
					) {

					if (root != i.Target.FullName) {
						if (!i.Target.Is(BSharpClassAttributes.Static)) {
							foreach (BSharpClass ic in ((BSharpClass) i.Target).GetAllImports(root, config)) {
								yield return ic;
							}
						}
						yield return i.Target;
					}
					else {
						_context.RegisterError(BSharpErrors.RecycleImport(this,root,i));
					}
				}
			}

			foreach (var i in SelfImports.Where(_ =>null!=_.Target && _.Target.IsOrphaned)) {
				_context.RegisterError(BSharpErrors.OrphanImport(this, i));
			}
			foreach (var i in SelfImports.Where(_ => null != _.Target && _.Target.Is(BSharpClassAttributes.Ignored) && _.Match(config)))
			{
				_context.RegisterError(BSharpErrors.IgnoredImport(this, i));
			}
		}

		/// <summary>
		///     Полная проверка статуса Orphan
		/// </summary>
		/// <value></value>
		[SerializeNotNullOnly]
		public bool IsOrphaned {
			get {
				if (Is(BSharpClassAttributes.Explicit)) return false;
				if (Is(BSharpClassAttributes.Orphan)) return true;
				if (null == DefaultImport) return true;
				return DefaultImport.IsOrphaned;
			}
		}
	}
}