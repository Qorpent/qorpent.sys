using System;
using System.Linq;

namespace Qorpent.IO.DirtyVersion.Mapping {
	/// <summary>
	/// 
	/// </summary>
	public class MappingOperator : IMappingOperator {
		private readonly IMapperSession _session;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		public MappingOperator(IMapperSession session)
		{
			_session = session;
		}
		/// <summary>
		/// Целевая информация по мапингу
		/// </summary>
		public MappingInfo MappingInfo {
			get { return _session.GetMappingInfo(); }
		}

		/// <summary>
		/// Мержит новый коммит в структуру
		/// </summary>
		/// <param name="commit"></param>
		/// <param name="commitHeadBehavior"></param>
		/// <returns></returns>
		public Commit Commit(Commit commit, CommitHeadBehavior commitHeadBehavior = CommitHeadBehavior.Auto) {
			MappingInfo.Changed = true;
			commit.MappingInfo = MappingInfo;
			commit.Normalize();
			var target = MappingInfo.Resolve(commit.Hash);
			if (null == target) {
				target = commit;
				MappingInfo.Commits[commit.Hash] = commit;
			}
			else {
				target.Merge(commit);
			}
		    if (target.Hash != MappingInfo.Head) {
		        ProcessHead(target, commitHeadBehavior);
		    }
		    MappingInfo.Normalize();
			return target;
		}
		/// <summary>
		/// Перемещает хид
		/// </summary>
		/// <param name="headHash"></param>
		public void MoveHead(string headHash) {
			var existed = MappingInfo.Resolve(headHash);
			if (null == existed) {
				throw new Exception("given commit not existed");
			}
			if (existed.Hash != MappingInfo.Head) {
				MappingInfo.Head = existed.Hash;
				MappingInfo.Changed = true;
			}
		}
		/// <summary>
		/// Устанавливает альяс
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="hash"></param>
		public void SetAlias(string alias, string hash) {
			if (!MappingInfo.Aliases.ContainsKey(alias) || MappingInfo.Aliases[alias] != hash) {
				MappingInfo.Aliases[alias] = hash;
				MappingInfo.Changed = true;
			}
		}
		/// <summary>
		/// Удаляет альяс
		/// </summary>
		/// <param name="alias"></param>
		public void RemoveAlias(string alias) {
			if (MappingInfo.Aliases.ContainsKey(alias)) {
				MappingInfo.Aliases.Remove(alias);
				MappingInfo.Changed = true;
			}
		}
		/// <summary>
		/// Удаляет коммит и все его зависимости
		/// </summary>
		/// <param name="commitHash"></param>
		/// <param name="deleteHeadBehavior"></param>
		public void Delete(string commitHash, DeleteHeadBehavior deleteHeadBehavior = DeleteHeadBehavior.Deny) {
			var existed = MappingInfo.Resolve(commitHash);
			if (null == existed) return;
			if (MappingInfo.Head == existed.Hash) {
				PreprocessHeadDeletion(existed, deleteHeadBehavior);
			}
			MappingInfo.Changed = true;
			MappingInfo.Commits.Remove(existed.Hash);
			foreach (var c in MappingInfo.Commits.Values) {
				if (c.HasSources()) {
					if (c.Sources.Contains(existed.Hash)) {
						c.MergeSources(existed);
						c.Sources.Remove(existed.Hash);
						c.Normalize();
					}
				}
			}
			MappingInfo.Normalize();

		}
		/// <summary>
		/// 
		/// </summary>
		public void Cancel() {
			_session.Cancel();
		}
		/// <summary>
		/// 
		/// </summary>
		public void Commit() {
			_session.Commit();
		}

		private void PreprocessHeadDeletion(Commit existed, DeleteHeadBehavior deleteHeadBehavior) {
			if (deleteHeadBehavior == DeleteHeadBehavior.Deny) {
				throw new Exception("cannot remove head when DeleteHeadBehavior.Deny");
			}
			if (deleteHeadBehavior == DeleteHeadBehavior.Detach) {
				MappingInfo.Head = Const.DETACHEDHEAD;
			}else {
				if (existed.SourceType == CommitSourceType.Initial) {
					MappingInfo.Head = Const.DETACHEDHEAD;	
				}else if (existed.SourceType == CommitSourceType.Single) {
					MappingInfo.Head = existed.Sources[0];
				}
				else {
					if (deleteHeadBehavior == DeleteHeadBehavior.AllowSingleDetachMerge) {
						MappingInfo.Head = Const.DETACHEDHEAD;
					}
					else {
						throw new Exception("cannot process deletion of merged head with DeleteHeadBehavior." + deleteHeadBehavior);
					}
				}
			}
		}

		private void ProcessHead(Commit target, CommitHeadBehavior commitHeadBehavior) {
			if (commitHeadBehavior != CommitHeadBehavior.Deny) {
				if (target.MappingInfo.Head == Const.DETACHEDHEAD) {
					target.MappingInfo.Head = target.Hash;
				}
				else if (commitHeadBehavior == CommitHeadBehavior.Direct) {
					target.MappingInfo.Head = target.Hash;
				}
				else if(commitHeadBehavior==CommitHeadBehavior.Auto) {
					if (target.GetAllSources().Any(_ => _ == target.MappingInfo.Head)) {
						target.MappingInfo.Head = target.Hash;
					}
				}else if (commitHeadBehavior == CommitHeadBehavior.Override) {
					if (target.GetAllSources().All(_ => _ != target.MappingInfo.Head))
					{
						if (target.SourceType == CommitSourceType.Initial) {
							target.SourceType=CommitSourceType.Single;
						
						}else if (target.SourceType == CommitSourceType.Single) {
							target.SourceType = CommitSourceType.Merged;
						}
						target.Sources.Add(target.MappingInfo.Head);
					}
					target.MappingInfo.Head = target.Hash;
				}
			}
		}


		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose() {
			_session.Dispose();
		}
		/// <summary>
		/// Возвращает хэш хида
		/// </summary>
		/// <returns></returns>
		public string GetHeadHash() {
			return MappingInfo.Head;
		}
		/// <summary>
		/// Осуществляет разрешение коммитов
		/// </summary>
		/// <param name="hash"></param>
		/// <returns></returns>
		public Commit Resolve(string hash) {
			return MappingInfo.Resolve(hash);
		}
	}
}