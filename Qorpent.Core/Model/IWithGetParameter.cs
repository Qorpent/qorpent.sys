namespace Qorpent.Model {
	/// <summary>
	/// Интерфейс получения параметра по имени
	/// </summary>
	public interface IWithGetParameter {
		/// <summary>
		/// Получить значение именованного параметра
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		object GetParameter(string name);
	}
}