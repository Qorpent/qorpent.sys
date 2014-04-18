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
		/// ���������� �����
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
        /// ���������� ������� �������� � ����� �������
        /// </summary>
        public bool Merged { get; set; }

	    private bool? _automerge;
	    /// <summary>
	    /// �������������� ������ �� �������
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
		/// ����������� ���� ��� �������������� ������
		/// </summary>
		[SerializeNotNullOnly]
		public object CustomData { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }
	}
}