using System.Collections.Generic;
using System.Linq;
using Qorpent.Config;

namespace Qorpent.ObjectXml {
	/// <summary>
	///     Внутренний индекс компилятора
	/// </summary>
	public class ObjectXmlCompilerIndex : ConfigBase {
		private const string RAWCLASSES = "rawclasses";
		private const string ORPHANED = "orphaned";
		private const string ABSTRACTS = "abstracts";
		private const string WORKING = "working";
		private const string STATIC = "static";
		/// <summary>
		/// Возвращает рабочий класс по коду
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ObjectXmlClass Get(string name) {
			var full = Working.FirstOrDefault(_ => _.FullName == name);
			if (null != full) return full;
			return Working.FirstOrDefault(_ => _.Name == name);
		}

		/// <summary>
		///     Исходные сырые определения классов
		/// </summary>
		public IDictionary<string, ObjectXmlClass> RawClasses {
			get { return Get<IDictionary<string, ObjectXmlClass>>(RAWCLASSES); }
			set { Set(RAWCLASSES, value); }
		}

		/// <summary>
		///     Классы с непроинициализированным наследованием
		/// </summary>
		public List<ObjectXmlClass> Orphans {
			get { return Get<List<ObjectXmlClass>>(ORPHANED); }
			set { Set(ORPHANED, value); }
		}

		/// <summary>
		///     Классы с непроинициализированным наследованием
		/// </summary>
		public List<ObjectXmlClass> Abstracts {
			get { return Get<List<ObjectXmlClass>>(ABSTRACTS); }
			set { Set(ABSTRACTS, value); }
		}

		/// <summary>
		///     Классы с непроинициализированным наследованием
		/// </summary>
		public List<ObjectXmlClass> Working {
			get { return Get<List<ObjectXmlClass>>(WORKING); }
			set { Set(WORKING, value); }
		}

		/// <summary>
		/// Классы со статической компиляцией
		/// </summary>
		public List<ObjectXmlClass> Static {
			get { return Get<List<ObjectXmlClass>>(STATIC); }
			set { Set(STATIC, value); }
		}

		/// <summary>
		///     Присоединяет и склеивается с другим результатом
		/// </summary>
		/// <param name="subresult"></param>
		public void Merge(ObjectXmlCompilerIndex subresult) {
			if (null != subresult.Abstracts) {
				if (null == Abstracts) {
					Abstracts = new List<ObjectXmlClass>();
				}
				foreach (ObjectXmlClass a in subresult.Abstracts) {
					Abstracts.Add(a);
				}
			}
			if (null != subresult.Orphans) {
				if (null == Orphans) {
					Orphans = new List<ObjectXmlClass>();
				}
				foreach (ObjectXmlClass a in subresult.Orphans) {
					Orphans.Add(a);
				}
			}
			if (null != subresult.Working) {
				if (null == Working) {
					Working = new List<ObjectXmlClass>();
				}
				foreach (ObjectXmlClass a in subresult.Working) {
					Working.Add(a);
				}
			}
		}
	}
}