using System;
using System.Collections.Generic;

namespace Qorpent.IoC {
	/// <summary>
	/// Интерфейс источника компонентов
	/// </summary>
	public interface IComponentSource {
		/// <summary>
		/// 	Получить все зарегистрированные компоненты
		/// </summary>
		/// <returns> Все компоненты контейнера </returns>
		IEnumerable<IComponentDefinition> GetComponents();

		/// <summary>
		/// 	Find best matched component for type/name or null for
		/// </summary>
		/// <param name="type"> The type. </param>
		/// <param name="name"> The name. </param>
		/// <returns> </returns>
		/// <exception cref="NotImplementedException"></exception>
		/// <remarks>
		/// </remarks>
		IComponentDefinition FindComponent(Type type, string name);
	}
}