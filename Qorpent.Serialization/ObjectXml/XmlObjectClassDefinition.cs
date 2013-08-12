using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.Utils.Extensions;

namespace Qorpent.ObjectXml {
	/// <summary>
	/// 
	/// </summary>
	public class XmlObjectClassDefinition {
		/// <summary>
		/// 
		/// </summary>
		public XmlObjectClassDefinition() {
			MergeDefs = new List<XmlObjectMergeDefinition>();
			Imports = new List<XmlObjectImportDescription>();
		}

		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }


		/// <summary>
		/// Расширение имени, пакет, используется при конфликтующем разрешении имен
		/// по умолчанию если классы указаны в Namespace резолюция ведется только в рамках
		/// этого namespace, если без namespace, то глобально (RootNs)
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		/// Полное имя
		/// </summary>
		public string FullName { get {
			if (string.IsNullOrWhiteSpace(Namespace)) return Name;
			return Namespace + "." + Name;
		}}

		/// <summary>
		/// Признак абстракции
		/// </summary>
		public bool Abstract { get; set; }

		/// <summary>
		/// Класс, для которого еще не сопоставлен реальный тип
		/// </summary>
		public bool Orphaned { get; set; }

		/// <summary>
		/// Код первичного класса импорта
		/// </summary>
		public XmlObjectClassDefinition DefaultImport { get; set; }

		/// <summary>
		/// Код первичного класса импорта
		/// </summary>
		public string DefaultImportCode { get; set; }

		/// <summary>
		/// Явные импорты
		/// </summary>
		public IList<XmlObjectImportDescription> Imports { get; private set; }
		/// <summary>
		/// Определение сводимых элементов
		/// </summary>
		public IList<XmlObjectMergeDefinition> MergeDefs { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public XElement Source { get; set; }
		/// <summary>
		/// Компилированная версия класса
		/// </summary>
		public XElement Compiled { get; set; }
		/// <summary>
		/// Признак явного создания класса через ключевое слово
		/// </summary>
		public bool ExplicitClass { get; set; }
		/// <summary>
		/// Элемент хранящий данные об индексе параметров
		/// </summary>
		public IConfig ParamSourceIndex { get; set; }
		/// <summary>
		/// Сведенный словарь параметров
		/// </summary>
		public IConfig ParamIndex { get; set; }


		/// <summary>
		/// Возвращает XML для резолюции атрибутов
		/// </summary>
		/// <returns></returns>
		public IConfig BuildParametersConfig() {
			var result = new ConfigBase();
			var current = result;
			foreach (var i in CollectImports().Union(new[]{this})) {
				var selfconfig = new ConfigBase();
				selfconfig.Set("_class_", FullName);
				selfconfig.SetParent(current);
				current = selfconfig;
				foreach (var a in i.Source.Attributes()) {
					current.Set(a.Name.LocalName, a.Value);
				}
			}
			return current;
		}

	

		 

		/// <summary>
		/// Возвращает полное перечисление импортируемых классов в порядке их накатывания
		/// </summary>
		/// <returns></returns>
		public IEnumerable<XmlObjectClassDefinition> CollectImports(string root = null) {
			if (null == root) {
				root = this.Name;
			}
			return RawCollectImports(root).ToArray().Distinct();
		}

		private IEnumerable<XmlObjectClassDefinition> RawCollectImports(string root)
		{
			if (null != DefaultImport) {
				if (root != DefaultImport.Name) {
					foreach (var i in DefaultImport.CollectImports(root)) {
						yield return i;
					}
					yield return DefaultImport;
				}
			}
			foreach (var i in Imports) {
				if (!(i.Orphaned || i.Target.DetectIfIsOrphaned())
					&& root!=i.TargetCode
					) {
					foreach (var ic in i.Target.CollectImports(root))
					{
						yield return ic;
					}
					yield return i.Target;
				}
			}

		}

		/// <summary>
		/// Полная проверка статуса Orphan
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