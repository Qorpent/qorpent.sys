using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.ObjectXml {
	/// <summary>
	/// Компилятор ObjectXml по умолчанию
	/// </summary>
	public class ObjectXmlCompiler : ObjectXmlCompilerBase {
		/// <summary>
		/// Текущий контекстный индекс
		/// </summary>
		protected ObjectXmlCompilerIndex _currentBuildIndex;

		protected override ObjectXmlCompilerIndex BuildIndex(IEnumerable<XElement> sources) {
			_currentBuildIndex = new ObjectXmlCompilerIndex();

			return _currentBuildIndex;
		}

		protected override IEnumerable<XElement> Link(IEnumerable<XElement> sources, ObjectXmlCompilerIndex index) {
			_currentBuildIndex = index;
			var _classes = ExtractClassDefinitions(sources, index);

		}

		private IEnumerable<XmlObjectClassDefinition> ExtractClassDefinitions(IEnumerable<XElement> sources, ObjectXmlCompilerIndex index) {
			throw new System.NotImplementedException();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class XmlObjectClassDefinition {
		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Признак абстракции
		/// </summary>
		public bool Abstract { get; set; }

		/// <summary>
		/// Явные импорты
		/// </summary>
		public XmlObjectImportDescription[] Imports { get; set; }

		public 
	}
	/// <summary>
	/// 
	/// </summary>
	public class XmlObjectImportDescription {
		/// <summary>
		/// 
		/// </summary>
		public XmlObjectClassDefinition Target { get; set; } 
		/// <summary>
		/// Тип импорта 
		/// </summary>
		public XmlObjectImportType ImportType { get; set; }

	}

	/// <summary>
	/// 
	/// </summary>
	public enum XmlObjectImportType {
		
	}
}