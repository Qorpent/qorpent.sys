namespace Qorpent.BSharp.Builder {
	/// <summary>
	/// 
	/// </summary>
	public interface IBSharpBuilder {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="project"></param>
		void Initialize(IBSharpProject project);
		/// <summary>
		/// Построить проект
		/// </summary>
		/// <returns></returns>
		IBSharpContext Build();
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IBSharpProject GetProject();
	}
}