using System.Collections;

namespace Qorpent.Log{
	/// <summary>
	/// Конфигурация для отдельных видов сообщений
	/// </summary>
	public class SessionLogTypeConfiguration
	{
		/// <summary>
		/// Создает типовую конфигурацию для контроля значимых сообщений системы с апгрейдом
		/// </summary>
		/// <param name="code"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static SessionLogTypeConfiguration ApplicationAlert(string code,IEqualityComparer comparer = null){
			return new SessionLogTypeConfiguration{
				Code = code,
				IsSingleton = true,
				RequireAccept = true,
				ReopenAcceptOnUpdate = true,
				AutoAcceptBelow = LogLevel.Info,
				AutoRemoveBelow = LogLevel.Trace,
				CustomComparer = comparer,
				UpgradeTime = true
			};
		}
		/// <summary>
		/// Код для типа сообщений
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// Признак того, что сообщение может быть только в единственном экземпляре
		/// </summary>
		public bool IsSingleton { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool RequireAccept { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool ReopenAcceptOnUpdate { get; set; }
		/// <summary>
		/// Уровень при котором ошибка считается закрытой
		/// </summary>
		public LogLevel AutoAcceptBelow { get; set; }
		/// <summary>
		/// Требование обновлять версию сообщения
		/// </summary>
		public bool UpgradeTime { get; set; }
		/// <summary>
		/// Уровень, ниже которого можно удалить сообщение
		/// </summary>
		public LogLevel AutoRemoveBelow { get; set; }
		/// <summary>
		/// Кастомный класс для сравнения данных
		/// </summary>
		public IEqualityComparer CustomComparer { get; set; } 
	}
}