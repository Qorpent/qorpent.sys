using System;
using Qorpent.Serialization;

namespace Qorpent.Wiki {
	/// <summary>
	/// Описывает объект каталога Wiki
	/// </summary>
	[Serialize]
	public class WikiObjectDescriptor {
		/// <summary>
		/// Создать дескриптор на основе страницы
		/// </summary>
		/// <param name="page"></param>
		public WikiObjectDescriptor(WikiPage page) {
			Code = page.Code;
			Type = WikiObjectType.Page;
			ContentType = "text/wiki";
			Size = -1;
			Name = page.Title;
			LastWriteTime = page.LastWriteTime;
		}
		/// <summary>
		/// Создать дескриптор на основе файла
		/// </summary>
		/// <param name="file"></param>
		
		public WikiObjectDescriptor(WikiBinary file) {
			Code = file.Code;
			Type = WikiObjectType.File;
			ContentType = file.MimeType;
			Size = file.Size;
			Name = file.Title;
			LastWriteTime = file.LastWriteTime;
		}

		/// <summary>
		/// Код объекта
		/// </summary>
		[Serialize]
		public string Code { get; set; }
		/// <summary>
		/// Тип объекта
		/// </summary>
		[Serialize]
		public WikiObjectType Type { get; set; }


		/// <summary>
		/// Mime-Type (для файлов)
		/// </summary>
		[SerializeNotNullOnly]
		public string ContentType { get; set; }

		/// <summary>
		/// Размер (для файлов)
		/// </summary>
		[SerializeNotNullOnly]
		public long Size { get; set; }

		/// <summary>
		/// Имя, название
		/// </summary>
		[Serialize]
		public string Name { get; set; }

		/// <summary>
		/// Время последней редакции
		/// </summary>
		[SerializeNotNullOnly]
		public DateTime LastWriteTime { get; set; }
		
	}
}