using System;
using Qorpent.Serialization;

namespace Qorpent.Utils.Git{
	/// <summary>
	/// 
	/// </summary>
	public class GitCommitInfo{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected bool Equals(GitCommitInfo other){
			return string.Equals(ShortHash, other.ShortHash);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode(){
			return (ShortHash != null ? ShortHash.GetHashCode() : 0);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((GitCommitInfo) obj);
		}
		/// <summary>
		/// 
		/// </summary>
		public string Hash { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string ShortHash { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Author { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string AuthorEmail { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime LocalRevisionTime { get; set; }
		/// <summary>
		/// Глобальное время
		/// </summary>
		public DateTime GlobalRevisionTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Commiter { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string CommiterEmail { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Comment { get; set; }

        /// <summary>
        /// Клиентский признак сведения с неким бранчем
        /// </summary>
        public bool Merged { get; set; }

	    private bool? _automerge;
	    /// <summary>
	    /// Автоматический коммит на слияние
	    /// </summary>
	    public bool IsAutoMergeCommit {
	        get {
                if (null == _automerge) {
                    _automerge =  this.Comment.StartsWith("Merge remote-tracking branch");
                }
	            return _automerge.Value;
	        }
	    }
		/// <summary>
		/// Расширяемое поле для дополнительных данных
		/// </summary>
		[SerializeNotNullOnly]
		public object CustomData { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Признак собственного коммита
		/// </summary>
		public bool IsOwnCommit { get; set; }
		/// <summary>
		/// Автор, разрезольвенный по собственной логике
		/// </summary>
		public string ResolvedAuthor { get; set; }
	}
}