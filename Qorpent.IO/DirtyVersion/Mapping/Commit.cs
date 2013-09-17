using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.IO.DirtyVersion.Mapping {
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
		public CommitAuthorInfo Author { get; set; }
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
		/// <summary>
		/// Нормализует коммит
		/// </summary>
		public void Normalize() {
			if (string.IsNullOrWhiteSpace(Hash)) throw new Exception("hash cannot be empty");
			Author.Normalize();
			if (HasCoAuthors()) {
				foreach (var ca in CoAuthors) {
					ca.Normalize();
				}
			}
			if (HasSources()) {
				for (var i = 0; i < Sources.Count; i++) {
					var s = Sources[i];
					if (MappingInfo.Aliases.ContainsKey(s)) {
						Sources[i] = MappingInfo.Aliases[s];
					}
				}
				if (1 == Sources.Count) {
					SourceType = CommitSourceType.Single;
				}
				else {
					SourceType = CommitSourceType.Merged;
				}
			}
			else {
				SourceType = CommitSourceType.Initial;
			}
		}
		/// <summary>
		/// Сведение коммитов
		/// </summary>
		/// <param name="commit"></param>
		public void Merge(Commit commit) {
			MergeAuthors(commit);
			MergeSources(commit);
		}
		/// <summary>
		/// Перечисляет все источники
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetAllSources(IList<string> cache = null) {
			cache = cache?? new List<string>();
			if (!cache.Contains(Hash)) {
				cache.Add(Hash);
			}
			if (HasSources()) {
				foreach (var s in Sources) {
					if (!cache.Contains(s)) {
						cache.Add(s);
						yield return s;
						var c = MappingInfo.Resolve(s);
						foreach (var cs in c.GetAllSources(cache)) {
							yield return cs;
						}
					}
				}
			}
		}
		/// <summary>
		/// Применить исходники
		/// </summary>
		/// <param name="commit"></param>
		public void MergeSources(Commit commit) {
			if (commit.HasSources()) {
				if (!HasSources()) {
					SourceType = commit.SourceType;
					foreach (var s in commit.Sources) {
						Sources.Add(s);
					}
				}
				else {
					var additions = commit.Sources.Except(Sources).ToArray();
					if (0 != additions.Length) {
						SourceType = CommitSourceType.Merged;
						foreach (var s in additions) {
							Sources.Add(s);
						}
					}
				}
			}
		}

		private void MergeAuthors(Commit commit) {
			if (commit.Author.Time > Author.Time) {
				if (commit.Author.Commiter != Author.Commiter) {
					CoAuthors.Add(Author);
					Author = commit.Author;
				}
			}
			else {
				if (!CoAuthors.Contains(commit.Author)) {
					commit.CoAuthors.Add(commit.Author);
				}
			}
		}
	}
}