using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Syncronization{
	/// <summary>
	/// 
	/// </summary>
	public class FileItem{
		/// <summary>
		/// 
		/// </summary>
		public FileItem(){
			
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <param name="op"></param>
		public FileItem(XElement e, FileOperation op){
			this.Operation = op;
			this.Name = e.Attr("n");
			this.Hash = e.Attr("h");
		}

		/// <summary>
		/// Название
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Хэш содержимого
		/// </summary>
		public string Hash { get; set; }
		/// <summary>
		/// Хэш для сравнения
		/// </summary>
		public string OtherHash { get; set; }
		/// <summary>
		/// Действие
		/// </summary>
		public FileOperation Operation { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			return Operation + " " + Name;
		}
		
	}
}