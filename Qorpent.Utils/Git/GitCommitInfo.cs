using System;

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
	}
}