using System;

namespace Qorpent.Security.Watchdog {
	/// <summary>
	/// Any exception in paranoid environment
	/// </summary>
	[Serializable]
	public class ParanoidException : QorpentSecurityException {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		public ParanoidException(ParanoidState state):base(state.ToString()) {
			this.State = state;
		}

		/// <summary>
		/// State of paranoid
		/// </summary>
		public ParanoidState State { get;private set; }
	}
}