using System;
using Qorpent.Serialization;

namespace Qorpent.Wiki {
	/// <summary>
	/// Описывает Wiki-страницу
	/// </summary>
	[Serialize]
	public class WikiBinary
	{
		/// <summary>
		/// Код страницы
		/// </summary>
		[SerializeNotNullOnly]
		public string Code { get; set; }


		/// <summary>
		/// Mime - тип
		/// </summary>
		[SerializeNotNullOnly]
		public string MimeType { get; set; }
		/// <summary>
		/// Время последней редакции
		/// </summary>
		[SerializeNotNullOnly]
		public DateTime LastWriteTime { get; set; }
		/// <summary>
		/// Владелец
		/// </summary>
		[SerializeNotNullOnly]
		public string Owner { get; set; }

		/// <summary>
		///  Автор последней редакции
		/// </summary>
		[SerializeNotNullOnly]
		public string Editor { get; set; }

		/// <summary>
		/// Заголовок, имя страницы
		/// </summary>
		[SerializeNotNullOnly]
		public string Title { get; set; }
		/// <summary>
		/// Бинарные данные
		/// </summary>
		[SerializeNotNullOnly]
		public byte[] Data { get; set; }
		/// <summary>
		/// Размер файла
		/// </summary>
		public long Size { get; set; }
	}
}