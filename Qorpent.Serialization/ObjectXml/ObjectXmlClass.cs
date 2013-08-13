using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.Utils.Extensions;

namespace Qorpent.ObjectXml {
	/// <summary>
	/// </summary>
	public class ObjectXmlClass {
		/// <summary>
		/// </summary>
		public ObjectXmlClass() {
			MergeDefs = new List<ObjectXmlMerge>();
			Imports = new List<ObjectXmlImport>();
		}

		/// <summary>
		/// </summary>
		public string Name { get; set; }


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
		///     Признак абстракции
		/// </summary>
		public bool Abstract { get; set; }


		/// <summary>
		///     Признак статического класса
		/// </summary>
		public bool Static { get; set; }

		/// <summary>
		///     Класс, для которого еще не сопоставлен реальный тип
		/// </summary>
		public bool Orphaned { get; set; }

		/// <summary>
		///     Код первичного класса импорта
		/// </summary>
		public ObjectXmlClass DefaultImport { get; set; }

		/// <summary>
		///     Код первичного класса импорта
		/// </summary>
		public string DefaultImportCode { get; set; }

		/// <summary>
		///     Явные импорты
		/// </summary>
		public IList<ObjectXmlImport> Imports { get; private set; }

		/// <summary>
		///     Определение сводимых элементов
		/// </summary>
		public IList<ObjectXmlMerge> MergeDefs { get; private set; }

		/// <summary>
		/// </summary>
		public XElement Source { get; set; }

		/// <summary>
		///     Компилированная версия класса
		/// </summary>
		public XElement Compiled { get; set; }

		/// <summary>
		///     Признак явного создания класса через ключевое слово
		/// </summary>
		public bool ExplicitClass { get; set; }

		/// <summary>
		///     Элемент хранящий данные об индексе параметров
		/// </summary>
		public IConfig ParamSourceIndex { get; set; }

		/// <summary>
		///     Сведенный словарь параметров
		/// </summary>
		public IConfig ParamIndex { get; set; }
		/// <summary>
		/// Признак класса с закоченным билдом
		/// </summary>
		public bool IsBuilt { get; set; }
		/// <summary>
		/// Список всех определений мержа
		/// </summary>
		public List<ObjectXmlMerge> AllMergeDefs { get; set; }
		/// <summary>
		/// Текущая задача на построение
		/// </summary>
		public Task BuildTask { get; set; }
		/// <summary>
		/// Флаг того, что класс находится в режиме построения
		/// </summary>
		public bool InBuiltMode { get; set; }


		/// <summary>
		/// Метод построения собственного индекса параметров
		/// </summary>
		/// <returns></returns>
		public IConfig BuildSelfParametesSource() {
			var result = new ConfigBase();
			foreach (var a in Source.Attributes()) {
				result.Set(a.Name.LocalName, a.Value);
			}
			return result;
		}


		/// <summary>
		///     Возвращает полное перечисление импортируемых классов в порядке их накатывания
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ObjectXmlClass> CollectImports(string root = null, IConfig config = null)
		{
			if (null == root) {
				root = Name;
			}
			config = config ?? new ConfigBase();
			return RawCollectImports(root,config).ToArray().Distinct();
		}
		/// <summary>
		/// Собирает все определения мержей из класса
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ObjectXmlMerge> CollectMerges() {
			return CollectImports().SelectMany(_ => _.MergeDefs).Union(MergeDefs).Distinct();
		}

		

		private IEnumerable<ObjectXmlClass> RawCollectImports(string root,IConfig config) {

			var dict = ((IDictionary<string, object>) config);
			var self = ((IDictionary<string, object>)BuildSelfParametesSource());
			foreach (var p in self.Where(_=>_.Value!=null)) {
				if (!dict.ContainsKey(p.Key) && !p.Value.ToStr().Contains("${")) {
					dict[p.Key] = p.Value;
				}
			}

			if (null != DefaultImport) {
				if (root != DefaultImport.Name) {
					foreach (ObjectXmlClass i in DefaultImport.CollectImports(root,config)) {
						yield return i;
					}
					yield return DefaultImport;
				}
			}



			foreach (ObjectXmlImport i in Imports) {
				if (null!=i.Target
					&&
					!i.Target.DetectIfIsOrphaned()
				    && root != i.Target.FullName
					&& i.Match(config)
					) {
					if (!i.Target.Static) {
						foreach (ObjectXmlClass ic in i.Target.CollectImports(root)) {
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
		public bool DetectIfIsOrphaned() {
			if (ExplicitClass) return false;
			if (Orphaned) return true;
			if (null == DefaultImport) return true;
			return DefaultImport.DetectIfIsOrphaned();
		}
	}
}