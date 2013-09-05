using System;

namespace Qorpent.IO.DirtyVersion.Mapping {
	/// <summary>
	/// Сессия для работы с мапингом версий
	/// </summary>
	public interface IMapperSession : IDisposable {
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IMappingOperator GetOperator();
	}
}