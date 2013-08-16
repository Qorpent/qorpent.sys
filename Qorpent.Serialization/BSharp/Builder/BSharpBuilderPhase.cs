namespace Qorpent.BSharp.Builder {
	/// <summary>
	/// Фазы построения
	/// </summary>
	public enum BSharpBuilderPhase {
		/// <summary>
		/// 
		/// </summary>
		Init =1,
		/// <summary>
		/// 
		/// </summary>
		PreProcess =1<<1,
		/// <summary>
		/// 
		/// </summary>
		PreVerify =1<<2,
		/// <summary>
		/// 
		/// </summary>
		PreBuild =1<<3,
		/// <summary>
		/// 
		/// </summary>
		Build = 1<<4,
		/// <summary>
		/// 
		/// </summary>
		PostBuild =1 <<5,
		/// <summary>
		/// 
		/// </summary>
		PostVerify =1<<6,
		/// <summary>
		/// 
		/// </summary>
		PostProcess =1<<7
	}
}