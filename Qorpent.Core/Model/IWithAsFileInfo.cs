using System;

namespace Qorpent.Model {
	/// <summary>
	/// Интерфейс а-ля файловой информации для сущностей - создатель, последний редактор, время создания и правки
	/// </summary>
	public interface IWithAsFileInfo {
		/// <summary>
		/// Создатель, владелец сущности
		/// </summary>
		string Owner { get; set; }

		/// <summary>
		/// Последний правщик, редактор сущности
		/// </summary>
		string Updater { get; set; }

		/// <summary>
		/// Время создания сущности
		/// </summary>
		DateTime Created { get; set; }

		/// <summary>
		/// Время правки сущности
		/// </summary>
		DateTime Updated { get; set; }
	}
}