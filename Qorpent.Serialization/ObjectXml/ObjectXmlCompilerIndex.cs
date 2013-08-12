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
		public List<XmlObjectClassDefinition> Orphans {
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
		/// <summary>
		/// Присоединяет и склеивается с другим результатом
		/// </summary>
		/// <param name="subresult"></param>
		public void Merge(ObjectXmlCompilerIndex subresult) {
			if (null != subresult.Abstracts) {
				if (null == this.Abstracts) {
					Abstracts = new List<XmlObjectClassDefinition>();
				}
				foreach (var a in subresult.Abstracts) {
					this.Abstracts.Add(a);
				}
			}
			if (null != subresult.Orphans) {
				if (null == this.Orphans)
				{
					this.Orphans = new List<XmlObjectClassDefinition>();
				}
				foreach (var a in subresult.Orphans) {
					this.Orphans.Add(a);
				}
			}
			if (null != subresult.Working) {
				if (null == this.Working)
				{
					this.Working = new List<XmlObjectClassDefinition>();
				}
				foreach (var a in subresult.Working) {
					this.Working.Add(a);
				}
			}
		}
	}
}