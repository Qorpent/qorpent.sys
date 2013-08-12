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

	public class XmlObjectClassDefinition {
		/// <summary>
		/// 
		/// </summary>
		public string Code { get; set; }
	}
}