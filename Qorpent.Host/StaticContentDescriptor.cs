using System;
using System.IO;

namespace Qorpent.Host{
	/// <summary>
	/// 
	/// </summary>
	public abstract class StaticContentDescriptor{
		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string FullName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string MimeType { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool IsFixedContent { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string FixedContent { get; set; }

		/// <summary>
		///		
		/// </summary>
		public abstract DateTime GetLastVersion();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public abstract Stream Open();
	}
}