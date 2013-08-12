using System.Collections.Generic;
using System.Linq;
using Qorpent.Config;

namespace Qorpent.ObjectXml {
	/// <summary>
	/// Внутренний индекс компилятора
	/// </summary>
	public class ObjectXmlCompilerIndex :ConfigBase {
		private const string RAWCLASSES = "rawclasses";
		private const string ORPHANED = "orphaned";
		private const string ABSTRACTS = "abstracts";
		private const string WORKING = "working";

		/// <summary>
		/// Исходные сырые определения классов
		/// </summary>
		public IDictionary<string,XmlObjectClassDefinition> RawClasses {
			get { return Get<IDictionary<string, XmlObjectClassDefinition>>(RAWCLASSES); }
			set { Set(RAWCLASSES, value); }
		}
		
		/// <summary>
		/// Классы с непроинициализированным наследованием
		/// </summary>
		public List<XmlObjectClassDefinition> Orphaned {
			get { return Get<List<XmlObjectClassDefinition>>(ORPHANED); }
			set { Set(ORPHANED, value); }
		}

		/// <summary>
		/// Классы с непроинициализированным наследованием
		/// </summary>
		public List<XmlObjectClassDefinition> Abstracts
		{
			get { return Get<List<XmlObjectClassDefinition>>(ABSTRACTS); }
			set { Set(ABSTRACTS, value); }
		}
		/// <summary>
		/// Классы с непроинициализированным наследованием
		/// </summary>
		public List<XmlObjectClassDefinition> Working
		{
			get { return Get<List<XmlObjectClassDefinition>>(WORKING); }
			set { Set(WORKING, value); }
		}

	}
}