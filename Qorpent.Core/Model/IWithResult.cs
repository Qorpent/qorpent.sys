namespace Qorpent.Model {
	/// <summary>
	///		Concern of having some result
	/// </summary>
	public interface IWithResult {
		/// <summary>
		///		Set a result
		/// </summary>
		/// <param name="result">Some result</param>
		void SetResult(object result);
		/// <summary>
		///		Get typed result
		/// </summary>
		/// <typeparam name="T">Type param of result</typeparam>
		/// <returns>Typed result</returns>
		T GetResult<T>();
	}
}