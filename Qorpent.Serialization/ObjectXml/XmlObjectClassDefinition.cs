using System.Collections.Generic;
using System.Xml.Linq;

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