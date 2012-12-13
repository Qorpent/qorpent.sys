using System;
using Qorpent.Model;

namespace Zeta.Data.Model {
	/// <summary>
	/// Базовый класс сущности
	/// </summary>
	public class Entity : IEntity {
		/// <summary>
		/// 	Целочисленный уникальный идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 	Строковый уникальный идентификатор
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 	Название/имя
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 	An index of object
		/// </summary>
		public int Idx { get; set; }

		/// <summary>
		/// 	Строка тегов
		/// </summary>
		public string Tag { get; set; }

		/// <summary>
		/// 	Комментарий
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// 	Название
		/// </summary>
		public DateTime Version { get; set; }
	}
}