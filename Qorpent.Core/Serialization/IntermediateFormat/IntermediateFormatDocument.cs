using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qorpent.Config;

namespace Qorpent.Serialization.IntermediateFormat {
	/// <summary>
	/// Переносимая структура в формате IntermediateDocument
	/// </summary>
	public class IntermediateFormatDocument: ConfigBase {
		/// <summary>
		/// Ссылка на запрос - контейнер
		/// </summary>
		public IntermediateFormatQuery Query{
			get { return Get<IntermediateFormatQuery>("query"); }
			set { Set("query",value); }
		}

		/// <summary>
		/// Записывает ZIF документ в поток в соответствии с запросом
		/// </summary>
		/// <param name="output">Поток вывода</param>
		/// <param name="query">Запрос ZIF</param>
		public void Write(Stream output, IntermediateFormatQuery query = null){
			query = query ?? Query;
			if (null == query){
				throw new IntermediateFormatException("query was not given for document output",null);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private readonly IList<IntermediateFormatDocument> _children =new List<IntermediateFormatDocument>();
		/// <summary>
		/// 
		/// </summary>
		public IntermediateFormatDocument[] Children{
			get { return _children.ToArray(); }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="document"></param>
		public void AddChildDocument(IntermediateFormatDocument document){
			document.SetParent(this);
			_children.Add(document);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="document"></param>
		public void RemoveChildDocument(IntermediateFormatDocument document){
			_children.Remove(document);
		}
	}
}