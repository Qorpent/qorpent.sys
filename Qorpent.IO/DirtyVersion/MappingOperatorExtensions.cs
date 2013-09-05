using System;
using Qorpent.IO.DirtyVersion.Mapping;

namespace Qorpent.IO.DirtyVersion {
	/// <summary>
	/// Расширения оператора
	/// </summary>
	public static class MappingOperatorExtensions {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="op"></param>
		/// <param name="hash"></param>
		/// <param name="sources"></param>
		/// <returns></returns>
		public static Commit Commit(this IMappingOperator op, string hash, params string[] sources) {
			return Commit(op, hash, null, sources);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="op"></param>
		/// <param name="hash"></param>
		/// <param name="commiter"></param>
		/// <param name="sources"></param>
		/// <returns></returns>
		public static Commit Commit(this IMappingOperator op, string hash, string commiter, params string[] sources)
		{
			var cb = CommitHeadBehavior.Auto;
			if (null == sources || 0 == sources.Length) {
				cb = CommitHeadBehavior.Override;
			}
			return Commit(op,hash, commiter, cb, sources);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="op"></param>
		/// <param name="hash"></param>
		/// <param name="behavior"></param>
		/// <param name="sources"></param>
		/// <returns></returns>
		public static Commit Commit(this IMappingOperator op, string hash, CommitHeadBehavior behavior, params string[] sources) {
			return Commit(op, hash, null, behavior, sources);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="op"></param>
		/// <param name="hash"></param>
		/// <param name="commiter"></param>
		/// <param name="behavior"></param>
		/// <param name="sources"></param>
		/// <returns></returns>
		public static Commit Commit(this IMappingOperator op, string hash, string commiter, CommitHeadBehavior behavior, params string[] sources) {
			var commit = new Commit {Hash = hash, Author = new CommitAuthorInfo {Commiter = commiter, Time = DateTime.Now}};
			if (null != sources && 0 != sources.Length) {
				foreach (var s in sources) {
					commit.Sources.Add(s);
				}
			}
			return op.Commit(commit);
		}
	}
}