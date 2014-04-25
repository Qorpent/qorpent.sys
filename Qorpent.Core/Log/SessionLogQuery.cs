namespace Qorpent.Log{
	/// <summary>
	/// Запрос на журнал сессии
	/// </summary>
	public class SessionLogQuery{
		/// <summary>
		/// Начальное время
		/// </summary>
		public int StartTimestamp { get; set; }
		/// <summary>
		/// Только принятые
		/// </summary>
		public bool OnlyAccepted { get; set; }
		/// <summary>
		/// Только не принятые
		/// </summary>
		public bool OnlyNotAccepted { get; set; }
		/// <summary>
		/// Начальный уровень
		/// </summary>
		public LogLevel StartLevel { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool OnlyRequests { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Code { get; set; }
	}
}