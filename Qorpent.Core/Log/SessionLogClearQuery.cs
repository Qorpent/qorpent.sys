namespace Qorpent.Log{
	/// <summary>
	/// Запрос на журнал сессии
	/// </summary>
	public class SessionLogClearQuery
	{
		/// <summary>
		/// 
		/// </summary>
		public SessionLogClearQuery(){
			KeepAllRequests = true;
			MaxLevel= LogLevel.Fatal;
		}
		/// <summary>
		/// Начальное время
		/// </summary>
		public int EndTimestamp { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool KeepAccpeted { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool KeepAllRequests { get; set; }
		/// <summary>
		/// Начальный уровень
		/// </summary>
		public LogLevel MaxLevel { get; set; }
	}
}