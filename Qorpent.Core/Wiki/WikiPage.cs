using System;
using System.Collections.Generic;
using Qorpent.Serialization;

namespace Qorpent.Wiki {
	/// <summary>
	///     Описывает Wiki-страницу
	/// </summary>
	[Serialize]
	public class WikiPage {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected bool Equals(WikiPage other) {
			return string.Equals(Code, other.Code) && Existed.Equals(other.Existed) && string.Equals(Text, other.Text) && LastWriteTime.Equals(other.LastWriteTime) && string.Equals(Owner, other.Owner) && string.Equals(Editor, other.Editor) && string.Equals(Title, other.Title) && string.Equals(Version, other.Version) && Published.Equals(other.Published) && string.Equals(Locker, other.Locker);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() {
			unchecked {
				int hashCode = (Code != null ? Code.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ Existed.GetHashCode();
				hashCode = (hashCode*397) ^ (Text != null ? Text.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ LastWriteTime.GetHashCode();
				hashCode = (hashCode*397) ^ (Owner != null ? Owner.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (Editor != null ? Editor.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (Title != null ? Title.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (Version != null ? Version.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ Published.GetHashCode();
				hashCode = (hashCode*397) ^ (Locker != null ? Locker.GetHashCode() : 0);
				return hashCode;
			}
		}

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

        /// <summary>
        ///     Версия страницы
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        ///     Дата публикации страницы
        /// </summary>
        public DateTime Published { get; set; }

        /// <summary>
        ///     Юзер, залочивший страницу
        /// </summary>
        public string Locker { get; set; }
		/// <summary>
		/// Формирует копию страницы
		/// </summary>
		/// <returns></returns>
		public WikiPage Clone() {
			var result = MemberwiseClone() as WikiPage;
			result.Propeties = new Dictionary<string, string>();
			foreach (var propety in this.Propeties) {
				result.Propeties[propety.Key] = propety.Value;
			}
			return result;
		}
		/// <summary>
		/// Про
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((WikiPage) obj);
		}
	}
}