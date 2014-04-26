namespace Qorpent.Host
{
	/// <summary>
	/// 
	/// </summary>
	public enum HostApplicationMode
	{
		/// <summary>
		/// Внедренный сервер, не подменяет собой приложение
		/// </summary>
		Embeded,	
		/// <summary>
		/// Разделяет текущеее приложение, не инициирует новое приложение
		/// </summary>
		Shared,
		/// <summary>
		/// Автономный сервер, являющийся приложением
		/// </summary>
		Standalone,
	}
}