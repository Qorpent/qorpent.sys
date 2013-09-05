using System;
using Qorpent.IO.DirtyVersion.Mapping;

namespace Qorpent.IO.DirtyVersion {
	/// <summary>
	/// Сессия для работы с мапингом версий
	/// </summary>
	public interface IMapperSession : IDisposable {
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IMappingOperator GetOperator();
		/// <summary>
		/// Получить исходный мапинг
		/// </summary>
		/// <returns></returns>
		MappingInfo GetMappingInfo();

		/// <summary>
		/// Отменяет изменения и сбрасывает MappingInfo
		/// </summary>
		void Revert();
		/// <summary>
		/// Помечает сессию к сохранению
		/// </summary>
		void Commit();
	}
}