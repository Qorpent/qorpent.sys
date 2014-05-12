using System.Xml.Linq;

namespace Qorpent.Data.DataDiff{
	/// <summary>
	/// Пара на обновление
	/// </summary>
	public class DiffPair{
		/// <summary>
		/// Имя файла
		/// </summary>
		public string FileName { get; set; }
		/// <summary>
		/// Базовый элемент
		/// </summary>
		public XElement Base { get; set; }

		/// <summary>
		/// Обновленный элемент
		/// </summary>
		public XElement Updated { get; set; }
	}
}