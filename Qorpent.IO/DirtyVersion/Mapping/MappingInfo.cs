using System.Collections.Generic;
using System.Linq;

namespace Qorpent.IO.DirtyVersion.Mapping {

	


	/// <summary>
	/// Информация 
	/// </summary>
	public class MappingInfo {
		/// <summary>
		/// 
		/// </summary>
		public MappingInfo() {
			Commits = new Dictionary<string,Commit>();
			Aliases = new Dictionary<string, string>();
		}
		/// <summary>
		/// Разрешает коммит по имени или по псевдониму
		/// </summary>
		/// <param name="hashOrAlias"></param>
		/// <returns></returns>
		public Commit Resolve(string hashOrAlias) {
			var hash = hashOrAlias;
			if (Aliases.ContainsKey(hashOrAlias)) {
				hash = Aliases[hashOrAlias];
			}
			if (Commits.ContainsKey(hash)) {
				return Commits[hash];
			}
			return null;
		}
		/// <summary>
		/// Исходное имя
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Хэш исходного имени
		/// </summary>
		public string NameHash { get; set; }
		/// <summary>
		/// Хэш основного текущего коммита
		/// </summary>
		public string Head { get; set; }
		/// <summary>
		/// Полный лог коммитов
		/// </summary>
		public IDictionary<string,Commit> Commits { get; private set; }
		/// <summary>
		/// Прсевдонимы коммитов
		/// </summary>
		public IDictionary<string,string> Aliases { get; private set; }
		/// <summary>
		/// Признак изменения
		/// </summary>
		public bool Changed { get; set; }

		/// <summary>
		/// Нормализует мэпинг
		/// </summary>
		public void Normalize() {
			var headcommit = Resolve(Head);
			Init();
			CalculateReferences();
			if (null != headcommit) {
				headcommit.HeadState = HeadState.IsHead;
				PropagateMergedState(headcommit,false);
			}
			SetupNonMergedState();
		}

		private void Init() {
			foreach (var c in Commits.Values) {
				c.Refs = 0;
				c.HeadState = HeadState.None;
				c.MappingInfo = this;
			}
		}

		private void SetupNonMergedState() {
			foreach (var c in Commits.Values) {
				if (c.HeadState == HeadState.None) {
					if (c.Refs == 0) {
						c.HeadState = HeadState.NonMergedHead;
					}
					else {
						c.HeadState = HeadState.NonMerged;
					}
				}
			}
		}

		private void CalculateReferences() {
			foreach (var c in Commits.Values) {
				if (c.HasSources()) {
					foreach (var s in c.Sources) {
						var sc = Resolve(s);
						if (null != sc) {
							sc.Refs++;
						}
					}
				}
			}
		}

		private void PropagateMergedState(Commit c,bool self) {
			if(self)c.HeadState = HeadState.Merged;
			if (c.HasSources()) {
				foreach (var s in c.Sources) {
					var sc = Resolve(s);
					if(null!=sc)PropagateMergedState(sc,true);
				}
			}
		}

		/// <summary>
		/// Возвращает хид коммит
		/// </summary>
		/// <returns></returns>
		public Commit GetHead() {
			return Resolve(Head);
		}
		/// <summary>
		/// Получает список несмерженных версий
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Commit> GetNonMergedHeads() {
			return Commits.Values.Where(_ => _.HeadState == HeadState.NonMergedHead);
		}

	}
}