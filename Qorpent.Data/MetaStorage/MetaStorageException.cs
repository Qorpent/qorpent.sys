using System;

namespace Qorpent.Data.MetaStorage {
	/// <summary>
	/// Общее исключение репозитория Zeta
	/// </summary>
	[Serializable]
	public class MetaStorageException : QorpentException {


		/// <summary>
		/// 
		/// </summary>
		public MetaStorageException() {}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public MetaStorageException(string message) : base(message) {}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public MetaStorageException(string message, Exception inner) : base(message, inner) {}

	}
}