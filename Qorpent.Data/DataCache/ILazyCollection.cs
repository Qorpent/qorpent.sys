using System.Collections.Generic;

namespace Qorpent.Data.DataCache{
	/// <summary>
	/// Интерфейс ленивой коллекции
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ILazyCollection<T> : ICollection<T>{
		/// <summary>
		/// Признак того, что коллекция уже загрузилась
		/// </summary>
		bool WasLoaded { get; }
		/// <summary>
		/// Выполнение загрузки коллекции
		/// </summary>
		void Load();
	}
}