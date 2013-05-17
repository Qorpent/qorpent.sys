using System;
using System.Collections.Generic;
using Qorpent.Serialization;

namespace Qorpent.Wiki {
	/// <summary>
	/// Описывает Wiki-страницу
	/// </summary>
	[Serialize]
	public	class WikiPage {
		/// <summary>
		/// Создает пустую страницу
		/// </summary>
		public WikiPage() {
			Propeties = new Dictionary<string, string>();
		}
		/// <summary>
		/// Код страницы
		/// </summary>
		[SerializeNotNullOnly]
		public string Code { get; set; }
		/// <summary>
		/// Признак того, что страница существует
		/// </summary>
		[SerializeNotNullOnly]
		public bool Existed { get; set; }
		/// <summary>
		/// Метаданные Wiki
		/// </summary>
		[SerializeNotNullOnly]
		public IDictionary<string,string> Propeties { get; private set; }
		/// <summary>
		/// Собственно текст страницы
		/// </summary>
		[SerializeNotNullOnly]
		public string Text { get; set; }
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
	}
}