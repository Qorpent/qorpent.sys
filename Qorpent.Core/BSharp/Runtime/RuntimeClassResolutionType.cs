namespace Qorpent.BSharp.Runtime {
	/// <summary>
	/// Тип резолюции класса в рантайм
	/// </summary>
	public enum RuntimeClassResolutionType {
		/// <summary>
		/// В исходном коде не определен целевой тип рантайма
		/// </summary>
		NotDefined =1 ,
		/// <summary>
		/// Тип указан, но найти его в сборках не удалось
		/// </summary>
		NotResolved =1<<1,
		/// <summary>
		/// Тип найден в среде выполнения
		/// </summary>
		Resolved=1<<2,

		/// <summary>
		/// Найденный тип - интерфейс и требует инстанцирования через контенер
		/// </summary>
		ContainerService=1<<3,

		/// <summary>
		/// Найденный тип - имя объекта в контейнере, требует инстанцирования по имени
		/// </summary>
		ContainerName =1<<4,
	}
}