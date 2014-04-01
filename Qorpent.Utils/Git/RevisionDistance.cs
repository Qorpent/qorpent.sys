using Qorpent.Serialization;

namespace Qorpent.Utils.Git{
	/// <summary>
	/// Расстояние между двумя ревизиями
	/// </summary>
	[Serialize]
	public class  RevisionDistance{
		/// <summary>
		/// Вперед
		/// </summary>
		public int Forward { get; set; }
		/// <summary>
		/// Назад
		/// </summary>
		public int Behind { get; set; }
		/// <summary>
		/// Признак отсутствия дистанции
		/// </summary>
		public bool IsZero{
			get { return Forward == 0 && Behind == 0; }
		}
		/// <summary>
		/// Признак бранча, который может быть обновлен до цели
		/// </summary>
		public bool IsForwardable{
			get { return Behind == 0 && Forward!=0; }
		}
		/// <summary>
		/// Признак бранча, который может использоваться как основа апдейта
		/// </summary>
		public bool IsUpdateable{
			get { return Forward == 0 && Behind != 0; }
		}
	}
}