using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp {
	/// <summary>
	/// </summary>
	public class BSharpClass {
		/// <summary>
		/// </summary>
		public BSharpClass() {
			MergeDefs = new List<BSharpElement>();
			Imports = new List<BSharpImport>();
		}

		/// <summary>
		/// </summary>
		public string Name { get; set; }

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
				flags.HasFlag(BSharpClassAttributes.Override)
				||
				flags.HasFlag(BSharpClassAttributes.Extension)
				) {
				if (null == TargetClassName) {
					TargetClassName = Name;
					Name = Guid.NewGuid().ToString();
				}
				Set(BSharpClassAttributes.Explicit);
			}
			if (flags.HasFlag(BSharpClassAttributes.Explicit)) {
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
		public string Namespace { get; set; }

		/// <summary>
		///     Полное имя
		/// </summary>
		public string FullName {
			get {
				if (string.IsNullOrWhiteSpace(Namespace)) return Name;
				return Namespace + "." + Name;
			}
		}

		

		/// <summary>
		///     Код первичного класса импорта
		/// </summary>
		public BSharpClass DefaultImport { get; set; }

		/// <summary>
		///     Код первичного класса импорта
		/// </summary>
		public string DefaultImportCode { get; set; }

		/// <summary>
		///     Явные импорты
		/// </summary>
		public IList<BSharpImport> Imports { get; private set; }

		/// <summary>
		///     Определение сводимых элементов
		/// </summary>
		public IList<BSharpElement> MergeDefs { get; private set; }

		/// <summary>
		/// </summary>
		public XElement Source { get; set; }

		/// <summary>
		///     Компилированная версия класса
		/// </summary>
		public XElement Compiled { get; set; }

		/// <summary>
		///     Элемент хранящий данные об индексе параметров
		/// </summary>
		public IConfig ParamSourceIndex { get; set; }

		/// <summary>
		///     Сведенный словарь параметров
		/// </summary>
		public IConfig ParamIndex { get; set; }
		
		/// <summary>
		/// Список всех определений мержа
		/// </summary>
		public List<BSharpElement> AllMergeDefs { get; set; }
		/// <summary>
		/// Текущая задача на построение
		/// </summary>
		public Task BuildTask { get; set; }

		/// <summary>
		/// Ошибка компиляции
		/// </summary>
		public Exception Error { get; set; }

	
		/// <summary>
		/// Для расширений - имя целевого класса
		/// </summary>
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

		private BSharpClass[] _cachedImports;
		/// <summary>
		///     Возвращает полное перечисление импортируемых классов в порядке их накатывания
		/// </summary>
		/// <returns></returns>
		public IEnumerable<BSharpClass> CollectImports(string root = null, IConfig config = null) {
			if (null != _cachedImports) return _cachedImports;
			lock (this) {

				if (null == root) {
					root = Name;
				}
				config = config ?? new ConfigBase();
				_cachedImports = RawCollectImports(root, config).Distinct().ToArray();
				return _cachedImports;
			}
		}


		/// <summary>
		/// Собирает все определения мержей из класса
		/// </summary>
		/// <returns></returns>
		public IEnumerable<BSharpElement> CollectMerges() {
			return CollectImports().SelectMany(_ => _.MergeDefs).Union(MergeDefs).Distinct();
		}

		

		private IEnumerable<BSharpClass> RawCollectImports(string root,IConfig config) {

			var dict = ((IDictionary<string, object>) config);
			var self = ((IDictionary<string, object>)BuildSelfParametesSource());
			foreach (var p in self.Where(_=>_.Value!=null)) {
				if (!dict.ContainsKey(p.Key) && !p.Value.ToStr().Contains("${")) {
					dict[p.Key] = p.Value;
				}
			}

			if (null != DefaultImport) {
				if (root != DefaultImport.Name) {
					foreach (var i in DefaultImport.RawCollectImports(root,config)) {
						yield return i;
					}
					yield return DefaultImport;
				}
			}



			foreach (BSharpImport i in Imports) {
				if (null!=i.Target
					&&
					!i.Target.IsOrphaned()
				    && root != i.Target.FullName
					&& i.Match(config)
					) {
					if (!i.Target.Is(BSharpClassAttributes.Static)) {
						foreach (BSharpClass ic in i.Target.RawCollectImports(root,config)) {
							yield return ic;
						}
					}
					yield return i.Target;
				}
			}
		}

		/// <summary>
		///     Полная проверка статуса Orphan
		/// </summary>
		/// <returns></returns>
		public bool IsOrphaned() {
			if (Is(BSharpClassAttributes.Explicit)) return false;
			if (Is(BSharpClassAttributes.Orphan)) return true;
			if (null == DefaultImport) return true;
			return DefaultImport.IsOrphaned();
		}
	}
}