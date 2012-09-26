using System;

namespace Qorpent.Mvc {
	/// <summary>
	/// Интерфейс дескриптора абстрактного файла
	/// </summary>
	public interface IFileDescriptor : IWithRole {
		/// <summary>
		/// Имя ресурса возможно с путем
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Содержимое файла
		/// </summary>
		string Content { get; set; }

		/// <summary>
		/// Mime-Type файла
		/// </summary>
		string MimeType { get; set; }

		/// <summary>
		/// Длина файла
		/// </summary>
		int Length { get; set; }

		/// <summary>
		/// Время последнего изменения
		/// </summary>
		DateTime LastWriteTime { get; set; }
	}
}