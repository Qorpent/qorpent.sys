using System;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// Описатель ревизии
	/// </summary>
	public class RevisionDescriptor{
		/// <summary>
		/// Номер ревизии
		/// </summary>
		public string Revision { get; set; }
		/// <summary>
		/// Хэш контента
		/// </summary>
		public string Hash { get; set; }
		/// <summary>
		/// Время создания ревизии
		/// </summary>
		public DateTime RevisionTime { get; set; }

		/// <summary>
		/// Сверяет идентичность версий
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool IsMatchRevision(RevisionDescriptor other){
			if (null == other) return false;
			if (!string.IsNullOrWhiteSpace(other.Revision)){
				if (other.Revision != this.Revision) return false;
			}
			if (!string.IsNullOrWhiteSpace(other.Hash))
			{
				if (other.Hash != this.Hash) return false;
			}
			if (other.RevisionTime.Year > 1900){
				if (other.RevisionTime != this.RevisionTime) return false;
			}
			return true;
		}
	}
}