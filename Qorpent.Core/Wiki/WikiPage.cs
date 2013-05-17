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
		[Serialize]
		public string Code { get; set; }
		/// <summary>
		/// Признак того, что страница существует
		/// </summary>
		[Serialize]
		public bool Existed { get; set; }
		/// <summary>
		/// Метаданные Wiki
		/// </summary>
		[Serialize]
		public IDictionary<string,string> Propeties { get; private set; }
		/// <summary>
		/// Собственно текст страницы
		/// </summary>
		[Serialize]
		public string Text { get; set; }
		/// <summary>
		/// Время последней редакции
		/// </summary>
		[Serialize]
		public DateTime LastWriteTime { get; set; }
		/// <summary>
		/// Владелец
		/// </summary>
		[Serialize]
		public string Owner { get; set; }

		/// <summary>
		///  Автор последней редакции
		/// </summary>
		[Serialize]
		public string Editor { get; set; }
	}
}