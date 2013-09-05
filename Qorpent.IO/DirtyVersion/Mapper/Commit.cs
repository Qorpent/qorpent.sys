using System.Collections.Generic;

namespace Qorpent.IO.DirtyVersion.Mapper {
	/// <summary>
	/// Описание коммита
	/// </summary>
	public class Commit {
		private IList<CommitAuthorInfo> _coCommitHistoryInfo;
		private IList<string> _soruces;

		/// <summary>
		/// Обратная ссылка на объект
		/// </summary>
		public MappingInfo MappingInfo { get; set; }
		/// <summary>
		/// Хэш ключ коммита
		/// </summary>
		public string Hash { get; set; }
		/// <summary>
		/// Информация об авторе коммита
		/// </summary>
		public CommitAuthorInfo CommitAuthorInfo { get; set; }
		/// <summary>
		/// При совпадении бинарного содержания коммита - информация о "соавторах"
		/// </summary>
		public IList<CommitAuthorInfo> CoAuthors {
			get { return _coCommitHistoryInfo ?? (_coCommitHistoryInfo = new List<CommitAuthorInfo>()); }
			set { _coCommitHistoryInfo = value; }
		}

		/// <summary>
		/// Информация об общем характере происхождения коммита
		/// </summary>
		public CommitSourceType SourceType { get; set; }
		/// <summary>
		/// Состояние головы
		/// </summary>
		public HeadState HeadState { get; set; } 
		/// <summary>
		/// Коммиты - источники
		/// </summary>
		public IList<string> Sources {
			get { return _soruces ?? (_soruces = new List<string>()); }
			set { _soruces = value; }
		}
		/// <summary>
		/// Количество исходящих ссылок на коммит
		/// </summary>
		public int Refs { get; set; }

		/// <summary>
		/// Признак наличия соавторов
		/// </summary>
		/// <returns></returns>
		public bool HasCoAuthors() {
			return _coCommitHistoryInfo != null && _coCommitHistoryInfo.Count != 0;
		}

		/// <summary>
		/// Признак наличия источников
		/// </summary>
		/// <returns></returns>
		public bool HasSources()
		{
			return _soruces != null && _soruces.Count != 0;
		}
	}
}