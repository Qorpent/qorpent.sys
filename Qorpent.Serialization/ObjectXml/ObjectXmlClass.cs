using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Config;

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
		///     Возвращает XML для резолюции атрибутов
		/// </summary>
		/// <returns></returns>
		public IConfig BuildParametersConfig() {
			var result = new ConfigBase();
			ConfigBase current = result;
			foreach (ObjectXmlClass i in CollectImports().Union(new[] {this})) {
				var selfconfig = new ConfigBase();
				selfconfig.Set("_class_", FullName);
				selfconfig.SetParent(current);
				current = selfconfig;
				foreach (XAttribute a in i.Source.Attributes()) {
					current.Set(a.Name.LocalName, a.Value);
				}
			}
			return current;
		}


		/// <summary>
		///     Возвращает полное перечисление импортируемых классов в порядке их накатывания
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ObjectXmlClass> CollectImports(string root = null) {
			if (null == root) {
				root = Name;
			}
			return RawCollectImports(root).ToArray().Distinct();
		}

		private IEnumerable<ObjectXmlClass> RawCollectImports(string root) {
			if (null != DefaultImport) {
				if (root != DefaultImport.Name) {
					foreach (ObjectXmlClass i in DefaultImport.CollectImports(root)) {
						yield return i;
					}
					yield return DefaultImport;
				}
			}
			foreach (ObjectXmlImport i in Imports) {
				if (!(i.Orphaned || i.Target.DetectIfIsOrphaned())
				    && root != i.TargetCode
					) {
					foreach (ObjectXmlClass ic in i.Target.CollectImports(root)) {
						yield return ic;
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